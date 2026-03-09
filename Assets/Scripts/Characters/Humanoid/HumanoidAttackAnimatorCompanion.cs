using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
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
    private float TimeFromStartOfAnimation
    {
        get { return Time.time - startTime; }
    }
    private float cutoffTime; // to track how far into attack it has to reverse
    private float returnTime; // What time of the previous swing to return from
    private Weapon weaponScript;

#nullable enable

    private bool isInAnimation;
    private Coroutine? currentAnimation;

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
        if (weapon == null)
        {
            Debug.Log("no weapon has been assigned to " + this);
        }
        else if (weaponScript == null)
        {
            hasWeapon = weapon.TryGetComponent(out weaponScript);
            weaponScript.Companion = this;
            weaponScript.SwordArm = swordArm;
            weaponScript.Head = head;
            weaponScript.Core = core;
        }
    }

    //WaitUntill is between UpdateAndLateUpdate
    private IEnumerator AttackAnimation()
    {
        isInAnimation = true;
        startTime = Time.time;

        yield return new WaitUntil(SwingPart);
        float returnTime = TimeFromStartOfAnimation;

        yield return new WaitUntil(ReturnPart);

        isInAnimation = false;

        bool ReturnPart()
        {
            return weaponScript.ReturnAttack(TimeFromStartOfAnimation, returnTime);
        }

        bool SwingPart()
        {
            return weaponScript.Swing(TimeFromStartOfAnimation);
        }
    }

    private IEnumerator RecoilAnimation()
    {
        isInAnimation = true;
        float cutoffTime = TimeFromStartOfAnimation;
        startTime = Time.time;

        yield return new WaitUntil(RecoilPart);

        float returnTime = TimeFromStartOfAnimation;

        yield return new WaitUntil(ReturnPart);

        isInAnimation = false;

        bool RecoilPart()
        {
            return weaponScript.RecoilAttack(TimeFromStartOfAnimation, cutoffTime);
        }

        bool ReturnPart()
        {
            return weaponScript.ReturnAttack(TimeFromStartOfAnimation, returnTime);
        }
    }

    private void Update()
    {
        if (hasWeapon)
        {
            if (attacking)
            {
                HandleAttack();
            }
            else if (blocking)
            {
                HandleBlock();
            }
            else
            {
                swordArm.data.targetPositionWeight = 0;
                swordArm.data.targetRotationWeight = 0;
            }
        }

        void HandleAttack()
        {
            switch (attackState)
            {
                case AttackState.Charge:
                    if (weaponScript.ChargeAttack(TimeFromStartOfAnimation))
                    {
                        ChangeAttackState(AttackState.Hold);
                    }
                    break;
                case AttackState.Hold:
                    if (weaponScript.HoldAttack(TimeFromStartOfAnimation))
                    {
                        ChangeAttackState(AttackState.Swing);
                        weaponScript.onSwing.Invoke();
                    }
                    break;
                case AttackState.Swing:
                    if (weaponScript.Swing(TimeFromStartOfAnimation))
                    {
                        ChangeAttackState(AttackState.Return);
                    }
                    break;
                case AttackState.Recoil:
                    if (weaponScript.RecoilAttack(TimeFromStartOfAnimation, cutoffTime))
                    {
                        ChangeAttackState(AttackState.Return);
                    }
                    break;
                case AttackState.Return:
                    if (weaponScript.ReturnAttack(TimeFromStartOfAnimation, returnTime))
                    {
                        attacking = false;
                    }
                    break;

            }
        }

        void HandleBlock()
        {
            switch (blockState)
            {
                case BlockState.Charge:
                    if (weaponScript.ChargeBlock(TimeFromStartOfAnimation))
                    {
                        ChangeBlockState(BlockState.Block);
                    }
                    break;
                case BlockState.Block:
                    if (weaponScript.Block(TimeFromStartOfAnimation))
                    {
                        ChangeBlockState(BlockState.Return);
                    }
                    break;
                case BlockState.Return:
                    if (weaponScript.ReturnBlock(TimeFromStartOfAnimation))
                    {
                        blocking = false;
                    }
                    break;
            }
        }
    }

    public void PrepareAttack(float angle)
    {
        if (hasWeapon && !attacking && !blocking)
        {
            weaponScript.Angle = angle;
        }
    }

    public void Attack(float angle)
    {
        if (hasWeapon)
        {
            if (!isInAnimation)
            {
                weaponScript.Angle = angle;
                currentAnimation = StartCoroutine(AttackAnimation());
            }
        }
    }
    
    public void Block(float angle)
    {
        if (hasWeapon)
        {
            if (!attacking && !blocking)
            {
                ChangeBlockState(BlockState.Charge);
                blocking = true;
                weaponScript.Angle = angle;
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
    }

    private void ChangeAttackState(AttackState newState)
    {
        if (attackState == AttackState.Swing || attackState == AttackState.Recoil)
        {
            returnTime = TimeFromStartOfAnimation;
        }
        else
        {
            weaponScript.SetDamageActive(false);
        }
        
        attackState = newState;
        onAttackStateChange.Invoke(attackState);
        startTime = Time.time;
    }
    
    private void ChangeBlockState(BlockState newState)
    {
        blockState = newState;
        weaponScript.SetBlockActive(blockState == BlockState.Block);
        onBlockStateChange.Invoke(blockState);
        startTime = Time.time;
    }

    public void OnGetBlocked()
    {
        onGetBlocked.Invoke();
        StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(RecoilAnimation());
    }

    public bool TryEquip(GameObject newWeaponPrefab, [NotNullWhen(true)] out Weapon? weaponScripta)
    {
        Destroy(weapon);
        weapon = Instantiate(newWeaponPrefab, hand);
        if (!weapon.TryGetComponent(out weaponScript))
        {
            Debug.LogError("No weapon script found on " + weapon);
            weaponScripta = null;
            return false;
        }

        weaponScripta = weaponScript;
        hasWeapon = true;
        weaponScript!.Companion = this;
        weaponScript.SwordArm = swordArm;
        weaponScript.Head = head;
        weaponScript.Core = core;
        return true;
    }

    public void Unequip()
    {
        Destroy(weapon);
        weaponScript = null;
        hasWeapon = false;
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
