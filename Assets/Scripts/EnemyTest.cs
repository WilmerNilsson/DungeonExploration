using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTest : MonoBehaviour
{
    [SerializeField] private bool agentMove;
    
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Controller controller;

    private NavMeshPath path;
    
    private Vector3 currentTarget;
    private Vector3 oldTarget;
    private Vector3 realTarget;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        path = new NavMeshPath();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTarget != oldTarget)
        {
            oldTarget = currentTarget;
            realTarget = currentTarget - transform.position;
            controller.Rotate(Quaternion.LookRotation(realTarget));
            //controller.Move(transform.forward);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (agentMove)
            {
                agent.destination = other.transform.position;
            }
            else
            {
                agent.CalculatePath(other.transform.position, path);
                currentTarget = path.corners[1];
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (agentMove)
            {
                agent.ResetPath();
            }
            else
            {
                path.ClearCorners();
            }
            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(currentTarget, 0.1f);
        Gizmos.DrawLine(transform.position, currentTarget);
    }
}
