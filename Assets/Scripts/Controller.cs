using UnityEngine;

public class Controller : MonoBehaviour
{
    [Header("Sub Scripts")]
    [SerializeField] private Movement movement;

    public bool isSprinting;
    public bool isCrouching;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Move(Vector3 direction)
    {
        direction.y = 0;
        movement.Move(direction.normalized);
    }

    public void Jump()
    {
        movement.Jump();
    }

    public void Rotate(float angle)
    {
        movement.Rotate(angle);
    }
}
