using System;
using UnityEngine;
using UnityEngine.AI;


[System.Serializable]
public class MadState
{
    [HideInInspector] public MadAdventurer mad = null;
    [SerializeField] protected float minDistanceToCorner;
    public NavMeshPath path;
    protected Vector3 target;
    protected Vector2 position;
    protected int pathIndex;


    public virtual void OnValidate(MadAdventurer madAdventurer)
    {
        mad = madAdventurer;
        path = new NavMeshPath();
    }
    
    public virtual void Awake(){}
    public virtual void Start(){}

    public virtual void Enter(){}
    
    public virtual void Exit(){}

    public virtual void Update()
    {
        Move();
    }
    
    protected virtual void OnHit(){}
    
    protected virtual void OnDeath(){}

    protected void Stop()
    {
        mad.controller.Move(Vector3.zero);
    }

    protected virtual void Move()
    {
        mad.controller.Rotate(Quaternion.LookRotation((target-mad.transform.position)+Vector3.up));
        mad.controller.Move(Vector3.forward);
    }
    
    protected Vector3 GetNextCorner()
    {
        Vector2 corner = new Vector2(path.corners[pathIndex].x, path.corners[pathIndex].z);
        
        if (Vector2.Distance(position, corner) <= minDistanceToCorner)
        {
            if (pathIndex < path.corners.Length) pathIndex++;
        }
        
        return path.corners[pathIndex];
    }
}
