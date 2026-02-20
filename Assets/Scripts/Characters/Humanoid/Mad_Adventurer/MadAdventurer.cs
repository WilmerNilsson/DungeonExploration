using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class MadAdventurer : MonoBehaviour
{
    [Header("Required parts")]
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public HumanoidController controller;
    [SerializeField] public Animator animator;
    [SerializeField] public DetectPlayer vision;
    
    [Header("States")]
    public UnityEvent<MadState> onMadState;
    public MadState currentState;
    public MadIdle idleState = new MadIdle();
    public MadChasing chasingState = new MadChasing();
    public MadMelee meleeState = new MadMelee();
    public MadSearching searchingState = new MadSearching();
    public MadDyingState dyingState = new MadDyingState();
    
    [HideInInspector] public Transform player;
    [HideInInspector] public Transform target;

#if DEBUG
    private void OnValidate()
    {
        idleState.OnValidate(this);
        chasingState.OnValidate(this);
        meleeState.OnValidate(this);
        searchingState.OnValidate(this);
        dyingState.OnValidate(this);
    }
#endif

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;

            if (player == null)
            {
                Debug.LogWarning("Cant find Player", this);
                return;
            }
        }

        idleState.Intialize(this);
        chasingState.Intialize(this);
        meleeState.Intialize(this);
        searchingState.Intialize(this);
        dyingState.Intialize(this);

        idleState.Start();
        chasingState.Start();
        meleeState.Start();
        searchingState.Start();
        dyingState.Start();
        currentState = idleState;
        currentState.Enter();

        //agent
    }

    // Update is called once per frame
    void Update()
    {
        currentState?.Update();
    }
    
    public void Transit(MadState targetState)
    {
        currentState.Exit();
        currentState = targetState;
        onMadState.Invoke(currentState);
        currentState.Enter();
    }

    public void Die()
    {
        Transit(dyingState);
    }

    private void OnDrawGizmos()
    {
        if (currentState == null) return;

        Gizmos.color = Color.red;
        if (currentState.path != null && currentState.path.corners.Length > 0)
        {
            Vector3[] nodes = currentState.path.corners;
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                Gizmos.DrawSphere(nodes[i], 0.1f);
                Gizmos.DrawLine(nodes[i], nodes[i + 1]);
            }
            Gizmos.DrawSphere(nodes[^1], 0.1f);
        }
    }
}
