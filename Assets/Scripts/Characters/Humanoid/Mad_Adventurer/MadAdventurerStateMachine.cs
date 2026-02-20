using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class MadAdventurerStateMachine : MonoBehaviour
{
    [Header("Required parts")]
    [SerializeField] public NavMeshAgent NavMeshAgent;
    [SerializeField] public HumanoidController Controller;
    [SerializeField] public Animator Animator;
    [SerializeField] public DetectPlayer Vision;
    
    [Header("States")]
    public UnityEvent<MadAventurerBaseState> OnMadState;
    public MadAventurerBaseState CurrentState;
    public MadAdventurerIdleState IdleState = new MadAdventurerIdleState();
    public MadAdventurerChasingState ChasingState = new MadAdventurerChasingState();
    public MadAdventurerAttackState AttackState = new MadAdventurerAttackState();
    public MadAdventurerSearchingState SearchingState = new MadAdventurerSearchingState();
    
    [HideInInspector] public Transform PlayerTransform;
    [HideInInspector] public Transform TargetTransform;

#if DEBUG
    private void OnValidate()
    {
        IdleState.OnValidate(this);
        ChasingState.OnValidate(this);
        AttackState.OnValidate(this);
        SearchingState.OnValidate(this);
    }
#endif

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerTransform == null)
        {
            PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;

            if (PlayerTransform == null)
            {
                Debug.LogWarning("Cant find Player", this);
                return;
            }
        }

        IdleState.Intialize(this);
        ChasingState.Intialize(this);
        AttackState.Intialize(this);
        SearchingState.Intialize(this);

        IdleState.Start();
        ChasingState.Start();
        AttackState.Start();
        SearchingState.Start();
        CurrentState = IdleState;
        CurrentState.Enter();

        //agent
    }

    // Update is called once per frame
    void Update()
    {
        CurrentState?.Update();
    }
    
    public void Transit(MadAventurerBaseState targetState)
    {
        CurrentState.Exit();
        CurrentState = targetState;
        OnMadState.Invoke(CurrentState);
        CurrentState.Enter();
    }

    private void OnDrawGizmos()
    {
        if (CurrentState == null) return;

        Gizmos.color = Color.red;
        if (CurrentState.NavMeshPath != null && CurrentState.NavMeshPath.corners.Length > 0)
        {
            Vector3[] nodes = CurrentState.NavMeshPath.corners;
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                Gizmos.DrawSphere(nodes[i], 0.1f);
                Gizmos.DrawLine(nodes[i], nodes[i + 1]);
            }
            Gizmos.DrawSphere(nodes[^1], 0.1f);
        }
    }
}
