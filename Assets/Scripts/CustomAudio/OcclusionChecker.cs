using UnityEngine;

[System.Serializable]
public class OcclusionChecker
{
    private static float _hits;
    [SerializeField] private LayerMask layerMask;
    [Range(0, 3)][SerializeField] private float spread = 1f;
    [Range(0, 1)] [SerializeField] private float bounceValue = 0.5f;
    [Range(0,4)][SerializeField] private int linesOnEitherSide;
    
    private int _lineCount;
    private int _posModifier;
    private Vector3 _sourcePos;
    private Vector3 _targetPos;

    [SerializeField] private bool allowBounce;
    [SerializeField] private bool drawDebug;

    private struct HitData
    {
        public bool Hit1;
        public RaycastHit Hit1Info;
        public bool Hit2;
        public RaycastHit Hit2Info;
    }
    
    //TODO: punkterna följer inte spelarens rotation, utan istället emittern? prob inte göra detta
    //TODO: weighting på normalkurva? baserat på spread?
    //TODO: kolla ovan och under spelaren också?
    
    public void CheckOcclusion(GameObject sourceGo, GameObject targetGo, out float occlusion)
    {
        _lineCount = linesOnEitherSide * 2 + 1;
        var hits = new HitData[_lineCount];
        _posModifier = -linesOnEitherSide - 1;
        occlusion = 0f;
        _hits = 0;
        _sourcePos = sourceGo.transform.position;
        for (int i = 0; i < _lineCount; i++)
        {
            _posModifier++;
            _targetPos = targetGo.transform.position + targetGo.transform.right * (spread * _posModifier);
            hits[i].Hit1 = Physics.Linecast(_sourcePos, _targetPos, out hits[i].Hit1Info, layerMask);
            if (hits[i].Hit1)
            {
                if (allowBounce)
                {
                    hits[i].Hit2 = Physics.Linecast(hits[i].Hit1Info.point, targetGo.transform.position, out hits[i].Hit2Info, layerMask);
                    if (hits[i].Hit2) _hits++;
                    else _hits += bounceValue;
                }
                else
                {
                    _hits++;
                }
            }
            
            //Draw lines
            if (!drawDebug) continue;

            if (hits[i].Hit1)
            {
                if (!hits[i].Hit2 && allowBounce)
                {
                    Debug.DrawLine(_sourcePos, hits[i].Hit1Info.point, Color.cyan);
                    Debug.DrawLine(hits[i].Hit1Info.point, targetGo.transform.position, Color.green);
                }
                else
                {
                    Debug.DrawLine(_sourcePos, hits[i].Hit1Info.point, Color.red);
                }
            }
            else
            {
                Debug.DrawLine(_sourcePos, _targetPos, Color.green);
            }
        }
        occlusion = _hits / _lineCount;
    }
}
