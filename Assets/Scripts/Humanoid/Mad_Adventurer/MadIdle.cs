using UnityEngine;


[System.Serializable]
public class MadIdle : MadState
{
    [SerializeField, Tooltip("how far away in x it can move from its spawn"), Min(0f)] private float xWanderRange;
    [SerializeField, Tooltip("how far away in z it can move from its spawn"), Min(0f)] private float zWanderRange;
    [SerializeField, Tooltip("minimum wait time in seconds"), Min(0f)] private float minWaitTime;
    [SerializeField, Tooltip("maximum wait time in seconds"), Min(1f)] private float maxWaitTime;
    private Vector3 spawnPosition;
    private Vector3 position;
    
    private float waitTime;
    private bool walking;

    public override void OnValidate(MadAdventurer madAdventurer)
    {
        base.OnValidate(madAdventurer);
        spawnPosition = mad.transform.position;
    }

    public override void Enter()
    {
        base.Enter();
        target = FindPath(getRandomPosition());
    }

    public override void FixedUpdate()
    {
        if (walking)
        {
            if (Vector2.SqrMagnitude(new Vector2(target.x - mad.transform.position.x, target.z - mad.transform.position.z)) < 1f)
            {
                walking = false;
                waitTime = Random.Range(minWaitTime, maxWaitTime);
                Stop();
            }
            else
            {
                base.FixedUpdate();
            }
        }
        else
        {
            if (waitTime <= 0)
            {
                walking = true;
                target = FindPath(getRandomPosition());
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }

    private Vector3 FindPath(Vector3 pos)
    {
        if (mad.agent.CalculatePath(pos, path))
        {
            return path.corners[1];
        }
        return mad.transform.position;
    }

    private Vector3 getRandomPosition()
    {
        return spawnPosition + new Vector3(Random.Range(-xWanderRange, xWanderRange), 0f, Random.Range(-zWanderRange, zWanderRange));
    }
}
