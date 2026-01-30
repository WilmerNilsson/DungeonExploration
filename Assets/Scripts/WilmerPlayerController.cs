using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WilmerPlayerController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GroundCheck groundCheck;
    private Vector3 moveVector; //The direction of the players move input, translated from x,y to x,z
    private Vector3 rotatedVector; //The move vector translated to the cameras rotation
    private Vector2 lookVector; // 
    private Vector2 currentRotation;
    
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float airMoveSpeed = 0.1f;
    private float currentSpeed;
    [SerializeField] private float jumpForce = 5;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float stickSensitivity;
    enum PlayerStates
    {
        Normal,
        Crouch,
        Jump
    }

    private bool isSprinting;
    private bool isCrouching;
    PlayerStates playerState = PlayerStates.Normal;

    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSpeed = moveSpeed;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Awake()
    {
        rb =  GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Camera stuff
        currentRotation.y += lookVector.x;
        currentRotation.x -= lookVector.y;

        currentRotation.x = Mathf.Clamp(currentRotation.x, -70, 70);

        cameraTransform.eulerAngles = currentRotation;
        transform.rotation = Quaternion.Euler(transform.rotation.x, cameraTransform.eulerAngles.y, transform.rotation.z);

        //move stuff
        rotatedVector = Quaternion.AngleAxis(cameraTransform.eulerAngles.y, Vector3.up) * moveVector;
        
        if (!groundCheck.GetIsGrounded()) //In air
        {
            //transform.position += Time.deltaTime * currentSpeed * rotatedVector;
            rb.linearVelocity = new Vector3(Mathf.Clamp((rb.linearVelocity.x + rotatedVector.x) * airMoveSpeed, -currentSpeed, currentSpeed),
            rb.linearVelocity.y, Mathf.Clamp((rb.linearVelocity.z + rotatedVector.z) * airMoveSpeed, -currentSpeed, currentSpeed));
        }
        else
        {
            if (isCrouching) //crouching
            {
                currentSpeed = crouchSpeed;

            }
            else if (isSprinting && moveVector.z > 0) //sprinting forwards
            {
                currentSpeed = sprintSpeed;
            }
            else
            {
                currentSpeed = moveSpeed;
            }
            //transform.position += Time.deltaTime * currentSpeed * rotatedVector;
            rb.linearVelocity = new Vector3(currentSpeed * rotatedVector.x, rb.linearVelocity.y,
                currentSpeed * rotatedVector.z);
        }
        //Debug.Log(rb.linearVelocity);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveVector = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
        }

        if (context.canceled)
        {
            moveVector = Vector3.zero;
        }
    }

    public void OnMouseLook(InputAction.CallbackContext context)
    {
        lookVector = context.ReadValue<Vector2>() * mouseSensitivity;
    }
    
    public void OnStickLook(InputAction.CallbackContext context)
    {
        lookVector = context.ReadValue<Vector2>() * stickSensitivity;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && groundCheck.GetIsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }
    
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isSprinting = true;
        }

        if (context.canceled)
        {
            isSprinting = false;
        }
    }
    
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed && playerState != PlayerStates.Jump)
        {
            //currentSpeed = crouchSpeed;
            playerState = PlayerStates.Crouch;
            isCrouching = true;
            transform.localScale = new Vector3(1, 0.5f, 1);
        }

        if (context.canceled)
        {
            //currentSpeed = moveSpeed;
            isCrouching = false;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && GetComponentInChildren<AttackPlayer>())
        {
            GetComponentInChildren<AttackPlayer>().Attack();
        }
    }
}
