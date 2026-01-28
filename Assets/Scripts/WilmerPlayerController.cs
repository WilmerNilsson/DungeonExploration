using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WilmerPlayerController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GroundCheck groundCheck;
    private Vector3 moveVector;
    private Vector2 lookVector;
    private Vector2 currentRotation;
    
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float crouchSpeed;
    private float currentSpeed;
    [SerializeField] private float jumpForce = 5;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float stickSensitivity;

    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSpeed = moveSpeed;
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
        
        //Move stuff
        Vector3 rotatedVector = Quaternion.AngleAxis(cameraTransform.eulerAngles.y, Vector3.up) * moveVector;
        transform.position += Time.deltaTime * currentSpeed * rotatedVector;
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
        if (context.performed)
        {
            if (groundCheck.GetIsGrounded())
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            }
        }
    }
    
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            currentSpeed = sprintSpeed;
        }

        if (context.canceled)
        {
            currentSpeed = moveSpeed;
        }
    }
    
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            currentSpeed = crouchSpeed;
            transform.localScale = new Vector3(1, 0.5f, 1);
        }

        if (context.canceled)
        {
            currentSpeed = moveSpeed;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
