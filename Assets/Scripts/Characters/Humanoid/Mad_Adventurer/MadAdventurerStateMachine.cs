using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MadAdventurerStateMachine : MonoBehaviour
{
    [SerializeField] private string stateName;
    
    [Header("Required parts")]
    [SerializeField] public NavMeshAgent NavMeshAgent;
    [SerializeField] public HumanoidController Controller;
    [SerializeField] public Animator Animator;
    [SerializeField] public DetectPlayer Vision;
    
    [Header("Player detection")] 
    [SerializeField] private float sightThreshold;
    [SerializeField] private float soundThreshold;
    
    [Header("States")]
    public UnityEvent<MadAventurerBaseState> OnMadState;
    public MadAventurerBaseState CurrentState;
    public MadAdventurerIdleState IdleState = new MadAdventurerIdleState();
    public MadAdventurerChasingState ChasingState = new MadAdventurerChasingState();
    public MadAdventurerAttackState AttackState = new MadAdventurerAttackState();
    public MadAdventurerSearchingState SearchingState = new MadAdventurerSearchingState();
    public MadAdventurerDyingState DyingState = new();
    public MadAdventurerHallucinationState HallucinationState = new();
    [SerializeField] private bool startInHallucination = false;
    
    [HideInInspector] public Transform PlayerTransform;
    [HideInInspector] public Transform TargetTransform;

    private float lastAngle;
    
    [SerializeField] internal bool isAttacking;
    private float startTime;
    
    internal Coroutine currentAttack;

#if DEBUG
    private void OnValidate()
    {
        IdleState.OnValidate(this);
        ChasingState.OnValidate(this);
        AttackState.OnValidate(this);
        SearchingState.OnValidate(this);
        DyingState.OnValidate(this);
        HallucinationState.OnValidate(this);
    }
#endif

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerTransform == null)
        {
            try
            {
                PlayerTransform = PlayerTrackerSingleton.Instance.playerGameObject.transform;
            }
            catch
            {
                Console.WriteLine("Could not find player transform");
                throw;
            }
        }

        IdleState.Intialize(this);
        ChasingState.Intialize(this);
        AttackState.Intialize(this);
        SearchingState.Intialize(this);
        DyingState.Intialize(this);
        HallucinationState.Intialize(this);

        IdleState.Start();
        ChasingState.Start();
        AttackState.Start();
        SearchingState.Start();
        DyingState.Start();
        HallucinationState.Start();
        if(startInHallucination)
        {
            CurrentState = HallucinationState;
        }
        else
        {
            CurrentState = IdleState;
        }
        stateName = CurrentState.GetType().Name;
        CurrentState.Enter();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentState?.Update();
    }
    
    public void Transit(MadAventurerBaseState targetState)
    {
        CurrentState?.Exit();
        CurrentState = targetState;
        OnMadState.Invoke(CurrentState);
        stateName = CurrentState.GetType().Name;
        CurrentState.Enter();
    }
    
    public bool DetectPlayer()
    {
        return Vision.Detect(sightThreshold, soundThreshold);
    }
    
    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        startTime = Time.time;
        
        float angle = Random.Range(0, 180);
        if (lastAngle < 180) angle += 180f;

        yield return new WaitUntil(HoldPart);
        
        if(Controller.TryAttack(angle)) lastAngle = angle;
        isAttacking = false;

        bool HoldPart()
        {
            Controller.HoldAttackUpdate(angle);
            return Time.time - startTime > 0;
        }
    }

    public void Attack()
    {
        currentAttack = StartCoroutine(AttackRoutine());
    }

    public void Die()
    {
        if(currentAttack != null)
        {
            StopCoroutine(currentAttack);
        }
        Transit(DyingState);
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
