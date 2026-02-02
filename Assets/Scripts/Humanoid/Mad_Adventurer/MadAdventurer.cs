using System;
using UnityEngine;
using UnityEngine.AI;

public class MadAdventurer : MonoBehaviour
{
    [Header("Required parts")]
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public Controller controller;
    [SerializeField] private Animator animator;
    
    [Header("States")]
    public MadState currentState;
    public MadIdle idleState = new MadIdle();
    public MadChasing chasingState = new MadChasing();
    public MadMelee meleeState = new MadMelee();
    public MadSearching searchingState = new MadSearching();
    
    private NavMeshPath path;
    private NavMeshPath tempPath;
    private bool hasTarget;
    [HideInInspector] public Transform target;
    private Vector3 currentTarget;
    private Vector3 realTarget;

    private void OnValidate()
    {
        idleState.OnValidate(this);
        chasingState.OnValidate(this);
        meleeState.OnValidate(this);
        searchingState.OnValidate(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        path = new NavMeshPath();
        tempPath = new NavMeshPath();
        
        idleState.Start();
        chasingState.Start();
        meleeState.Start();
        searchingState.Start();
        currentState = idleState;
        currentState.Enter();
    }

    // Update is called once per frame
    void Update()
    {
        currentState.FixedUpdate();
    }

    private void Chase() // Moving towards target
    {
        if (hasTarget)
        {
            agent.CalculatePath(target.position, path);
            currentTarget = path.corners[1];
            realTarget = currentTarget - transform.position;
            controller.Rotate(Quaternion.LookRotation(realTarget));
            controller.Move(Vector3.forward);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Transit(chasingState);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Transit(searchingState);
        }
    }
    
    public void Transit(MadState targetState)
    {
        currentState.Exit();
        currentState = targetState;
        currentState.Enter();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(currentState.target, 0.1f);
        Gizmos.DrawLine(transform.position, currentState.target);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
    }
}
