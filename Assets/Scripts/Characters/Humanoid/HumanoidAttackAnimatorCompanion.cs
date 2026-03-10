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

    #region AttackAnimations
    private IEnumerator ChargeAttackAnimaton()
    {
        isInAnimation = true;
        startTime = Time.time;

        yield return new WaitUntil(ChargePart);

        startTime = Time.time;

        yield return new WaitUntil(HoldPart);

        isInAnimation = false;

        currentAnimation = StartCoroutine(AttackAnimation());


        bool ChargePart()
        {
            return weaponScript.ChargeAttack(TimeFromStartOfAnimation);
        }

        bool HoldPart()
        {
            return weaponScript.HoldAttack(TimeFromStartOfAnimation);
        }
    }

    //WaitUntill is between UpdateAndLateUpdate
    private IEnumerator AttackAnimation()
    {
        isInAnimation = true;
        startTime = Time.time;

        yield return new WaitUntil(SwingPart);
        float returnTime = TimeFromStartOfAnimation;
        startTime = Time.time;

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

    private IEnumerator ReturnAfterHitAnimation()
    {
        isInAnimation = true;
        float returnTime = TimeFromStartOfAnimation;
        startTime = Time.time;

        yield return new WaitUntil(ReturnPart);

        isInAnimation = false;


        bool ReturnPart()
        {
            return weaponScript.ReturnAttack(TimeFromStartOfAnimation, returnTime);
        }
    }

    private IEnumerator RecoilAnimation()
    {
        isInAnimation = true;
        float previousAnimationTime = TimeFromStartOfAnimation;
        startTime = Time.time;

        yield return new WaitUntil(RecoilPart);

        float returnTime = TimeFromStartOfAnimation;

        yield return new WaitUntil(ReturnPart);

        isInAnimation = false;

        bool RecoilPart()
        {
            return weaponScript.RecoilAttack(TimeFromStartOfAnimation, previousAnimationTime);
        }

        bool ReturnPart()
        {
            return weaponScript.ReturnAttack(TimeFromStartOfAnimation, returnTime);
        }
    }
    #endregion

    #region BlockAnimations

    private IEnumerator ChargeBlockAnimaton()
    {
        isInAnimation = true;
        startTime = Time.time;

        yield return new WaitUntil(ChargePart);

        startTime = Time.time;

        weaponScript.SetBlockActive(true);

        yield return new WaitUntil(HoldPart);

        weaponScript.SetBlockActive(false);

        isInAnimation = false;

        currentAnimation = StartCoroutine(ReturnBlockAnimation());


        bool ChargePart()
        {
            return weaponScript.ChargeBlock(TimeFromStartOfAnimation);
        }

        bool HoldPart()
        {
            return weaponScript.HoldBlock(TimeFromStartOfAnimation);
        }


    }

    //WaitUntill is between UpdateAndLateUpdate
    private IEnumerator ReturnBlockAnimation()
    {
        isInAnimation = true;
        startTime = Time.time;

        yield return new WaitUntil(ReturnPart);

        isInAnimation = false;

        bool ReturnPart()
        {
            return weaponScript.ReturnBlock(TimeFromStartOfAnimation);
        }
    }

    #endregion

    private void Update()
    {
        if (hasWeapon)
        {
            if (blocking)
            {
                HandleBlock();
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
                    if (weaponScript.HoldBlock(TimeFromStartOfAnimation))
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

    #region AttackInput

    //not sure what this is
    //public void PrepareAttack(float angle)
    //{
    //    if (hasWeapon && !attacking && !blocking)
    //    {
    //        weaponScript.Angle = angle;
    //    }
    //}

    public void HoldAttackUpdate(float angle)
    {
        if (hasWeapon && !isInAnimation)
        {
            weaponScript.Angle = angle;

            weaponScript.HoldAttack(0f, 0.2f);
        }
    }

    public void HoldAttack(bool start)
    {
        if (hasWeapon && !isInAnimation && ! start)
        {
            swordArm.data.targetPositionWeight = 0;
            swordArm.data.targetRotationWeight = 0;
        }
    }

    public void Attack(float angle)
    {
        if (hasWeapon && !isInAnimation)
        {
            weaponScript.Angle = angle;
            currentAnimation = StartCoroutine(AttackAnimation());
        }
    }

    public void AttackWithChargeupp(float angle)
    {
        if (hasWeapon && !isInAnimation)
        {
            weaponScript.Angle = angle;
            currentAnimation = StartCoroutine(ChargeAttackAnimaton());
        }
    }

    #endregion

    #region BlockInput

    public void HoldBlock(bool start)
    {
        if (hasWeapon && !isInAnimation)
        {
            if(start)
            {
                weaponScript.SetBlockActive(true);
            }
            else
            {
                swordArm.data.targetPositionWeight = 0;
                swordArm.data.targetRotationWeight = 0;

                weaponScript.SetBlockActive(false);
            }
        }
    }

    public void HoldBlockUpdate(float angle)
    {
        if (hasWeapon && !isInAnimation)
        {
            weaponScript.Angle = angle;

            weaponScript.HoldBlock(0f);
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

    public void BlockWithChargeup(float angle)
    {
        if (hasWeapon && !isInAnimation)
        {
            currentAnimation = StartCoroutine(ChargeBlockAnimaton());
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


    #endregion

    public void OnHitFlesh()
    {
        StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(ReturnAfterHitAnimation());
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
