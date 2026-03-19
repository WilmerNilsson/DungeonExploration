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
    /// Updates the start position of the attack
    /// </summary>
    public void HoldAttackUpdate(float angle)
    {
        animateAttack.HoldAttackUpdate(angle);
    }

    /// <summary>
    /// Starts the Attack
    /// </summary>
    public bool TryAttack(float angle)
    {
        return animateAttack.TryAttack(angle);
    }
    
    /// <summary>
    /// Toggles Blocking
    /// </summary>
    public void HoldBlock(bool start)
    {
        animateAttack.HoldBlock(start);
    }

    /// <summary>
    /// Updates the position of the Block
    /// </summary>
    public void HoldBlockUpdate(float angle)
    {
        animateAttack.HoldBlockUpdate(angle);
    }

    /// <summary>
    /// Attempts to parry while blocking
    /// </summary>
    public bool TryParry()
    {
        return animateAttack.TryParry();
    }
}
