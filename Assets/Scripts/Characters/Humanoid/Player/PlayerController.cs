using System;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(HumanoidController),typeof(HumanoidMovement),typeof(HumanoidRotator))]
[RequireComponent(typeof(HumanoidInteract),typeof(PlayerInput))]
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

    [SerializeField]private bool lockedMovement = false;
    [SerializeField]private bool lockedCamera = false;
    
    [SerializeField]bool startedAttack = false;
    [SerializeField]bool startedBlock = false;

    private Vector2 mouseStart;
    private Vector2 mouseEnd;

    void Start()
    {
        //we may move this to game manager OnLoadScene
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameManagerSO.Instance.OnLockMouse += LockMovement;
        GameManagerSO.Instance.OnLockCamera += LockCamera;
        
        lookVector.y = transform.localEulerAngles.y;
    }
    
    private void LockMovement(bool newValue)
    {
        lockedMovement = newValue;
    }
    
    private void LockCamera(bool newValue)
    {
        lockedCamera = newValue;
    }

    private void Update()
    {
        if (!lockedCamera)
        {
            Rotate(lookInput);
        }
    }

    private void OnDestroy()
    {
        GameManagerSO.Instance.OnLockMouse -= LockMovement;
        GameManagerSO.Instance.OnLockCamera -= LockCamera;
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
        if (lockedMovement) return;
        if (context.performed) controller.Crouch(true);
        if (context.canceled) controller.Crouch(false);
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
        if (lockedMovement) return;
        if (context.performed && !lockedCamera)
        {
            mouseStart = Mouse.current.position.ReadValue();
            startedAttack = true;
            GameManagerSO.Instance.LockCamera(true);
        }
        if (context.canceled && startedAttack)
        {
            mouseEnd = Mouse.current.position.ReadValue();
            startedAttack = false;
            if (Vector2.Distance(mouseStart, mouseEnd) > 10)
            {
                IK.Attack(Vector2.SignedAngle(Vector2.left, (mouseEnd - mouseStart)));
            }
            GameManagerSO.Instance.LockCamera(false);
        }
    }

    public void OnBlock(InputAction.CallbackContext context)
    {
        if (lockedMovement) return;
        if (context.performed && !lockedCamera)
        {
            mouseStart = Mouse.current.position.ReadValue();
            startedBlock = true;
            GameManagerSO.Instance.LockCamera(true);
        }
        if (context.canceled && startedBlock)
        {
            mouseEnd = Mouse.current.position.ReadValue();
            startedBlock = false;
            if (Vector2.Distance(mouseStart, mouseEnd) > 10)
            {
                IK.Block(Vector2.SignedAngle(Vector2.down, (mouseEnd - mouseStart)));
            }
            GameManagerSO.Instance.LockCamera(false);
        }
    }

    private void Rotate(Vector2 context)
    {
        lookVector.x -= context.y;
        lookVector.y += context.x;
        
        lookVector.x = Mathf.Clamp(lookVector.x, -70f, 70f);
        
        controller.Rotate(Quaternion.AngleAxis(lookVector.y, Vector3.up) * Quaternion.AngleAxis(lookVector.x, Vector3.right));
    }
}
