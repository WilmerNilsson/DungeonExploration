using System;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine;

public class OcclusionChecker : MonoBehaviour
{
    private static float _hits;
    [SerializeField] private LayerMask layerMask;
    [Range(0, 3)][SerializeField] private float spread = 1f;
    [Range(0, 1)] [SerializeField] private float bounceValue = 0.5f;
    [Range(0,4)][SerializeField] private int linesOnEitherSide;
    
    private int lineCount;
    private int posModifier;
    private Vector3 sourcePos;
    private Vector3 targetPos;

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
        lineCount = linesOnEitherSide * 2 + 1;
        var hits = new HitData[lineCount];
        posModifier = -linesOnEitherSide - 1;
        occlusion = 0f;
        _hits = 0;
        sourcePos = sourceGo.transform.position;
        for (int i = 0; i < lineCount; i++)
        {
            posModifier++;
            targetPos = targetGo.transform.position + targetGo.transform.right * (spread * posModifier);
            hits[i].Hit1 = Physics.Linecast(sourcePos, targetPos, out hits[i].Hit1Info, layerMask);
            if (hits[i].Hit1)
            {
                hits[i].Hit2 = Physics.Linecast(hits[i].Hit1Info.point, targetGo.transform.position, out hits[i].Hit2Info, layerMask);
                if (hits[i].Hit2) _hits++;
                else _hits += bounceValue;
            }
            
            //Draw lines
            if (!AudioManager.IsValid) return;
            if (!AudioManager.Instance.debug) return;

            if (hits[i].Hit1)
            {
                if (!hits[i].Hit2)
                {
                    Debug.DrawLine(sourcePos, hits[i].Hit1Info.point, Color.cyan);
                    Debug.DrawLine(hits[i].Hit1Info.point, targetGo.transform.position, Color.green);
                }
                else
                {
                    Debug.DrawLine(sourcePos, hits[i].Hit1Info.point, Color.red);
                }
            }
            else
            {
                Debug.DrawLine(sourcePos, targetPos, Color.green);
            }
        }
        occlusion = _hits / lineCount;
    }
}
