using UnityEngine;


[RequireComponent(typeof(HumanoidAttackAnimatorCompanion),typeof(HumanoidMovement),typeof(HumanoidRotator))]
[RequireComponent(typeof(HumanoidInteract))]
public class HumanoidController : MonoBehaviour
{
    [Header("Sub Scripts")]
    [SerializeField] private HumanoidMovement movement;
    [SerializeField] private HumanoidRotator rotator;
    [SerializeField] private HumanoidInteract interact;
    [SerializeField] private HumanoidAttackAnimatorCompanion animateAttack;

    public bool isSprinting;
    public bool isCrouching;
    
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

    /// <summary>
    /// should be used by player
    /// </summary>
    public void PrepareAttackUpdate(float angle)
    {
        animateAttack.HoldAttackUpdate(angle);
    }

    /// <summary>
    /// Prepares the attack animation by moving to the start position
    /// </summary>
    public void PrepareAttack(bool start)
    {
        animateAttack.HoldAttack(start);
    }

    /// <summary>
    /// Starts the Attack
    /// </summary>
    public void Attack(float angle)
    {
        animateAttack.Attack(angle);
    }
    
    /// <summary>
    /// should be used by player
    /// </summary>
    public void HoldBlock(bool start)
    {
        animateAttack.HoldBlock(start);
    }

    /// <summary>
    /// should be used by player
    /// </summary>
    public void HoldBlockUpdate(float angle)
    {
        animateAttack.HoldBlockUpdate(angle);
    }
}
