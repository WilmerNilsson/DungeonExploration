using System;
using UnityEditor;
using UnityEngine;

public class OcclusionChecker : MonoBehaviour
{
    private static float _hits;
    [SerializeField] private LayerMask layerMask;
    private GameObject _lastSource;
    private GameObject _lastTarget;
    [SerializeField] private float spread;
    
    private bool[] linesHit = new bool[5];
    public void CheckOcclusion(GameObject source, GameObject target, out float occlusion) //TODO gör detta bättre, med wheights för alla lines och liknande
    {
        _lastSource = source;
        _lastTarget = target;
        occlusion = 0;
        _hits = 0;
        linesHit = new bool[5];
        if (Physics.Linecast(source.transform.position, target.transform.position + target.transform.right * (-spread*2), layerMask))
        {
            _hits++;
            linesHit[0] = true;
        }
        if (Physics.Linecast(source.transform.position, target.transform.position + target.transform.right * -spread, layerMask))
        {
            _hits++;
            linesHit[1] = true;
        }
        if (Physics.Linecast(source.transform.position, target.transform.position, layerMask))
        {
            _hits++;
            linesHit[2] = true;
        }
        if (Physics.Linecast(source.transform.position, target.transform.position + target.transform.right * spread, layerMask))
        {
            _hits++;
            linesHit[3] = true;
        }
        if (Physics.Linecast(source.transform.position, target.transform.position + target.transform.right * (spread * 2), layerMask))
        {
            _hits++;
            linesHit[4] = true;
        }
        
        occlusion = _hits / 5;
        Debug.Log(occlusion);
    }

    public void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        if (AudioManager.IsValid)
        {
            if (AudioManager.Instance.debug)
            {
                Gizmos.color = linesHit[0] ? Color.red : Color.green;
                Gizmos.DrawLine(_lastSource.transform.position, _lastTarget.transform.position + _lastTarget.transform.right * (-spread*2));
                Gizmos.color = linesHit[1] ? Color.red : Color.green;
                Gizmos.DrawLine(_lastSource.transform.position, _lastTarget.transform.position + _lastTarget.transform.right * -spread);
                Gizmos.color = linesHit[2] ? Color.red : Color.green;
                Gizmos.DrawLine(_lastSource.transform.position, _lastTarget.transform.position);
                Gizmos.color = linesHit[3] ? Color.red : Color.green;
                Gizmos.DrawLine(_lastSource.transform.position, _lastTarget.transform.position + _lastTarget.transform.right * spread);
                Gizmos.color = linesHit[4] ? Color.red : Color.green;
                Gizmos.DrawLine(_lastSource.transform.position, _lastTarget.transform.position + _lastTarget.transform.right * (spread * 2));
            }
        }
    }
}
