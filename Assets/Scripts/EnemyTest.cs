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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        path = new NavMeshPath();
    }

    // Update is called once per frame
    void Update()
    {
        if (path.corners.Length > 1)
        {
            currentTarget = path.corners[1]-transform.position;
        }
        if (currentTarget != oldTarget)
        {
            oldTarget = currentTarget;
            controller.Rotate(Vector2.Angle(new Vector2(transform.forward.x,transform.forward.z), new Vector2(currentTarget.x, currentTarget.z)));
            controller.Move(transform.forward);
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
        // Vector3[] nodes = agentMove ? agent.path.corners : path.corners;
        // Gizmos.color = Color.red;
        // for (int i = 0; i < nodes.Length-1; i++)
        // {
        //     Gizmos.DrawSphere(nodes[i+1], 0.1f);
        //     Gizmos.DrawLine(nodes[i], nodes[i+1]);
        // }
    }
}
