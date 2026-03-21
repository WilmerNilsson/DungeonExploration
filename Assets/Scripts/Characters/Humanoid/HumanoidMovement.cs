using System;
using UnityEngine;
using UnityEngine.Events;

public class HumanoidMovement : MonoBehaviour
{
    public UnityEvent<moveActions> OnMoveActionChange;
    public UnityEvent OnJump;
    public UnityEvent<float> OnLand;
    
    [SerializeField] HumanoidController controller;
    [SerializeField] CharacterController CC;
    [SerializeField] Animator animator;
    private Transform bodyTransform;
    
    [Header("Stats")]
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float sprintSpeed = 2;
    [SerializeField] private float crouchSpeed = .5f;
    [SerializeField] private float airMoveMod = 0.1f;
    [SerializeField] private float jumpHeight = 5;
    private Vector3 moveVector;
    private Vector3 rotatedVector;
    private Vector3 initialAirVector;
    private float initialMagnitude;
    public Vector3 lastGroundedPosition;
    private Vector3 playerVelocity;
    
    [Header("Debug")]
    [SerializeField] private bool doJump;
    [SerializeField] private bool grounded;
    
    private bool supressMoveFrame = false;

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

    private void Start()
    {
        bodyTransform = animator.gameObject.transform;
        lastGroundedPosition = bodyTransform.position;
    }

    private void FixedUpdate()
    {
        grounded = CC.isGrounded;
        rotatedVector = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * moveVector;
        
        if(supressMoveFrame)
        {
            supressMoveFrame = false;
            return;
        }

        if (grounded && playerVelocity.y < -2f) playerVelocity.y = -2f; // stays to the ground

        if (doJump)
        {
            OnJump.Invoke();
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
            doJump = false;
        }

        float deltaSpeed = moveSpeed;

        Vector3 finalMove = Vector3.zero;
        if (!grounded)
        {
            deltaSpeed = (controller.isSprinting && Vector3.Dot(transform.forward, rotatedVector) >= 0) ? sprintSpeed : moveSpeed;
            
            float speedX = playerVelocity.x + rotatedVector.x * airMoveMod * Time.fixedDeltaTime;
            float speedZ = playerVelocity.z + rotatedVector.z * airMoveMod * Time.fixedDeltaTime;
            Vector3 newVelocity = new Vector3(speedX, 0, speedZ);

            if (!currentAction.Equals(moveActions.Airborne))
            {
                SetMoveAction(moveActions.Airborne);
            }

            rotatedVector = Vector3.ClampMagnitude(newVelocity, deltaSpeed);
            
            playerVelocity = new Vector3(0, playerVelocity.y + (Physics.gravity.y * Time.fixedDeltaTime), 0);
            //playerVelocity.y += Physics.gravity.y * Time.fixedDeltaTime;
    
            finalMove = rotatedVector + playerVelocity;
            playerVelocity = finalMove;
            CC.Move(finalMove * Time.fixedDeltaTime);
            return;
        }
        else if (moveVector == Vector3.zero)
        {
            SetMoveAction(moveActions.None);
        }
        else if (controller.isCrouching)
        {
            deltaSpeed = crouchSpeed;
            SetMoveAction(moveActions.CrouchWalk);
        }
        else if (controller.isSprinting && Vector3.Dot(transform.forward, rotatedVector) >= 0)
        {
            deltaSpeed = sprintSpeed;
            SetMoveAction(moveActions.Sprinting);
        }
        else
        {
            deltaSpeed = moveSpeed;
            SetMoveAction(moveActions.Walking);
        }

        playerVelocity = new Vector3(0, playerVelocity.y + (Physics.gravity.y * Time.fixedDeltaTime), 0);
        //playerVelocity.y += Physics.gravity.y * Time.fixedDeltaTime;
    
        finalMove = rotatedVector * deltaSpeed + playerVelocity;
        playerVelocity = finalMove;
        lastGroundedPosition = transform.position;

        CC.Move(finalMove * Time.fixedDeltaTime);
    }

    /// <summary>
    /// makes the character controller not move next fixed update
    /// </summary>
    public void SupressMoveFrame()
    {
        supressMoveFrame = true;
    }

    void SetMoveAction(moveActions newAction)
    {
        if(newAction != currentAction)
        {
            if (newAction == moveActions.Airborne)
            {
                initialAirVector = rotatedVector;
                initialMagnitude = initialAirVector.magnitude;
            }
            else if (currentAction == moveActions.Airborne)
            {
                OnLand.Invoke(lastGroundedPosition.y - transform.position.y);
            }
            if (newAction == moveActions.Sprinting)
            {
                animator.SetBool("Running", true);
            }
            else
            {
                animator.SetBool("Running", false);
            }
            currentAction = newAction; 
            OnMoveActionChange?.Invoke(currentAction);
        }
    }

    public void Move(Vector3 direction)
    {
        moveVector = direction;
        animator.SetBool("Moving", direction != Vector3.zero);
    }

    public void Jump()
    {
        doJump = CC.isGrounded;
    }

    public void Crouch(bool isCrouching)
    {
        if (isCrouching && grounded)
        {
            CC.height = 2f;
            CC.center = new Vector3(0, 2f, 0);
        }
        else if (!Physics.Raycast(transform.position, Vector3.up, 0.1f, LayerMask.GetMask("Ground", "Walls", "Roof")))
        {
            CC.center = new Vector3(0, 1.5f, 0);
            CC.height = 3f;
            if (Physics.Raycast(transform.position + CC.center, Vector3.down, 1f, LayerMask.GetMask("Ground", "Walls", "Roof")))
            {
                CC.Move(new Vector3(0, 1, 0));
            }
        }
    }
}
