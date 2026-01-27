using UnityEngine;

public class Camera : MonoBehaviour
{
    private float mouseX, mouseY;
    [SerializeField] private float mouseSensitivity;
    
    [SerializeField] private Vector3 currentRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        currentRotation.y += mouseX;
        currentRotation.x -= mouseY;
        
        currentRotation.x = Mathf.Clamp(currentRotation.x, -70, 70);
        
        transform.eulerAngles = currentRotation;
    }
}
