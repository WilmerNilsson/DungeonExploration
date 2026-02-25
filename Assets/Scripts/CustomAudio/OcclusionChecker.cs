using System;
using UnityEngine;

[System.Serializable]
public class OcclusionChecker
{
    private static float _occlusionScore;
    [SerializeField] private LayerMask layerMask;
    [Range(0, 10)][SerializeField] private float spread = 4;
    [Range(0, 1)] [SerializeField] private float bounceValue = 0.25f;
    [Range(0, 8)] [SerializeField] private int maxBounces;
    [Range(0,4)][SerializeField] private int linesOnEitherSide;

    private const float Offset = 0.02f;
    
    private int _lineCount;
    private int _posModifier;
    private Vector3 _direction;
    private float _distance;
    private float _totalDistance;
    private Vector3 _sourcePos;
    private Vector3 _targetPos;

    [SerializeField] private bool allowBounce;
    [SerializeField] private bool drawDebug;

    [Serializable]
    public struct HitData
    {
        public float score;
        public RaycastHit[] Hits;
        public int castIndex;
    }
    
    public HitData[] hitDatas;
    
    
    
    //TODO: punkterna följer inte spelarens rotation, utan istället emittern? prob inte göra detta
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
            _distance = Vector3.Distance(_sourcePos, _targetPos);
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
                if (drawDebug)Debug.DrawRay(_sourcePos, _direction * _distance, Color.green);
                hitDatas[i].score = 0;
            }

            
        }
        foreach (var hitData in hitDatas)
        {
            _occlusionScore += Mathf.Clamp01(hitData.score);
        }
        Debug.Log(_occlusionScore + "     " + _lineCount);
        occlusion += _occlusionScore / _lineCount;
    }
}

/*
for (var i = 0; i < _lineCount; i++)
{
    _posModifier++;
    _targetPos = targetGo.transform.position;
    _distance = Vector3.Distance(_sourcePos, _targetPos);
    _direction = (_targetPos - _sourcePos).normalized;
    _direction = (Quaternion.AngleAxis(spread * _posModifier, Vector3.up) * _direction).normalized;
    if (Physics.Raycast(_sourcePos, _direction, out hits[i][0], _distance, layerMask))
    {
        //Om första raycast träffar något gör vi studs calculations
        if (drawDebug) Debug.DrawLine(_sourcePos, hits[i][0].point, Color.cyan);
        for (var j = 1; j < hits[i].Length; j++)
        {
            if (j == hits[i].Length - 1)
            {
                _occlusionScore += 1;
            }
            else
            {
                if (i == 0) Debug.Log(Mathf.Clamp01(bounceValue * j));
                _occlusionScore += Mathf.Clamp01(bounceValue * j);
            }
            
            if (!Physics.Linecast(hits[i][j-1].point + hits[i][j-1].normal * Offset, _targetPos, layerMask))
            {
                //Om inte träffar något lägg till occlusion och sen break
                if (drawDebug) Debug.DrawLine(hits[i][j-1].point + hits[i][j-1].normal * Offset, _targetPos, Color.green);
                break;
            }

            //Om träff studsa vidare
            _direction = Vector3.Reflect(_direction, hits[i][j-1].normal);
            if (Physics.Raycast(hits[i][j-1].point, _direction, out hits[i][j], layerMask))
            {
                //Om studs träffar
                if (drawDebug) Debug.DrawLine(hits[i][j-1].point, hits[i][j].point, Color.cyan);
                //Om sista studs och inte träff lägg till max occlusion
            }
            else
            {
                //Om studs inte träffar något (ray åkt skitlångt bort) max occlusion
                //_occlusionScore += hits[i].Length - 1;
                occlusion = 1;
                break;
            }
        }
    }
    else
    {
        //Om inte träff ingen occlusion
        if (drawDebug) Debug.DrawRay(_sourcePos, _direction * _distance, Color.green);
    }
    
}
*/