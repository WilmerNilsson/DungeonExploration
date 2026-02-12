using System;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(HumanoidController),typeof(HumanoidMovement),typeof(HumanoidRotator))]
[RequireComponent(typeof(HumanoidInteract),typeof(HumanoidAttackAnimatorCompanion),typeof(PlayerInput))]
[RequireComponent(typeof(PlayerUIController),typeof(OneShotPlayer))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private HumanoidController controller;
    [SerializeField, Tooltip("TODO add to HumanoidController instead")] private PlayerIK IK; //TODO add to HumanoidController instead
    
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float stickSensitivity = 5f;
    
    private Vector2 lookVector;
    private Vector2 lookInput;
    private Vector3 moveVector;

    private bool lockedMovement = false;

    private Vector2 mouseStart;
    private Vector2 mouseEnd;

    void Start()
    {
        //we may move this to game manager OnLoadScene
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameManagerSO.Instance.OnLockMouse += LockMovement;
    }

    private void Update()
    {
        if (!lockedMovement)
        {
            Rotate(lookInput);
        }
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
        
        lookInput = context.ReadValue<Vector2>() * mouseSensitivity;
    }
    
    public void OnStickLook(InputAction.CallbackContext context)
    {
        if (lockedMovement) return;

        lookInput = context.ReadValue<Vector2>() * stickSensitivity;
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
        if (context.performed && !lockedMovement)
        {
            mouseStart = Mouse.current.position.ReadValue();
            GameManagerSO.Instance.LockMouse(true);
        }
        if (context.canceled)
        {
            mouseEnd = Mouse.current.position.ReadValue();
            if (Vector2.Distance(mouseStart, mouseEnd) < 10)
            {
                GameManagerSO.Instance.LockMouse(false);
                return;
            }
            IK.Attack(Vector2.SignedAngle(Vector2.left, (mouseEnd - mouseStart)));
            GameManagerSO.Instance.LockMouse(false);
        }
    }

    public void OnBlock(InputAction.CallbackContext context)
    {
        if (context.performed && !lockedMovement)
        {
            mouseStart = Mouse.current.position.ReadValue();
            GameManagerSO.Instance.LockMouse(true);
        }
        if (context.canceled)
        {
            mouseEnd = Mouse.current.position.ReadValue();
            if (Vector2.Distance(mouseStart, mouseEnd) < 10)
            {
                GameManagerSO.Instance.LockMouse(false);
                return;
            }
            IK.Block(Vector2.SignedAngle(Vector2.right, (mouseEnd - mouseStart)));
            GameManagerSO.Instance.LockMouse(false);
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
