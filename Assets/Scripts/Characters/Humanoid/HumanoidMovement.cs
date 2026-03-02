using System;
using UnityEngine;
using UnityEngine.Events;

public class HumanoidMovement : MonoBehaviour
{
    public UnityEvent<moveActions> OnMoveActionChange;
    public UnityEvent OnJump;
    
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

        if (!grounded) //TODO fix air movement, maybe save initial movement and edit it while in the air?
        {
            deltaSpeed *= airMoveMod;
            SetMoveAction(moveActions.Airborne);
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
        
        playerVelocity.y += Physics.gravity.y * Time.fixedDeltaTime;
        
        Vector3 finalMove = rotatedVector * deltaSpeed + playerVelocity;
        CC.Move(finalMove * Time.fixedDeltaTime);
    }

    /// <summary>
    /// makes the charachter controller not move next fixed update
    /// </summary>
    public void SupressMoveFrame()
    {
        supressMoveFrame = true;
    }

    void SetMoveAction(moveActions newAction)
    {
        if(newAction != currentAction)
        {
            if (newAction == moveActions.Sprinting)
            {
                animator.SetFloat("RunSpeed", 2);
            }
            else
            {
                animator.SetFloat("RunSpeed", 1);
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
        Vector3 heightMod = new Vector3(0, 0.5f, 0);
        if (isCrouching && grounded)
        {
            GetComponent<CapsuleCollider>().height = 1.5f;
            GetComponent<CharacterController>().height = 1.5f;
            //bodyTransform.position -= heightMod;
        }
        else if (!Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, 0.1f))
        {
            //bodyTransform.position += heightMod;
            GetComponent<CapsuleCollider>().height = 3;
            GetComponent<CharacterController>().height = 3;
        }
    }
}
