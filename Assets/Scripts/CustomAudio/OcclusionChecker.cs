using System;
using UnityEngine;

[System.Serializable]
public class OcclusionChecker
{
    public enum FirstDistance
    {
        DistanceToPlayer,
        MaxInputDistance,
    }
    
    private static float _occlusionScore;
    [SerializeField] public LayerMask layerMask;
    [Range(0, 60)] public float spread = 4;
    [Range(0, 1)]  public float bounceValue = 0.25f;
    [Range(0, 8)]  public int maxBounces;
    [Range(0,4)]   public int linesOnEitherSide;

    private const float Offset = 0.02f;
    
    private int _lineCount;
    private int _posModifier;
    private Vector3 _direction;
    private float _distance;
    private float _totalDistance;
    private Vector3 _sourcePos;
    private Vector3 _targetPos;
    
    public FirstDistance firstDistance;
    public bool checkIfFirstMiss;
    public bool drawDebug;

    [Serializable]
    public struct HitData
    {
        public float score;
        public RaycastHit[] Hits;
        public int castIndex;
    }
    public HitData[] hitDatas;
    
    //TODO: weighting på normalkurva? baserat på spread?
    //TODO: kolla ovan och under spelaren också?

    public void CheckOcclusion(GameObject sourceGo, GameObject targetGo, out float occlusion, float maxDistance = -1)
    {
        _lineCount = linesOnEitherSide * 2 + 1;
        hitDatas = new HitData[_lineCount];
        for (var i = 0; i < _lineCount; i++)
        {
            hitDatas[i] = new HitData()
            {
                score = 0,
                Hits = new RaycastHit[1 + maxBounces]
            };
        }

        _posModifier = -linesOnEitherSide - 1;
        occlusion = 0f;
        _occlusionScore = 0;
        _sourcePos = sourceGo.transform.position;

        for (var i = 0; i < hitDatas.Length; i++)
        {
            _posModifier++;
            _targetPos = targetGo.transform.position;
            switch (firstDistance)
            {
                case FirstDistance.DistanceToPlayer:
                    _distance = Vector3.Distance(_sourcePos, _targetPos);
                    break;
                case FirstDistance.MaxInputDistance:
                    if (maxDistance > 0) _distance = maxDistance;
                    else _distance = Vector3.Distance(_sourcePos, _targetPos) * 2f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _direction = (_targetPos - _sourcePos).normalized;
            _direction = (Quaternion.AngleAxis(spread * _posModifier, Vector3.up) * _direction).normalized;

            if (Physics.Raycast(_sourcePos, _direction, out hitDatas[i].Hits[0], _distance, layerMask))
            {
                if (drawDebug) Debug.DrawLine(_sourcePos, hitDatas[i].Hits[0].point, Color.red);
                
                for (var j = 1; j < hitDatas[i].Hits.Length; j++)
                {
                    hitDatas[i].score += bounceValue;
                    if (hitDatas[i].score > 1)
                    {
                        break;
                    }
                    
                    if (!Physics.Linecast(hitDatas[i].Hits[j - 1].point + hitDatas[i].Hits[j - 1].normal * Offset,
                            _targetPos, out hitDatas[i].Hits[j], layerMask))
                    {
                        if (drawDebug) Debug.DrawLine(hitDatas[i].Hits[j - 1].point + hitDatas[i].Hits[j - 1].normal * Offset,
                            _targetPos, Color.green);
                        hitDatas[i].castIndex = j;
                        break;
                    }
                    
                    _direction = Vector3.Reflect(_direction, hitDatas[i].Hits[j - 1].normal);
                    
                    if (j == hitDatas[i].Hits.Length - 1)
                    {
                        hitDatas[i].castIndex = j;
                        hitDatas[i].score = 1;
                        if (drawDebug)
                        {
                            _distance = Vector3.Distance(_targetPos, hitDatas[i].Hits[j - 1].point);
                            Debug.DrawRay(hitDatas[i].Hits[j - 1].point, _direction * _distance, Color.black);
                        }
                        break;
                    }
                    
                    if (maxDistance > 0) _distance = maxDistance;
                    else _distance = Mathf.Infinity;
                    
                    if (Physics.Raycast(hitDatas[i].Hits[j - 1].point, _direction, out hitDatas[i].Hits[j], _distance, layerMask))
                    {
                        if (drawDebug) Debug.DrawLine(hitDatas[i].Hits[j - 1].point, hitDatas[i].Hits[j].point, Color.blue);
                    }
                    else
                    {
                        if (drawDebug) Debug.DrawRay(hitDatas[i].Hits[j - 1].point, _direction * _distance, Color.black);
                        hitDatas[i].score = 1;
                        hitDatas[i].castIndex = j;
                        break;
                    }
                }
                
            }
            else
            {
                if (checkIfFirstMiss) //TODO: använda Hits[0].distance här?
                {
                    if (!Physics.Linecast(_sourcePos + (_direction.normalized * _distance), 
                            _targetPos, layerMask))
                    {
                        hitDatas[i].score += bounceValue;
                        if (drawDebug)
                        {
                            Debug.DrawLine(_sourcePos + (_direction.normalized * _distance),
                                _targetPos, Color.green);
                            Debug.DrawRay(_sourcePos, _direction.normalized * _distance, Color.blue);
                        }
                        hitDatas[i].castIndex = 1;
                    }
                    else
                    {
                        if (drawDebug)Debug.DrawRay(_sourcePos, _direction.normalized * _distance, Color.red);
                        hitDatas[i].score = 1;
                    }
                }
                else
                {
                    if (drawDebug)Debug.DrawRay(_sourcePos, _direction.normalized * _distance, Color.green);
                    hitDatas[i].score = 0;
                }
                
            }

            
        }
        foreach (var hitData in hitDatas)
        {
            _occlusionScore += Mathf.Clamp01(hitData.score);
        }
        occlusion += _occlusionScore / _lineCount;
    }
    
    
}
