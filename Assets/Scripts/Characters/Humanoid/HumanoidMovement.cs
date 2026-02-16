using System;
using UnityEngine;
using UnityEngine.Events;

public class HumanoidMovement : MonoBehaviour
{
    public UnityEvent<moveActions> OnMoveActionChange;
    
    [SerializeField] HumanoidController controller;
    [SerializeField] CharacterController CC;
    
    [Header("Stats")]
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float sprintSpeed = 2;
    [SerializeField] private float crouchSpeed = .5f;
    [SerializeField] private float airMoveMod = 0.1f;
    [SerializeField] private float jumpHeight = 5;
    private Vector3 moveVector;
    private Vector3 rotatedVector;
    private Vector3 playerVelocity;
    
    [Header("Debug")]
    [SerializeField] private bool doJump;
    [SerializeField] private bool grounded;
    
    private float currentSpeed;

    private moveActions currentAction = moveActions.None;

    private void OnDestroy()
    {
        OnMoveActionChange = null;
    }

    public enum moveActions
    {
        None,
        Walking,
        Sprinting,
        CrouchWalk,
        Airborne
    }

    private void Update()
    {
        grounded = CC.isGrounded;
        rotatedVector = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * moveVector;

        if (grounded && playerVelocity.y < -2f) playerVelocity.y = -2f; // stays to the ground

        if (doJump)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
            doJump = false;
        }

        
        if (moveVector == Vector3.zero)
        {
            SetMoveAction(moveActions.None);
        }
        else if (controller.isCrouching)
        {
            currentSpeed = crouchSpeed;
            SetMoveAction(moveActions.CrouchWalk);
        }
        else if (controller.isSprinting)
        {
            currentSpeed = sprintSpeed;
            SetMoveAction(moveActions.Sprinting);
        }
        else
        {
            currentSpeed = moveSpeed;
            SetMoveAction(moveActions.Walking);
        }

        if (!grounded) //TODO fix air movement, maybe save initial movement and edit it while in the air?
        {
            currentSpeed *= airMoveMod;
            SetMoveAction(moveActions.Airborne);
        }
        
        playerVelocity.y += Physics.gravity.y * Time.deltaTime;
        
        Vector3 finalMove = rotatedVector * currentSpeed + playerVelocity;
        CC.Move(finalMove * Time.deltaTime);
    }

    void SetMoveAction(moveActions newAction)
    {
        if(newAction != currentAction)
        {
            currentAction = newAction; 
            OnMoveActionChange?.Invoke(currentAction);
        }
    }

    public void Move(Vector3 direction)
    {
        moveVector = direction;
    }

    public void Jump()
    {
        doJump = CC.isGrounded;
    }
}
