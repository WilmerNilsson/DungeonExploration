using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MadIdle : MadState
{
    [Header("Player detection")]
    [SerializeField] private float sightRange;
    [SerializeField] private float soundRange;
    [SerializeField] private float sightLimit;
    [SerializeField] private float soundLimit;
    
    [Header("behaviour Variables")]
    [SerializeField, Tooltip("how far away it can move from its spawn"), Min(0f)] private float wanderRange = 5;
    [SerializeField, Tooltip("how quickly it turns"), Min(0f)] private float turnSize = 5;
    [SerializeField, Tooltip("how close the obstacle has to be"), Min(0f)] private float dodgeRange = 5;
    [SerializeField, Tooltip("how many Rays are made for dodging walls, how many side rays"), Min(1f)] private int dodgeRays = 1;
    [SerializeField, Tooltip("how wide of an area the rays cover"), Min(1f)] private int rayAngle = 5;
    [SerializeField] private LayerMask dodgeMask;
    
    [Header("Weights"), Tooltip("the modifier to the behaviours, higher means more impact")]
    [SerializeField] private float wanderWeight = 1;
    [SerializeField] private float dodgeWeight = 1;
    [SerializeField] private float containWeight = 1;
    
    [Header("Wait Times")]
    [SerializeField, Tooltip("minimum wait time in seconds"), Min(0f)] private float minWaitTime = 0;
    [SerializeField, Tooltip("maximum wait time in seconds"), Min(1f)] private float maxWaitTime = 1;
    [SerializeField, Tooltip("minimum wait time in seconds"), Min(0f)] private float minWalkTime = 0;
    [SerializeField, Tooltip("maximum wait time in seconds"), Min(1f)] private float maxWalkTime = 1;
    private Vector3 spawnPosition;
    private Vector3 position;
    private float wanderAngle;
    
    private Vector2 targetPosition;
    
    private float waitTime;
    private float walkTime;

    private bool headHome = false;
    
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
        if (Vector3.Distance(mad.transform.position, spawnPosition) > wanderRange)
        {
            headHome = true;
            FindPath(getRandomPosition());
        }
        CombatChecker.RemoveFromChaseList(mad.gameObject);
    }

    public override void Exit()
    {
        CombatChecker.AddToChaseList(mad.gameObject);
    }

    public override void Update()
    {
        if(lookForPlayer()) mad.Transit(mad.chasingState);
        
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

    private bool LookForPlayer()
    {
        float sight = mad.vision.SightDetection(sightRange);
        float sound = mad.vision.SoundDetection(soundRange);
        
        return sight > sightLimit || sound > soundLimit;
    }

    protected override void Move()
    {
        if (headHome)
        {
            // walk along path
        }
        else
        {
            targetPosition = CombineVector();
            Debug.DrawRay(mad.transform.position, new Vector3(targetPosition.x,0,targetPosition.y)*3, Color.green);
            mad.controller.Rotate(Quaternion.LookRotation(new Vector3(targetPosition.x,0,targetPosition.y)));
            mad.controller.Move(Vector3.forward);
        }
    }

    private bool FindPath(Vector3 pos)
    {
        return mad.agent.CalculatePath(pos, path);
    }

    private Vector3 getRandomPosition()
    {
        return spawnPosition + new Vector3(Random.Range(-wanderRange, wanderRange), 0f, Random.Range(-wanderRange, wanderRange)).normalized * wanderRange;
    }

    private Vector2 CombineVector()
    {
        Vector2 direction = Vector2.zero;
        Vector2 wander = Wander() * wanderWeight;
        Vector2 contain = Contain() * containWeight;
        direction += wander; // towards wander
        direction += contain; // towards contain
        foreach (var dodge in Dodge())
        {
            direction -= dodge * dodgeWeight; // away from dodge
        }
        
        return direction.normalized;
    }

    private Vector2 Wander()
    {
        Vector2 newDirection = new Vector2(Mathf.Cos(wanderAngle), Mathf.Sin(wanderAngle));
        
        return newDirection;
    }

    private List<Vector2> Dodge()
    {
        List<Vector2> list = new List<Vector2>();
        RaycastHit2D hit;
        float angle = 0;
        float currentAngle = mad.transform.eulerAngles.y;
        
        for (int i = -dodgeRays; i <= dodgeRays; i++)
        {
            angle = ((i * rayAngle) + currentAngle) * Mathf.Deg2Rad;
            Vector3 newDirection = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
            Debug.DrawRay(mad.transform.position, newDirection * dodgeRange, Color.red);
            hit = Physics2D.Raycast(mad.transform.position, newDirection, dodgeRange, dodgeMask);
            if (hit.collider != null)
            {
                list.Add(hit.point);
            }
        }
        
        return list;
    }

    private Vector2 Contain()
    {
        Vector2 contain = Vector2.zero;
        float distance = Vector3.Distance(mad.transform.position, spawnPosition);
        if (distance > wanderRange)
        {
            contain = new Vector2(spawnPosition.x - mad.transform.position.x, spawnPosition.z - mad.transform.position.z).normalized * (distance-wanderRange);
        }
        return contain;
    }

    private void Reset()
    {
        walkTime = Random.Range(minWalkTime, maxWalkTime);
        waitTime = Random.Range(minWaitTime, maxWaitTime);
        
        wanderAngle = (mad.transform.eulerAngles.y + Random.Range(-turnSize, turnSize)) * Mathf.Deg2Rad;
    }
}
