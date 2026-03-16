using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class Weapon : MonoBehaviour
{
    [Header("Events")]
    [FormerlySerializedAs("onDamage")] public UnityEvent OnDamage;
    [FormerlySerializedAs("onDeflectCollision")] public UnityEvent<string, Vector3> OnDeflectCollision;
    public UnityEvent onParry;
    public UnityEvent onSwing;
    
    [Header("Weapon Stats")]
    [SerializeField, Min(1)] private int damage = 1;
    [field: SerializeField, Min(1), FormerlySerializedAs("durability")] 
    public int Durability 
    {
        get;
        set;
    } = 1;
    
    [SerializeField] private bool dealDamage = false;
    [SerializeField] private bool isBlocking = false;
    [SerializeField] private bool unbreakable;
    [SerializeField] private Collider body;
    public Spline startSpline;
    private float splinePosition;
    
    [Header("Attack")]
    [SerializeField, FormerlySerializedAs("angle")] public float Angle;
    [SerializeField] private float curveHeight = 1.5f;
    [SerializeField, Tooltip("distance from middle toward start"), Range(0,1)] private float startBend;
    [SerializeField, Tooltip("distance from middle toward end"), Range(0,1)] private float endBend;
    [SerializeField] private float attackChargeTime = 0.5f;
    [SerializeField] private float attackHoldTime = 2f;
    [SerializeField] private float attackSwingTime = 1f;
    [SerializeField] private float attackResetTime = 1f;
    
    [Header("Block")]
    [SerializeField] private float blockDistance = 0.5f;
    [SerializeField] private float blockChargeTime = 0.5f;
    [SerializeField] private float blockHoldTime = 1f;
    [SerializeField] private float blockReturnTime = 0.5f;
    
    private Vector3 up;
    private Vector3 forward;
    private Vector3 right;
    
    private Vector3 blockAnglePos;
    private Vector3 blockOffsetPos;
    public Vector3 BlockPos 
    {
        set => blockAnglePos = value;
    }
    public Vector3 BlockOffset 
    {
        set => blockOffsetPos = value;
    }
    
    [Header("References")]
    [FormerlySerializedAs("companion")] public HumanoidAttackAnimatorCompanion Companion;
    [FormerlySerializedAs("swordArm")] public TwoBoneIKConstraint SwordArm;
    [FormerlySerializedAs("head")] public Transform Head;
    [FormerlySerializedAs("core")] public Transform Core;
    private Transform HandIK => SwordArm.data.target;
    private Transform Shoulder => SwordArm.data.root;
    
    private Vector3 P0 => startSpline.EvaluatePosition(splinePosition);
    private Vector3 P1 => Vector3.Lerp(P0,P3,startBend/2) + Vector3.forward * curveHeight;
    private Vector3 P2 => Vector3.Lerp(P0,P3,1-endBend/2) + Vector3.forward * curveHeight;
    private Vector3 P3 => new (-P0.x, -P0.y, P0.z);

    #region Attack

    /// <summary>
    ///  Go from neutral to P0 <br/>
    /// returns true when time is more than attack time and state machine can continue
    /// </summary>
    public bool ChargeAttack(float time)
    {
        SwordArm.data.targetPositionWeight = time / attackChargeTime;
        SwordArm.data.targetRotationWeight = time / attackChargeTime;
        
        AttackPositionRotation(0);
        
        return time >= attackChargeTime;
    }

    /// <summary>
    /// Stay at P0 <br/>
    /// </summary>
    public void HoldAttack(float percentage)
    {
        SwordArm.data.targetPositionWeight = 1;
        SwordArm.data.targetRotationWeight = 1;
        
        splinePosition = percentage;
        
        AttackPositionRotation(0);
    }

    /// <summary>
    /// Swing along bezier curve <br/>
    /// returns true when time is more than attack time and state machine can continue
    /// </summary>
    public bool Swing(float time)
    {
        SwordArm.data.targetPositionWeight = 1;
        SwordArm.data.targetRotationWeight = 1;
        
        AttackPositionRotation(time / attackSwingTime);
        
        SetDamageActive(time / attackSwingTime > .1 && time / attackSwingTime < .9);
        
        return time >= attackSwingTime;
    }

    /// <summary>
    /// Go back to Neutral from P3 <br/>
    /// returns true when time is more than attack time and state machine can continue
    /// </summary>
    public bool ReturnAttack(float time, float returnTime)
    {
        SwordArm.data.targetPositionWeight = 1 - time/attackResetTime;
        SwordArm.data.targetRotationWeight = 1 - time/attackResetTime;
        
        AttackPositionRotation(returnTime / attackSwingTime);
        
        return time >= attackResetTime;
    }

    /// <summary>
    /// Bounce back along curve to P0 <br/>
    /// returns true when time is more than attack time and state machine can continue
    /// </summary>
    public bool RecoilAttack(float time, float cutoffTime)
    {
        float localTime = (cutoffTime - time) / attackSwingTime;
        
        SwordArm.data.targetPositionWeight = 1;
        SwordArm.data.targetRotationWeight = 1;
        
        AttackPositionRotation(localTime);
        
        return localTime <= 0;
    }

    #endregion

    #region Block

    /// <summary>
    /// Go from neutral to BlockPos <br/>
    /// returns true when time is more than block time and state machine can continue
    /// </summary>
    public bool ChargeBlock(float time)
    {
        SwordArm.data.targetPositionWeight = time / blockChargeTime;
        SwordArm.data.targetRotationWeight = time / blockChargeTime;
        
        BlockPositionRotation(1);
        
        return time >= blockChargeTime;
    }

    /// <summary>
    /// Stay at BlockPos <br/>
    /// returns true when time is more than block time and state machine can continue
    /// </summary>
    public bool HoldBlock(float time)
    {
        SwordArm.data.targetPositionWeight = 1;
        SwordArm.data.targetRotationWeight = 1;
        
        BlockPositionRotation(1);
        
        return time >= blockHoldTime;
    }
    /// <summary>
    /// Go back to Neutral from BlockPos <br/>
    /// returns true when time is more than block time and state machine can continue
    /// </summary>

    public bool ReturnBlock(float time)
    {
        SwordArm.data.targetPositionWeight = 1 - time / blockReturnTime;
        SwordArm.data.targetRotationWeight = 1 - time / blockReturnTime;
        
        BlockPositionRotation(1);
        
        return time >= blockReturnTime;
    }
    
    #endregion

    public void SetDamageActive(bool value)
    {
        dealDamage = value;
    }
    
    public void SetBlockActive(bool value)
    {
        isBlocking = value;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (dealDamage)
        {
            Debug.Log($"OnTriggerEnter name {other.gameObject.name} tag {other.gameObject.tag}");
            if (!transform.IsChildOf(other.transform))
            {
                if (other.TryGetComponent(out Health health))
                {
                    health.TakeDamage(damage);
                    LoseDurability(health.DurabilityDamage);
                    OnDamage?.Invoke();
                    Debug.Log($"The target {other.gameObject.name} health is " + health.CurrentHealth);
                }
                else if (other.gameObject.CompareTag("Flesh")) // Potential fix for ragdoll 
                {
                    health = other.gameObject.GetComponentInParent<Health>();
                    Debug.Log($"health object is {health.gameObject.name}, companion object is {Companion.gameObject.name}");
                    if (health.gameObject == Companion.gameObject) return;
                    if (health != null)
                    {
                        health.TakeDamage(damage);
                        LoseDurability(health.DurabilityDamage);
                        OnDamage?.Invoke();
                        Debug.Log($"The target {other.gameObject.name} health is " + health.CurrentHealth);
                    }
                    else
                    {
                        Debug.Log($"The target {other.gameObject.name} health is NULL, or it was self harm");
                    }
                }
                switch (other.tag) // Handle Audio Visual Feedback
                {
                    case "Wood":
                        Debug.Log("Wood");
                        OnDeflectCollision.Invoke("Wood", transform.position);
                        Companion.OnGetBlocked();
                        break;
                    case "Stone":
                        Debug.Log("Stone");
                        OnDeflectCollision.Invoke("Stone", transform.position);
                        Companion.OnGetBlocked();
                        break;
                    case "Metal":
                        Debug.Log("Metal");
                        if (other.TryGetComponent(out Weapon weapon))
                        {
                            if (!weapon.isBlocking)
                            {
                                Debug.Log("Other weapon not blocking");
                            }
                            else
                            {
                                onParry.Invoke();
                                Companion.OnGetBlocked();
                            }
                            break;
                        }
                        OnDeflectCollision.Invoke("Metal", transform.position);
                        Companion.OnGetBlocked();
                        break;
                    case "Player":
                    case "Flesh":
                        Debug.Log("Flesh");
                        OnDeflectCollision.Invoke("Flesh", transform.position);
                        Companion.OnHitFlesh();
                        break;
                }
            }
            dealDamage = false;
        }
    }
    
    public void LoseDurability(int amount)
    {
        if (unbreakable)
        {
            return;
        }
        Durability -= amount;
        if (Durability <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    #region Support Functions

    private void AttackPositionRotation(float time)
    {
        HandIK.position = Head.position + RelativeRotation(GetCurvePosition(time));
        
        up = Quaternion.AngleAxis(Angle+90, Head.forward) * Head.up;
        Debug.DrawRay(HandIK.position, up, Color.green);
        forward = RotateVecAroundPoint(GetCurveTangent(time), Quaternion.AngleAxis(Core.transform.eulerAngles.y, Vector3.up), Vector3.zero );
        Debug.DrawRay(HandIK.position, forward, Color.blue); 
        HandIK.rotation = Quaternion.LookRotation(up, forward);
    }

    private void BlockPositionRotation(float time)
    {
        HandIK.position = Head.position + RelativeRotation((blockAnglePos + blockOffsetPos).normalized * blockDistance + Vector3.forward * 0.5f);
        
        HandIK.rotation = Quaternion.LookRotation(RelativeRotation((Vector3.forward * Angle).normalized), RelativeRotation(blockAnglePos + blockOffsetPos * 0.5f));
    }
    private Vector3 RelativeRotation(Vector3 rotation)
    {
        return Quaternion.AngleAxis(Core.transform.eulerAngles.y, Vector3.up) * Quaternion.AngleAxis(Mathf.Asin(Head.forward.y) * Mathf.Rad2Deg, Vector3.left) * rotation;
    }
    
    private Vector3 GetCurvePosition(float t)
    {
        Vector3 cubic = (Mathf.Pow(1 - t, 3) * P0) + 3 * Mathf.Pow(1 - t, 2)*t*P1 + 3*(1-t)*t*t * P2 + t*t*t * P3;
        return cubic;
    }

    private Vector3 GetCurveTangent(float t)
    {
        Vector3 a = 3 * (P1 - P0);
        Vector3 b = 3 * (P2 - P1);
        Vector3 c = 3 * (P3 - P2);
        return a * Mathf.Pow(1 - t, 2) + b * (2 * (1 - t) * t) + c * (t * t);
    }
    
    private Vector3 GetCurveNormal(float t) // Doesnt really work atm
    {
        Vector3 normal = Vector3.Cross(up, forward);
        return normal.normalized;
    }
    
    private Vector3 RotateVecAroundPoint(Vector3 vector, Quaternion rotation, Vector3 point)
    {
        return rotation * (vector - point) + point;
    }
    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (startSpline != null)
        {
            Gizmos.color = Color.blue;
            for (float i = 0; i < 1; i+=.01f)
            {
                Vector3 position = Head.transform.TransformPoint(GetCurvePosition(i));
                Gizmos.DrawSphere(position, .01f);
            }
        }
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(P0,.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(P3,.1f);
    }
#endif

}
