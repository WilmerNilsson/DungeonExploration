using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class WallChecker
{
    private const int MaxWalls = 3;

    [SerializeField] private LayerMask wallLayers;
    [SerializeField] private LayerMask groundLayers;

    [SerializeField] private bool drawDebug;
    
    private List<RaycastHit> _hits = new List<RaycastHit>();

    [SerializeField] private float[] distances;
    
    private RaycastHit[] _groundHits = new RaycastHit[3];
    
    [Range(0f, 5f)]
    [SerializeField] private float minHitDistance;
    
    private Vector3 _direction;
    private float _distance;
    
    private Color _lGrey = new Color(0.66f, 0.66f, 0.66f);
    private Color _dGrey = new Color(0.33f, 0.33f, 0.33f);
    
    public void CheckWalls(GameObject sourceGo, GameObject targetGo, out int wallCount)
    {
        _direction = (targetGo.transform.position - sourceGo.transform.position).normalized;
        _distance = Vector3.Distance(sourceGo.transform.position, targetGo.transform.position);
        if (Physics.Linecast(sourceGo.transform.position, targetGo.transform.position, out _groundHits[0], groundLayers))
        {
            if (drawDebug) Debug.DrawLine(sourceGo.transform.position, _groundHits[0].point, Color.white);
            if (Physics.Raycast(_groundHits[0].point, Vector3.Reflect(_direction, _groundHits[0].normal), out _groundHits[1],
                    Mathf.Infinity, groundLayers + wallLayers))
            {
                if (drawDebug) Debug.DrawLine(_groundHits[0].point, _groundHits[1].point, Color.white);
                if (!Physics.Linecast(_groundHits[1].point, targetGo.transform.position, out _groundHits[2], groundLayers + wallLayers))
                {
                    if (drawDebug) Debug.DrawLine(_groundHits[1].point, targetGo.transform.position, Color.white);
                    wallCount = 0;
                }
                else wallCount = MaxWalls;
            }
            else wallCount = MaxWalls;
        }
        else
        {
            _hits = Physics.RaycastAll(sourceGo.transform.position, _direction, _distance, wallLayers).ToList();
            distances = new float[_hits.Count];
            if (_hits.Count > 1)
            {
                for (var i = 1; i < _hits.Count; i++)
                {
                    distances[i] = Vector3.Distance(_hits[i].point, _hits[i - 1].point);
                    if (Vector3.Distance(_hits[i-1].point, _hits[i].point) < minHitDistance) _hits.RemoveAt(i);
                }
            }
            wallCount = _hits.Count;
        }
        wallCount = Mathf.Clamp(wallCount, 0, MaxWalls);
        
        //Draw rays
        if (!drawDebug) return;
        switch (wallCount)
        {
            case <1:
                Debug.DrawRay(sourceGo.transform.position, _direction * _distance, Color.white);
                break;
            case 1:
                Debug.DrawRay(sourceGo.transform.position, _direction * _distance, _lGrey);
                break;
            case 2:
                Debug.DrawRay(sourceGo.transform.position, _direction * _distance, _dGrey);
                break;
            case >2:
                Debug.DrawRay(sourceGo.transform.position, _direction * _distance, Color.black);
                break;
        }
    }
}
