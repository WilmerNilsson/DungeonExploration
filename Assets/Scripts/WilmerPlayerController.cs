using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WilmerPlayerController : MonoBehaviour
{
    public Transform cameraTransform;
    private Vector3 moveVector;
    private Vector2 lookVector;
    private Vector2 currentRotation;
    public float moveSpeed;
    public float sprintSpeed;
    public float crouchSpeed;
    private float currentSpeed;
    public float jumpForce = 5;
    public float mouseSensitivity;
    public float stickSensitivity;

    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {
        rb =  GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        currentRotation.y += lookVector.x;
        currentRotation.x -= lookVector.y;
        
        currentRotation.x = Mathf.Clamp(currentRotation.x, -70, 70);
        
        cameraTransform.eulerAngles = currentRotation;
        //moveVector = Vector3.Project(moveVector, new Vector3(cameraTransform.forward.x, 0 , cameraTransform.forward.z));
        Vector3 rotatedVector = Quaternion.AngleAxis(cameraTransform.eulerAngles.y, Vector3.up) * moveVector;
        transform.position += rotatedVector * Time.deltaTime * currentSpeed;
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
            rb.AddForce(Vector3.up * 5, ForceMode.VelocityChange);
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
