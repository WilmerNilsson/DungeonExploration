using System;
using UnityEngine;
using UnityEngine.AI;


[System.Serializable]
public class MadState
{
    [HideInInspector] public MadAdventurer mad = null;
    
    public Vector3 target;
    public NavMeshPath path;
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
        target = path.corners[pathIndex+1];
        mad.controller.Rotate(Quaternion.LookRotation((target-mad.transform.position)+Vector3.up));
        mad.controller.Move(Vector3.forward);
    }
}
