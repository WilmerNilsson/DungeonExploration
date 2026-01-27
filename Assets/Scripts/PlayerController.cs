using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    
    [SerializeField] float moveSpeed;
    
    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference sprintAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        sprintAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        sprintAction.action.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        float theta = -transform.rotation.eulerAngles.y * Mathf.PI / 180;
        float cos = Mathf.Cos(theta);
        float sin = Mathf.Sin(theta);
        
        Vector3 move = new Vector3(input.x * cos - input.y * sin, 0, input.x * sin + input.y * cos) * (input.magnitude * moveSpeed);

        if (sprintAction.action.ReadValue<float>() != 0) move *= 2;
        
        characterController.Move(move * Time.deltaTime);
    }
}
