using UnityEngine;
using UnityEngine.AI;


[System.Serializable]
public class MadAdventurerIdleState : MadAventurerBaseState
{
    [SerializeField, Tooltip("how far away in x it can move from its spawn"), Min(0f)] private float wanderRange = 5;
    [SerializeField, Tooltip("minimum wait time in seconds"), Min(0f)] private float minWaitTime = 0;
    [SerializeField, Tooltip("maximum wait time in seconds"), Min(1f)] private float maxWaitTime = 1;
    [SerializeField, Tooltip("minimum wait time in seconds"), Min(0f)] private float minWalkTime = 0;
    [SerializeField, Tooltip("maximum wait time in seconds"), Min(1f)] private float maxWalkTime = 1;
    private Vector3 spawnPosition;
    
    private float waitTime;
    private float walkTime;
    private bool walking;

    [Header("Player detection")] 
    [SerializeField] private float maxSightRange;
    [SerializeField] private float maxSoundRange;
    [SerializeField] private float sightThreshold;
    [SerializeField] private float soundThreshold;

    public override void Start()
    {
        base.Start();
        Physics.Raycast(mad.transform.position + Vector3.up, Vector3.down,out RaycastHit hit, LayerMask.GetMask("Ground"));
        spawnPosition = hit.point;
    }

    public override void Enter()
    {
        FindPath(GetRandomPosition());
        target = GetNextCorner();
        Reset();
        CombatChecker.RemoveFromChaseList(MyMadAdventurerStateMachine.gameObject);
    }

    public override void Exit()
    {
        CombatChecker.AddToChaseList(MyMadAdventurerStateMachine.gameObject);
    }

    public override void Update()
    {
        position = new Vector2(MyMadAdventurerStateMachine.transform.position.x, MyMadAdventurerStateMachine.transform.position.z);
        
        if(DetectPlayer())MyMadAdventurerStateMachine.Transit(MyMadAdventurerStateMachine.ChasingState);
        
        if (NavMeshPath.status == NavMeshPathStatus.PathInvalid || Vector2.Distance(position, new Vector2(NavMeshPath.corners[^1].x, NavMeshPath.corners[^1].z)) < minDistanceToCorner)
        {
            FindPath(GetRandomPosition());
        }
        
        if (walking)
        {
            if (walkTime <= 0)
            {
                walking = false;
                Stop();
            }
            else
            {
                walkTime -= Time.deltaTime;
                target = GetNextCorner();
                Move();
            }
        }
        else
        {
            if (waitTime <= 0)
            {
                walking = true;
                Reset();
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }

    private bool DetectPlayer()
    {
        return MyMadAdventurerStateMachine.Vision.SightDetection(maxSightRange) > sightThreshold || MyMadAdventurerStateMachine.Vision.SoundDetection(maxSoundRange) > soundThreshold;
    }

    private void Reset()
    {
        waitTime = Random.Range(minWaitTime, maxWaitTime);
        walkTime = Random.Range(minWalkTime, maxWalkTime);
    }

    private Vector3 GetRandomPosition()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float magnitude = Random.Range(0, wanderRange);
        return spawnPosition + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * magnitude;
    }
}
