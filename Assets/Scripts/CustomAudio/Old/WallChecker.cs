using UnityEngine;
using System;

[Serializable]
public class WallChecker
{
    private const int MaxWalls = 3;

    [SerializeField] private LayerMask wallLayers;
    [SerializeField] private LayerMask groundLayers;

    [SerializeField] private bool drawDebug;
    
    private RaycastHit[] _hits = new RaycastHit[MaxWalls];
    
    private Vector3 _direction;
    private float _distance;
    
    private Color _lGrey = new Color(0.66f, 0.66f, 0.66f);
    private Color _dGrey = new Color(0.33f, 0.33f, 0.33f);
    
    public void CheckWalls(GameObject sourceGo, GameObject targetGo, out int wallCount)
    {
        _direction = (targetGo.transform.position - sourceGo.transform.position).normalized;
        _distance = Vector3.Distance(sourceGo.transform.position, targetGo.transform.position);
        if (Physics.Linecast(sourceGo.transform.position, targetGo.transform.position, out var groundHit, groundLayers))
        {
            wallCount = MaxWalls;
        }
        else
        {
            var size = Physics.RaycastNonAlloc(sourceGo.transform.position, _direction, _hits, _distance, wallLayers);
            wallCount = size;
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
