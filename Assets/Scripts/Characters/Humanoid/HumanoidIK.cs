using System;
using UnityEngine;
using UnityEngine.Events;

public class HumanoidIK : MonoBehaviour
{
    public UnityEvent<AttackState> onAttackStateChange;
    public UnityEvent<BlockState> onBlockStateChange;
    [SerializeField] protected AttackState attackState = AttackState.Start;
    [SerializeField] protected BlockState blockState = BlockState.Start;
    [SerializeField] protected bool attacking = false;
    [SerializeField] protected bool blocking = false;
    
    public Animator animator;
    protected Weapon weapon;
    [SerializeField, Tooltip("The Shoulder node of the weapon arm")] protected Transform shoulderObj = null;
    [SerializeField, Tooltip("The Avatars head")] protected Transform headObj = null;
    [SerializeField, Tooltip("Where the avatar should look")] protected Transform lookObj = null; 
    protected Vector3 Forward => headObj.position + headObj.forward;
    
    [SerializeField,Tooltip("the rotation of the hand, readonly as they are set in code")] protected float x, z;
    [Header("Attack Settings")]
    [SerializeField, Tooltip("the min angle between the attack and straight up/down")] protected float angleLimit = 30f;
    [SerializeField, Tooltip("the lenght of the weapon arm")] protected float armLenght = 2f;
    
    [Header("Attacking")]
    [SerializeField] protected Vector3 swingStart;
    [SerializeField] protected Vector3 swingEnd;
    [SerializeField] protected float swingAngle;
    [SerializeField] protected float curveHeight = 2.5f;
    [SerializeField] protected float nodeTime = 0.5f;
    [SerializeField] protected float chargeTime = 2f;
    [SerializeField] protected float resetTime = 1f;
    [SerializeField] protected float recoilTime = 3f;
    protected Quaternion rotation;
    
    [Header("Blocking")]
    [SerializeField] protected float blockChargeTime = .5f;
    [SerializeField] protected float blockTime = 1f;
    [SerializeField] protected float blockResetTime = .5f;
    [SerializeField] protected float handOffset = .5f;
    protected float blockAngle;
    protected Vector3 blockPos;
    protected Quaternion blockRot;
    protected Vector3 blockPosMod;
    
    protected float startTime = 0;
    protected float time = 0;


    private void Start()
    {
        weapon = GetComponentInChildren<Weapon>();
    }

    public enum AttackState
    {
        Start,
        Swing,
        Return,
        Interrupt
    }

    public enum BlockState
    {
        Start,
        Block,
        Return
    }

    public virtual void Attack(float angle = 0) {}

    public virtual void Block(float angle = 0) {}
    
    protected virtual void OnAnimatorIK(int layerIndex)
    {
        
    }

    protected virtual void ChangeAttackState(AttackState newState)
    {
        if(newState != attackState)
        {
            attackState = newState; 
            onAttackStateChange?.Invoke(attackState);
        }
    }
    
    protected virtual void ChangeBlockState(BlockState newState)
    {
        if(newState != blockState)
        {
            blockState = newState; 
            onBlockStateChange?.Invoke(blockState);
        }
    }
    
    protected Vector3 RelativePosition(Vector3 position)
    {
        return transform.TransformDirection(position) + headObj.position;
    }

    protected Quaternion RelativeRotation(Quaternion rotation)
    {
        Vector3 euler = rotation.eulerAngles;
        euler.y += transform.parent.eulerAngles.y;
        return Quaternion.Euler(euler);
    }
    
    protected virtual void Reset()
    {
        attackState = AttackState.Start;
        blockState = BlockState.Start;
        startTime = 0;
        time = 0;
        
        weapon.SetActive(false);
    }

    protected Vector3 GetCurvePosition(float t)
    {
        Vector3 heighPoint = swingStart + (swingEnd - swingStart) / 2 + headObj.forward * curveHeight;
        return (Mathf.Pow(1 - t, 2) * swingStart) + (2 * (1 - t) * t * heighPoint) + (t * t * swingEnd);
    }
}
