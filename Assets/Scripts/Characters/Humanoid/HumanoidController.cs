using UnityEngine;

public class HumanoidController : MonoBehaviour
{
    [Header("Sub Scripts")]
    [SerializeField] private HumanoidMovement movement;
    [SerializeField] private HumanoidRotator rotator;
    [SerializeField] private HumanoidInteract interact;
    [SerializeField] private HumanoidAttackAnimatorCompanion attacking;

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

    public void Crouch(bool isCrouching)
    {
        this.isCrouching = isCrouching;
        movement.Crouch(isCrouching);
    }

    public void Rotate(Quaternion rotationQuaternion)
    {
        rotator.Rotate(rotationQuaternion);
    }
    
    public void Interact()
    {
        interact.Interact();
    }

    public void Attack()
    {
        attacking.Attack();
    }
}
