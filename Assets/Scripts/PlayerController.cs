using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Controller controller;
    [SerializeField] private Transform cameraTransform;
    
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float stickSensitivity;
    
    private Vector2 lookVector;
    private float Xrotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            controller.Move(new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y));
        }

        if (context.canceled)
        {
            controller.Move(Vector3.zero);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        controller.Jump();
    }
    
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            controller.isSprinting = true;
        }

        if (context.canceled)
        {
            controller.isSprinting = false;
        }
    }
    
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            controller.isCrouching = true;
        }

        if (context.canceled)
        {
            controller.isCrouching = true;
        }
    }
    
    public void OnMouseLook(InputAction.CallbackContext context)
    {
        Rotate(context.ReadValue<Vector2>() * mouseSensitivity);
    }
    
    public void OnStickLook(InputAction.CallbackContext context)
    {
        Rotate(context.ReadValue<Vector2>() * stickSensitivity);
    }

    private void Rotate(Vector2 context)
    {
        controller.Rotate(context.x);
        Xrotation -= context.y;
        Xrotation = Mathf.Clamp(Xrotation, -70, 70);
        if ((lookVector.y > 0 && cameraTransform.eulerAngles.x > -70) || (lookVector.y < 0 && cameraTransform.eulerAngles.x < 70))
        {
            //cameraTransform.Rotate(Vector3.right, lookVector.y);
        }
        cameraTransform.localEulerAngles = new Vector3(Xrotation, 0, 0);
    }
}
