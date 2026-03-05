using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class HumanoidAttackAnimatorCompanion : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onDealDamage;
    public UnityEvent onGetBlocked;
    public UnityEvent<AttackState> onAttackStateChange;
    public UnityEvent<BlockState> onBlockStateChange;
    
    [SerializeField] protected AttackState attackState = AttackState.Charge;
    [SerializeField] protected BlockState blockState = BlockState.Charge;
    
    [SerializeField] private bool hasWeapon = false;
    [SerializeField] private bool attacking = false;
    [SerializeField] private bool blocking = false;
    [SerializeField] private GameObject weapon;
    [SerializeField] private TwoBoneIKConstraint swordArm; 
    [SerializeField] private Transform core;
    [SerializeField] private Transform head;
    [SerializeField] private Transform hand;
    
    private float startTime;
    private float time;
    private float cutoffTime; // to track how far into attack it has to reverse
    private float returnTime; // What time of the previous swing to return from
    private Weapon weaponScript;
    
    public enum AttackState
    {
        Charge,
        Hold,
        Swing,
        Return,
        Recoil
    }

    public enum BlockState
    {
        Charge,
        Block,
        Return
    }

    private void Start()
    {
        if (weaponScript == null)
        {
            hasWeapon = weapon.TryGetComponent(out weaponScript);
        }
    }

    public void Attack(float angle)
    {
        if (!attacking && !blocking)
        {
            ChangeAttackState(AttackState.Charge);
            attacking = true;
            weaponScript.angle = angle;
        }
    }
    
    public void Block(float angle)
    {
        if (!attacking && !blocking)
        {
            ChangeBlockState(BlockState.Charge);
            blocking = true;
            weaponScript.angle = angle;
            Vector3 anglePos = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), -Mathf.Cos(angle * Mathf.Deg2Rad), 0);
            Vector3 offsetPos = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
            if (angle > 0)
            {
                offsetPos = -offsetPos;
            }
            weaponScript.BlockPos = anglePos;
            weaponScript.BlockOffset = offsetPos;
        }
    }

    public void ChangeAttackState(AttackState newState)
    {
        if (attackState == AttackState.Recoil) returnTime = 0;
        if (attackState == AttackState.Swing) returnTime = 1;
        else
        {
            weaponScript.SetDamageActive(false);
        }
        
        
        attackState = newState;
        onAttackStateChange.Invoke(attackState);
        startTime = Time.time;
    }
    
    public void ChangeBlockState(BlockState newState)
    {
        blockState = newState;
        weaponScript.SetBlockActive(blockState == BlockState.Block);
        onBlockStateChange.Invoke(blockState);
        startTime = Time.time;
    }

    public void OnGetBlocked()
    {
        onGetBlocked.Invoke();
        cutoffTime = time;
        ChangeAttackState(AttackState.Recoil);
    }

    private void Update()
    {
        if (hasWeapon)
        {
            time = (Time.time - startTime);
            if (attacking)
            {
                switch (attackState)
                {
                    case AttackState.Charge:
                        if (weaponScript.ChargeAttack(time))
                        {
                            ChangeAttackState(AttackState.Hold);
                        }
                        break;
                    case AttackState.Hold:
                        if (weaponScript.HoldAttack(time))
                        {
                            ChangeAttackState(AttackState.Swing);
                        }
                        break;
                    case AttackState.Swing:
                        if (weaponScript.Swing(time))
                        {
                            ChangeAttackState(AttackState.Return);
                        }
                        break;
                    case AttackState.Recoil:
                        if (weaponScript.RecoilAttack(time,cutoffTime))
                        {
                            ChangeAttackState(AttackState.Return);
                        }
                        break;
                    case AttackState.Return:
                        if (weaponScript.ReturnAttack(time, returnTime))
                        {
                            attacking = false;
                        }
                        break;
                    
                }
            }
            else if (blocking)
            {
                switch (blockState)
                {
                    case BlockState.Charge:
                        if (weaponScript.ChargeBlock(time))
                        {
                            ChangeBlockState(BlockState.Block);
                        }
                        break;
                    case BlockState.Block:
                        if (weaponScript.Block(time))
                        {
                            ChangeBlockState(BlockState.Return);
                        }
                        break;
                    case BlockState.Return:
                        if (weaponScript.ReturnBlock(time))
                        {
                            blocking = false;
                        }
                        break;
                }
            }
            else
            {
                swordArm.data.targetPositionWeight = 0;
                swordArm.data.targetRotationWeight = 0;
            }
        }
    }

    public void Equip(GameObject newWeapon)
    {
        Destroy(weapon);
        weapon = Instantiate(newWeapon, hand);
        if (TryGetComponent(out Weapon script))
        {
            Debug.Log("No weapon script found on " + newWeapon);
        }
        script.companion = this;
        script.swordArm = swordArm;
        script.head = head;
        script.core = core;
    }

    public void Activate()
    {
        weapon.GetComponent<Collider>().enabled = true;
    }

    public void Deactivate()
    {
        weapon.GetComponent<Collider>().enabled = false;
    }
    
    
}
