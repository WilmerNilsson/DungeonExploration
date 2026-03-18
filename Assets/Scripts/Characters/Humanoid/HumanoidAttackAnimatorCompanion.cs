using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class HumanoidAttackAnimatorCompanion : MonoBehaviour
{
    [Header("Events")]
    [FormerlySerializedAs("onAttackStateChange")] public UnityEvent<AttackState> OnAttackStateChange;
    [FormerlySerializedAs("onBlockStateChange")] public UnityEvent<BlockState> OnBlockStateChange;

    [SerializeField] private float staggerTime;
    
    [SerializeField] private bool hasWeapon = false;
    [SerializeField] private GameObject weapon;
    [SerializeField] private TwoBoneIKConstraint swordArm; 
    [SerializeField] private Transform core;
    [SerializeField] private Transform head;
    [SerializeField] private Transform hand;
    [SerializeField] private bool isBlocking = false;

    [SerializeField] private SplineContainer swordSplineContainer;
    private float angleLimit;
    private float angle;
    private float Percentage => (angle-angleLimit) / (360f-angleLimit*2f);
    
    private float startTime;
    private float TimeFromStartOfAnimation => Time.time - startTime;
    
    private float cutoffTime; // to track how far into attack it has to reverse
    private float returnTime; // What time of the previous swing to return from
    private Weapon weaponScript;

#nullable enable

    private bool isInAnimation;
    private Coroutine? currentAnimation;

    //used for signals
    public enum AttackState
    {
        Charge,
        Hold,
        Swing,
        Return,
        Recoil
    }

    //used for signals
    public enum BlockState
    {
        Charge,
        Block,
        Parry,
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
            weaponScript.startSpline = swordSplineContainer[0];
        }
        CalculateAngleLimit();
    }

    #region AttackAnimations

    //Wait Until its between UpdateAndLateUpdate
    private IEnumerator AttackAnimation()
    {
        isInAnimation = true;
        startTime = Time.time;

        weaponScript.onSwing.Invoke();
        OnAttackStateChange?.Invoke(AttackState.Swing);

        yield return new WaitUntil(SwingPart);
        float returnTime = TimeFromStartOfAnimation;
        startTime = Time.time;

        OnAttackStateChange?.Invoke(AttackState.Return);

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

        OnAttackStateChange?.Invoke(AttackState.Return);

        yield return new WaitUntil(ReturnPart);

        isInAnimation = false;


        bool ReturnPart()
        {
            return weaponScript.ReturnAttack(TimeFromStartOfAnimation, returnTime);
        }
    }

    private IEnumerator RecoilAnimation(bool parried)
    {
        isInAnimation = true;
        float previousAnimationTime = TimeFromStartOfAnimation;
        startTime = Time.time;

        OnAttackStateChange?.Invoke(AttackState.Recoil);

        yield return new WaitUntil(RecoilPart);
        
        if (parried)
        {
            startTime = Time.time;
            yield return new WaitUntil(StunPart);
        }

        float returnTime = TimeFromStartOfAnimation;

        OnAttackStateChange?.Invoke(AttackState.Return);

        yield return new WaitUntil(ReturnPart);

        isInAnimation = false;

        bool RecoilPart()
        {
            return weaponScript.RecoilAttack(TimeFromStartOfAnimation, previousAnimationTime);
        }

        bool StunPart()
        {
            return Time.time - startTime > staggerTime;
        }

        bool ReturnPart()
        {
            return weaponScript.ReturnAttack(TimeFromStartOfAnimation, returnTime);
        }
    }
    #endregion

    #region BlockAnimations

    private IEnumerator BlockAnimation()
    {
        isInAnimation = true;
        startTime = Time.time;
    
        OnBlockStateChange?.Invoke(BlockState.Charge);
        weaponScript.SetBlockActive(true);
    
        yield return new WaitUntil(ChargePart);
    
        startTime = Time.time;
    
        OnBlockStateChange?.Invoke(BlockState.Block);
        
        yield return new WaitUntil(HoldPart);
    
        weaponScript.SetBlockActive(false);
    
        currentAnimation = StartCoroutine(ReturnBlockAnimation());
    
    
        bool ChargePart()
        {
            return weaponScript.ChargeBlock(TimeFromStartOfAnimation, Percentage);
        }
    
        bool HoldPart()
        {
            weaponScript.HoldBlock(Percentage);
            return !isBlocking;
        }
    }

    private IEnumerator ParryAnimation()
    {
        isInAnimation = true;
        startTime = Time.time;
        
        weaponScript.SetParryActive(true);
        
        OnBlockStateChange?.Invoke(BlockState.Parry);

        yield return new WaitUntil(ParryPart);
        startTime = Time.time;
        
        weaponScript.SetParryActive(false);
        weaponScript.SetBlockActive(false);
        
        yield return new WaitUntil(WaitPart);
        startTime = Time.time;
        
        yield return new WaitUntil(ReturnPart);
        
        isInAnimation = false;

        bool ParryPart()
        {
            return weaponScript.ParrySwing(TimeFromStartOfAnimation);
        }

        bool WaitPart()
        {
            return weaponScript.ParryWait(TimeFromStartOfAnimation);
        }

        bool ReturnPart()
        {
            return weaponScript.ParryReturn(TimeFromStartOfAnimation);
        }
    }

    //WaitUntill is between UpdateAndLateUpdate
    private IEnumerator ReturnBlockAnimation()
    {
        isInAnimation = true;
        startTime = Time.time;

        OnBlockStateChange?.Invoke(BlockState.Return);

        yield return new WaitUntil(ReturnPart);

        isInAnimation = false;

        bool ReturnPart()
        {
            return weaponScript.ReturnBlock(TimeFromStartOfAnimation, Percentage);
        }
    }

    #endregion

    #region AttackInput

    public void HoldAttackUpdate(float angle)
    {
        if (hasWeapon && !isInAnimation)
        {
            angle = FixAngle(angle);
            this.angle = angle;
            weaponScript.Angle = angle;
            
            weaponScript.HoldAttack(Percentage);
        }
    }

    public bool TryAttack(float angle)
    {
        if (hasWeapon && !isInAnimation)
        {
            angle = FixAngle(angle);
            this.angle = angle;
            weaponScript.Angle = angle;
            weaponScript.HoldAttack(Percentage);
            currentAnimation = StartCoroutine(AttackAnimation());
            return true;
        }
        return false;
    }

    #endregion

    #region BlockInput

    public void HoldBlock(bool start)
    {
        isBlocking = start;
        if (hasWeapon && !isInAnimation)
        {
            if(start)
            {
                weaponScript.SetBlockActive(true);
                currentAnimation = StartCoroutine(BlockAnimation());
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
        if (hasWeapon)
        {
            angle = FixAngle(angle);
            this.angle = angle;
            weaponScript.Angle = angle;
        }
    }

    public bool TryParry()
    {
        if (isBlocking)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = StartCoroutine(ParryAnimation());
            return true;
        }
        return false;
    }

    #endregion

    public void OnHitFlesh()
    {
        StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(ReturnAfterHitAnimation());
    }

    /// <summary>
    /// Interrupts Attack and starts Recoil Animation
    /// </summary>
    public void OnGetBlocked()
    {
        StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(RecoilAnimation(false));
    }
    
    /// <summary>
    /// Interrupts Attack and starts Recoil Animation with parry delay
    /// </summary>
    public void OnGetParried()
    {
        StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(RecoilAnimation(true));
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
        weaponScript.startSpline = swordSplineContainer[0];
        return true;
    }

    public void Unequip()
    {
        StopCoroutine(currentAnimation);
        Destroy(weapon);
        weaponScript = null;
        hasWeapon = false;
    }
/// <summary>
/// clamps the angle between the angle limit
/// </summary>
    private float FixAngle(float angle)
    {
        angle = Mathf.Clamp(angle, angleLimit, 360 - angleLimit);
        
        return angle;
    }

/// <summary>
/// calculates and sets the angle limit based on the spline
/// </summary>
    private void CalculateAngleLimit()
    {
        angleLimit = Vector3.Angle(swordSplineContainer[0].EvaluatePosition(0), new Vector3(0, -1, .5f));
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
