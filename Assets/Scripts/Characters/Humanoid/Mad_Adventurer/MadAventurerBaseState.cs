using System;
using UnityEngine;
using UnityEngine.AI;


[System.Serializable]
public abstract class MadAventurerBaseState
{
    [HideInInspector] public MadAdventurerStateMachine MyMadAdventurerStateMachine = null;
    [SerializeField] protected float minDistanceToCorner = 0.1f;
    public NavMeshPath NavMeshPath;
    [SerializeField] protected Vector3 target;
    protected Vector2 position;
    protected int pathIndex;


    public virtual void OnValidate(MadAdventurerStateMachine madAdventurer) { }
    
    public virtual void Intialize(MadAdventurerStateMachine madAdventurer)
    {
        MyMadAdventurerStateMachine = madAdventurer;
        NavMeshPath = new NavMeshPath();
    }

    public virtual void Awake(){}
    public virtual void Start(){}

    public virtual void Enter(){}
    
    public virtual void Exit(){}

    public virtual void Update()
    {
        Move(Vector3.forward);
    }
    
    protected virtual void OnHit(){}
    
    protected virtual void OnDeath(){}

    protected virtual bool DetectPlayer()
    {
        return MyMadAdventurerStateMachine.DetectPlayer();
    }
    protected void Stop()
    {
        MyMadAdventurerStateMachine.Controller.Move(Vector3.zero);
    }

    /// <summary>
    /// Rotates to look at the target and then moves forward
    /// </summary>
    protected virtual void Move(Vector3 direction)
    {
        MyMadAdventurerStateMachine.Controller.Rotate(Quaternion.LookRotation((target-MyMadAdventurerStateMachine.transform.position)));
        MyMadAdventurerStateMachine.Controller.Move(direction);
    }
    
    /// <summary>
    /// Calculates a new path to the target position
    /// </summary>
    protected void FindPath(Vector3 pos)
    {
        pathIndex = 1;

        MyMadAdventurerStateMachine.NavMeshAgent.CalculatePath(pos, NavMeshPath);
    }
    
    protected Vector3 GetNextCorner()
    {
        if (NavMeshPath.status == NavMeshPathStatus.PathInvalid) return target;
        Vector2 corner = new Vector2(NavMeshPath.corners[pathIndex].x, NavMeshPath.corners[pathIndex].z);
        
        if (Vector2.Distance(position, corner) <= minDistanceToCorner)
        {
            if (pathIndex < NavMeshPath.corners.Length-1) pathIndex++;
        }
        
        return NavMeshPath.corners[pathIndex];
    }
}
