using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private HumanoidController controller;
    
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float stickSensitivity;
    
    private Vector2 lookVector;
    private Vector3 moveVector;

    private bool lockedMovement = false;

    void Start()
    {
        //we may move this to game manager OnLoadScene
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameManagerSO.Instance.OnLockMouse += LockMovement;
    }

    private void OnDestroy()
    {
        GameManagerSO.Instance.OnLockMouse -= LockMovement;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.canceled || lockedMovement)
        {
            moveVector = Vector3.zero;
        }
        else if (context.performed)
        {
            moveVector = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
        }

        controller.Move(moveVector);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (lockedMovement) return;

        if (context.performed) controller.Jump();
    }
    
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.canceled || lockedMovement)
        {
            controller.isSprinting = false;
        }
        else if (context.performed)
        {
            controller.isSprinting = true;
        }
    }
    
    public void OnCrouch(InputAction.CallbackContext context)
    {

        //we may want the playr to crouch while looting;
        if (context.canceled || lockedMovement)
        {
            controller.isCrouching = false;
        }
        else if (context.performed)
        {
            controller.isCrouching = true;
        }

        
    }
    
    public void OnMouseLook(InputAction.CallbackContext context)
    {
        if (lockedMovement) return;

        Rotate(context.ReadValue<Vector2>() * mouseSensitivity);
    }
    
    public void OnStickLook(InputAction.CallbackContext context)
    {
        if (lockedMovement) return;

        Rotate(context.ReadValue<Vector2>() * stickSensitivity);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (lockedMovement) return;

        if (context.performed)
        {
            controller.Interact();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (lockedMovement) return;

        if (context.performed)
        {
            controller.Attack();
        }
    }

    private void Rotate(Vector2 context)
    {
        lookVector.x -= context.y;
        lookVector.y += context.x;
        
        lookVector.x = Mathf.Clamp(lookVector.x, -70f, 70f);
        
        controller.Rotate(Quaternion.AngleAxis(lookVector.y, Vector3.up) * Quaternion.AngleAxis(lookVector.x, Vector3.right));
    }

    private void LockMovement(bool newValue)
    {
        lockedMovement = newValue;

        //controller.ResetMovement();
    }
}
