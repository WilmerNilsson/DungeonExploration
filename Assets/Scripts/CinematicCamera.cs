using UnityEngine;
using UnityEngine.InputSystem;

public class CinematicCamera : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float rotateSpeed = 1.0f;
    private bool goUp = false;
    private bool goDown = false;
    private Vector2 moveVector = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;
    private float mouseSensitivity = 1.0f;
    private Vector2 lookVector = Vector2.zero;

    private Vector2 rotationVector;
    private Vector3 rotatedVector;

    private Quaternion targetHeadQuaternion;
    private Quaternion targetBodyQuaternion;
    public Transform bodyTransform;
    public Transform headTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Rotate(lookInput * rotateSpeed);
        bodyTransform.rotation = targetBodyQuaternion;
        headTransform.eulerAngles = new Vector3(rotationVector.x, bodyTransform.eulerAngles.y, 0);
        rotatedVector = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * moveVector;
        Vector3 forward = headTransform.forward * moveVector.y;
        Vector3 sideways = headTransform.right * moveVector.x;
        rotatedVector = forward + sideways;
        transform.position += rotatedVector * (moveSpeed * Time.deltaTime);
        if (goUp)
        {
            transform.position += Vector3.up * (moveSpeed * Time.deltaTime);
        }

        if (goDown)
        {
            transform.position += Vector3.down * (moveSpeed * Time.deltaTime);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveVector = new Vector3(context.ReadValue<Vector2>().x, context.ReadValue<Vector2>().y);
    }

    public void onUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            goUp = true;
        }
        else
        {
            goUp = false;
        }
    }
    
    public void onDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            goDown = true;
        }
        else
        {
            goDown = false;
        }
    }
    
    public void OnMouseLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>() * mouseSensitivity;
    }
    
    private void Rotate(Vector2 context)
    {
        lookVector.x -= context.y;
        lookVector.y += context.x;
        
        lookVector.x = Mathf.Clamp(lookVector.x, -70f, 70f);
        
        Rotate2(Quaternion.AngleAxis(lookVector.y, Vector3.up) * Quaternion.AngleAxis(lookVector.x, Vector3.right));
    }
    
    public void Rotate2(Quaternion rotationQuaternion)
    {
        rotationVector = rotationQuaternion.eulerAngles;

        targetHeadQuaternion = rotationQuaternion;
        targetBodyQuaternion = Quaternion.AngleAxis(rotationVector.y, Vector3.up);
    }
}
