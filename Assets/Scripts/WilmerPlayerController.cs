using UnityEngine;
using UnityEngine.InputSystem;

public class WilmerPlayerController : MonoBehaviour
{
    public Vector3 moveVector;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveVector * Time.deltaTime;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveVector = context.ReadValue<Vector2>();
        }

        if (context.canceled)
        {
            moveVector = Vector3.zero;
        }
    }

    public void OnLook()
    {
        
    }
}
