using UnityEngine;

[System.Serializable]
public class MadIdle : MadState
{
    [SerializeField, Tooltip("how far away it can move from its spawn"), Min(0f)] private float wanderRange = 5;
    [SerializeField, Tooltip("minimum wait time in seconds"), Min(0f)] private float minWaitTime = 0;
    [SerializeField, Tooltip("maximum wait time in seconds"), Min(1f)] private float maxWaitTime = 1;
    [SerializeField, Tooltip("minimum wait time in seconds"), Min(0f)] private float minWalkTime = 0;
    [SerializeField, Tooltip("maximum wait time in seconds"), Min(1f)] private float maxWalkTime = 1;
    private Vector3 spawnPosition;
    private Vector3 position;
    
    private Quaternion targetRotation;
    private float newAngle;
    
    private float waitTime;
    private float walkTime;
    
    private float startTime = 0;
    private float time = 0;

    public override void OnValidate(MadAdventurer madAdventurer)
    {
        base.OnValidate(madAdventurer);
        spawnPosition = mad.transform.position;
    }

    public override void Enter()
    {
        base.Enter();
        FindPath(getRandomPosition());
        CombatChecker.RemoveFromChaseList(mad.gameObject);
    }

    public override void Exit()
    {
        CombatChecker.AddToChaseList(mad.gameObject);
    }

    public override void Update()
    {
        
        if (walkTime > 0)// Walk
        {
            walkTime -= Time.deltaTime;
            Move();
        }
        else if (waitTime > 0)// wait
        {
            waitTime -= Time.deltaTime;
        }
        else
        {
            Reset();
        }
    }

    protected override void Move()
    {
        if (Vector3.Distance(mad.transform.position, spawnPosition) > wanderRange)
        {
            targetRotation = Quaternion.LookRotation(getRandomPosition());
        }
        mad.controller.Rotate(targetRotation);
        mad.controller.Move(Vector3.forward);
    }

    private bool FindPath(Vector3 pos)
    {
        return mad.agent.CalculatePath(pos, path);
    }

    private Vector3 getRandomPosition()
    {
        return spawnPosition + new Vector3(Random.Range(-wanderRange, wanderRange), 0f, Random.Range(-wanderRange, wanderRange)).normalized * wanderRange;
    }

    private void Reset()
    {
        walkTime = Random.Range(minWalkTime, maxWalkTime);
        waitTime = Random.Range(minWaitTime, maxWaitTime);

        newAngle = Random.Range(0, 360);
        targetRotation = mad.transform.rotation * Quaternion.AngleAxis(newAngle, Vector3.up);
    }
}
