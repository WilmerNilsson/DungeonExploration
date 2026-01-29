using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTest : MonoBehaviour
{
    [SerializeField] private bool agentMove;
    
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Controller controller;

    private NavMeshPath path;
    private bool hasTarget;
    private Transform target;
    
    private Vector3 currentTarget;
    private Vector3 realTarget;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        path = new NavMeshPath();
    }

    // Update is called once per frame
    void Update()
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
            target = other.transform;
            hasTarget = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasTarget = false;
            path.ClearCorners();
            controller.Move(Vector3.zero);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(currentTarget, 0.1f);
        Gizmos.DrawLine(transform.position, currentTarget);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
    }
}
