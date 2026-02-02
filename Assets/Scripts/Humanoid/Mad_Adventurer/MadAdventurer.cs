using System;
using UnityEngine;
using UnityEngine.AI;

public class MadAdventurer : MonoBehaviour
{
    [Header("Required parts")]
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public Controller controller;
    [SerializeField] public Animator animator;
    
    [Header("States")]
    public MadState currentState;
    public MadIdle idleState = new MadIdle();
    public MadChasing chasingState = new MadChasing();
    public MadMelee meleeState = new MadMelee();
    
    [HideInInspector] public Transform target;

    private void OnValidate()
    {
        idleState.OnValidate(this);
        chasingState.OnValidate(this);
        meleeState.OnValidate(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        idleState.Start();
        chasingState.Start();
        meleeState.Start();
        currentState = idleState;
        currentState.Enter();
    }

    // Update is called once per frame
    void Update()
    {
        currentState.FixedUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            target = other.transform;
            Transit(chasingState);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            target = null;
            Transit(idleState);
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
