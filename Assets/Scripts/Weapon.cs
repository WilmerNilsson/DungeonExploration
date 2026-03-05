using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Weapon : MonoBehaviour
{
    [Header("Events")]
    [FormerlySerializedAs("onDamage")] public UnityEvent OnDamage;
    [FormerlySerializedAs("onDeflectCollision")] public UnityEvent<string, Vector3> OnDeflectCollision;
    
    [Header("Weapon Stats")]
    [SerializeField, Min(1)] private int damage = 1;
    [SerializeField, Min(1)] private int durability = 1;
    [SerializeField] private bool dealDamage = false;
    [SerializeField] private bool isBlocking = false;
    [SerializeField] private bool unbreakable;
    [SerializeField] private bool unblockable = false;
    [SerializeField] private Collider body;
    
    [Header("Attack")]
    [SerializeField, FormerlySerializedAs("angle")] public float Angle;
    [SerializeField] private float curveHeight = 1.5f;
    [SerializeField, Tooltip("distance from middle toward start"), Range(0,1)] private float startBend;
    [SerializeField, Tooltip("distance from middle toward end"), Range(0,1)] private float endBend;
    [SerializeField] private float attackChargeTime = 0.5f;
    [SerializeField] private float attackHoldTime = 2f;
    [SerializeField] private float attackSwingTime = 1f;
    [SerializeField] private float attackResetTime = 1f;
    [SerializeField] private float attackRecoilTime = 3f;
    
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
    
    private Vector3 P0 => Quaternion.AngleAxis(Angle, Vector3.forward) * Vector3.up;
    private Vector3 P1 => Vector3.Lerp(P0,P3,startBend/2) + Vector3.forward * curveHeight;
    private Vector3 P2 => Vector3.Lerp(P0,P3,1-endBend/2) + Vector3.forward * curveHeight;
    private Vector3 P3 => Quaternion.AngleAxis(Angle, Vector3.forward) * Vector3.down + Vector3.forward;

    #region Attack
    
    public bool ChargeAttack(float time) // Go from neutral to P0
    {
        SwordArm.data.targetPositionWeight = time / attackChargeTime;
        SwordArm.data.targetRotationWeight = time / attackChargeTime;
        
        SetPositionRotation(0);
        
        return time / attackChargeTime >= 1;
    }
    
    public bool HoldAttack(float time) // Stay at P0
    {
        SwordArm.data.targetPositionWeight = 1;
        SwordArm.data.targetRotationWeight = 1;
        
        SetPositionRotation(0);
        
        return time / attackHoldTime >= 1;
    }
    
    public bool Swing(float time) // Swing along bezier curve
    {
        SwordArm.data.targetPositionWeight = 1;
        SwordArm.data.targetRotationWeight = 1;
        
        SetPositionRotation(time / attackSwingTime);
        
        SetDamageActive(time / attackSwingTime > .1 && time / attackSwingTime < .9);
        
        return time / attackSwingTime >= 1;
    }
    
    public bool ReturnAttack(float time, float returnTime) // Go back to Neutral from P3
    {
        SwordArm.data.targetPositionWeight = 1 - time/attackResetTime;
        SwordArm.data.targetRotationWeight = 1 - time/attackResetTime;
        
        SetPositionRotation(returnTime);
        
        return time / attackResetTime >= 1;
    }
    
    public bool RecoilAttack(float time, float cutoffTime) // Bounce back along curve to P0
    {
        float localTime = (cutoffTime - time) / attackSwingTime;
        
        SwordArm.data.targetPositionWeight = 1;
        SwordArm.data.targetRotationWeight = 1;
        
        SetPositionRotation(localTime);
        
        return localTime <= 0;
    }
    
    #endregion
    
    #region Block
    
    public bool ChargeBlock(float time) // Go from neutral to BlockPos
    {
        SwordArm.data.targetPositionWeight = time / blockChargeTime;
        SwordArm.data.targetRotationWeight = time / blockChargeTime;
        
        HandIK.position = Head.position + RelativeRotation((blockAnglePos + blockOffsetPos).normalized + Vector3.forward * 0.5f) * blockDistance;
        
        HandIK.rotation = Quaternion.LookRotation(Vector3.forward * Angle, RelativeRotation(blockAnglePos + blockOffsetPos * 0.5f));
        
        return time / blockChargeTime > 1;
    }
    
    public bool Block(float time) // Stay at BlockPos
    {
        SwordArm.data.targetPositionWeight = 1;
        SwordArm.data.targetRotationWeight = 1;
        
        HandIK.position = Head.position + RelativeRotation((blockAnglePos + blockOffsetPos).normalized * blockDistance + Vector3.forward * 0.5f);
        
        HandIK.rotation = Quaternion.LookRotation(Vector3.forward * Angle, RelativeRotation(blockAnglePos + blockOffsetPos * 0.5f));
        
        return time / blockHoldTime > 1;
    }
    
    public bool ReturnBlock(float time) // Go back to Neutral from BlockPos
    {
        SwordArm.data.targetPositionWeight = 1 - time / blockReturnTime;
        SwordArm.data.targetRotationWeight = 1 - time / blockReturnTime;
        
        HandIK.position = Head.position + RelativeRotation((blockAnglePos + blockOffsetPos).normalized + Vector3.forward * 0.5f) * blockDistance;
        
        HandIK.rotation = Quaternion.LookRotation(Vector3.forward * Angle, RelativeRotation(blockAnglePos + blockOffsetPos * 0.5f));
        
        return time / blockReturnTime > 1;
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
                    OnDamage?.Invoke();
                    health.TakeDamage(damage);
                    LoseDurability(health.DurabilityDamage);
                    Debug.Log($"The target {other.gameObject.name} health is " + health.CurrentHealth);
                }
                else if (other.gameObject.layer == LayerMask.NameToLayer("Character")) // Potential fix for ragdoll 
                {
                    health = other.gameObject.GetComponentInParent<Health>();
                    if (health != null)
                    {
                        OnDamage?.Invoke();
                        health.TakeDamage(damage);
                        LoseDurability(health.DurabilityDamage);
                        Debug.Log($"The target {other.gameObject.name} health is " + health.CurrentHealth);
                    }
                    else
                    {
                        Debug.Log($"The target {other.gameObject.name} health is NULL");
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
                        if (other.TryGetComponent(out Weapon weapon) && !weapon.isBlocking)
                        {
                            Debug.Log("Other weapon not blocking");
                            break;
                        }
                        OnDeflectCollision.Invoke("Metal", transform.position);
                        Companion.OnGetBlocked();
                        break;
                    case "Player":
                    case "Flesh":
                        Debug.Log("Flesh");
                        OnDeflectCollision.Invoke("Flesh", transform.position);
                        Companion.ChangeAttackState(HumanoidAttackAnimatorCompanion.AttackState.Return);
                        break;
                }
            }
        }
    }
    
    public void LoseDurability(int amount)
    {
        if (unbreakable)
        {
            return;
        }
        durability -= amount;
        if (durability <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    #region Support Functions

    private void SetPositionRotation(float time)
    {
        HandIK.position = Shoulder.position + RelativeRotation(GetCurvePosition(time));
        
        up = Quaternion.AngleAxis(Angle+90, Core.forward) * Vector3.ProjectOnPlane(Head.up,Vector3.forward); // Doesnt account for head tilt
        forward = RotateVecAroundPoint(GetCurveTangent(time), Quaternion.AngleAxis(Core.transform.eulerAngles.y, Vector3.up), Vector3.zero );
            
        HandIK.rotation = Quaternion.LookRotation(up, forward);
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
        if (Head == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(Head.position + RelativeRotation(blockAnglePos), 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Head.position + RelativeRotation(blockOffsetPos), 0.1f);
    }
#endif

}
