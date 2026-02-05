using System;
using UnityEngine;

public class HumanoidMovement : MonoBehaviour
{
    [SerializeField] HumanoidController controller;
    [SerializeField] Rigidbody rb;
    
    [Header("Stats")]
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float sprintSpeed = 2;
    [SerializeField] private float crouchSpeed = .5f;
    [SerializeField] private float airMoveSpeed = 0.1f;
    [SerializeField] private float jumpForce = 5;
    private Vector3 moveVector;
    private Vector3 rotatedVector;
    
    private float currentSpeed;

    [Header("Stair raycast positions")]
    [SerializeField] private Transform upperRaycast;
    [SerializeField] private Transform lowerRaycast;
    [SerializeField] private float raycastDistance;
    [SerializeField] private float raycastAngle;
    [SerializeField] private float stepSmooth;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotatedVector = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * moveVector;
        
        if (!IsGrounded()) //In air
        {
            float speedX = rb.linearVelocity.x + rotatedVector.x * airMoveSpeed;
            float speedZ = rb.linearVelocity.z + rotatedVector.z * airMoveSpeed;
            Vector3 newVelocity = new Vector3(speedX, 0, speedZ);
            if (newVelocity.magnitude > currentSpeed)
            {
                newVelocity = newVelocity.normalized * currentSpeed;
            }
            rb.linearVelocity = new Vector3(newVelocity.x, rb.linearVelocity.y, newVelocity.z);
        }
        else
        {
            if (controller.isCrouching) //crouching
            {
                currentSpeed = crouchSpeed;
            }
            else if (controller.isSprinting && moveVector.z > 0) //sprinting forwards
            {
                currentSpeed = sprintSpeed;
            }
            else
            {
                currentSpeed = moveSpeed;
            }
            ClimbStair();
            rb.linearVelocity = new Vector3(currentSpeed * rotatedVector.x, rb.linearVelocity.y,
                currentSpeed * rotatedVector.z);
        }
    }

    public void Move(Vector3 direction)
    {
        moveVector = direction;
    }

    public void Jump()
    {
        if (IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    private void ClimbStair()
    {
        Vector3 forward = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * rotatedVector.normalized;
        Vector3 minusAngle = Quaternion.AngleAxis(transform.eulerAngles.y - raycastAngle, Vector3.up) * rotatedVector.normalized;
        Vector3 plusAngle = Quaternion.AngleAxis(transform.eulerAngles.y + raycastAngle, Vector3.up) * rotatedVector.normalized;
        
        RaycastHit lowerHit;
        RaycastHit upperHit;
        Debug.DrawRay(lowerRaycast.position, rotatedVector.normalized * raycastDistance, Color.red);
        Debug.DrawRay(upperRaycast.position, rotatedVector.normalized * raycastDistance, Color.green);
        if (Physics.Raycast(lowerRaycast.transform.position, forward, out lowerHit, raycastDistance))
        {
            if (!Physics.Raycast(upperRaycast.transform.position, forward, out upperHit, raycastDistance))
            {
                rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
        else if (Physics.Raycast(lowerRaycast.transform.position, minusAngle, out lowerHit, raycastDistance))
        {
            if (!Physics.Raycast(upperRaycast.transform.position, minusAngle, out upperHit, raycastDistance))
            {
                rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
        else if (Physics.Raycast(lowerRaycast.transform.position, plusAngle, out lowerHit, raycastDistance))
        {
            if (!Physics.Raycast(upperRaycast.transform.position, plusAngle, out upperHit, raycastDistance))
            {
                rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
    }
    
    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position - Vector3.up * 0.6f, .5f, LayerMask.GetMask("Ground"));
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(transform.position - Vector3.up * 0.6f, .5f);
        
        Gizmos.DrawLine(lowerRaycast.transform.position, lowerRaycast.transform.position + Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * rotatedVector.normalized * raycastDistance);
        Gizmos.DrawLine(upperRaycast.transform.position, upperRaycast.transform.position + Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * rotatedVector.normalized * raycastDistance); 
    }
}
