using UnityEngine;
using UnityEngine.AI;


[System.Serializable]
public class MadIdle : MadState
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

    public override void OnValidate(MadAdventurer madAdventurer)
    {
        base.OnValidate(madAdventurer);
        Physics.Raycast(mad.transform.position, Vector3.down,out RaycastHit hit, LayerMask.GetMask("Ground"));
        spawnPosition = hit.point;
    }

    public override void Enter()
    {
        FindPath(GetRandomPosition());
        target = GetNextCorner();
        Reset();
        CombatChecker.RemoveFromChaseList(mad.gameObject);
    }

    public override void Exit()
    {
        CombatChecker.AddToChaseList(mad.gameObject);
    }

    public override void Update()
    {
        position = new Vector2(mad.transform.position.x, mad.transform.position.z);
        
        if(DetectPlayer())mad.Transit(mad.chasingState);
        
        if (path.status == NavMeshPathStatus.PathInvalid || Vector2.Distance(position, new Vector2(path.corners[^1].x, path.corners[^1].z)) < minDistanceToCorner)
        {
            FindPath(GetRandomPosition());
        }
        
        if (walking)
        {
            if (walkTime <= 0)
            {
                walking = false;
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
        return mad.vision.SightDetection(maxSightRange) > sightThreshold || mad.vision.SoundDetection(maxSoundRange) > soundThreshold;
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
