using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Controller controller;
    
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float stickSensitivity;
    
    private Vector2 lookVector;
    private Vector3 moveVector;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
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
        controller.Move(moveVector);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) controller.Jump();
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

    public void OnInteract(InputAction.CallbackContext context)
    {
        controller.Interact();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        controller.Attack();
    }

    private void Rotate(Vector2 context)
    {
        lookVector.x -= context.y;
        lookVector.y += context.x;
        
        lookVector.x = Mathf.Clamp(lookVector.x, -70f, 70f);
        
        controller.Rotate(Quaternion.AngleAxis(lookVector.y, Vector3.up) * Quaternion.AngleAxis(lookVector.x, Vector3.right));
    }
}
