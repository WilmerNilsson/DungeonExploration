using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] Controller controller;
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
            rb.linearVelocity = new Vector3(Mathf.Clamp(rb.linearVelocity.x + rotatedVector.x * airMoveSpeed, -currentSpeed, currentSpeed),
                rb.linearVelocity.y, Mathf.Clamp(rb.linearVelocity.z + rotatedVector.z * airMoveSpeed, -currentSpeed, currentSpeed));
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
    
    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position - Vector3.up * 0.6f, .5f, LayerMask.GetMask("Ground"));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position - Vector3.up * 0.6f, .5f);
    }
}
