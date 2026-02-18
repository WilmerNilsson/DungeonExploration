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
    
    [HideInInspector] public Transform player;
    [HideInInspector] public Transform target;
<<<<<<< HEAD
    
=======

#if DEBUG
>>>>>>> NewNewMain
    private void OnValidate()
    {
        idleState.OnValidate(this);
        chasingState.OnValidate(this);
        meleeState.OnValidate(this);
        searchingState.OnValidate(this);
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

<<<<<<< HEAD
=======
        idleState.Intialize(this);
        chasingState.Intialize(this);
        meleeState.Intialize(this);
        searchingState.Intialize(this);

>>>>>>> NewNewMain
        idleState.Start();
        chasingState.Start();
        meleeState.Start();
        searchingState.Start();
        currentState = idleState;
        currentState.Enter();

        //agent
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< HEAD
        currentState.Update();
=======
        currentState?.Update();
>>>>>>> NewNewMain
    }
    
    public void Transit(MadState targetState)
    {
        currentState.Exit();
        currentState = targetState;
        onMadState.Invoke(currentState);
        currentState.Enter();
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
