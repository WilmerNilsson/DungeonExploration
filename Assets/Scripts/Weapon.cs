using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onDamage;
    public UnityEvent onBlocked;
    
    [Header("Weapon Stats")]
    [SerializeField, Min(1)] private int damage = 1;
    [SerializeField, Min(1)] private int durability = 1;
    [SerializeField] private bool unbreakable;
    [SerializeField] public bool dealDamage = true;
    [SerializeField] private bool unblockable = false;
    private Collider body;
    
    [Header("Attack")]
    [SerializeField] public float angle;
    [SerializeField] private float curveHeight = 1.5f;
    [SerializeField, Tooltip("distance from middle toward start"), Range(0,1)] private float startBend;
    [SerializeField, Tooltip("distance from middle toward end"), Range(0,1)] private float endBend;
    [SerializeField] private float attackChargeTime = 0.5f;
    [SerializeField] private float attackHoldTime = 2f;
    [SerializeField] private float attackSwingTime = 1f;
    [SerializeField] private float attackResetTime = 1f;
    [SerializeField] private float attackRecoilTime = 3f;
    private float cutoffTime;
    
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
    public HumanoidAttackAnimatorCompanion companion;
    public TwoBoneIKConstraint swordArm;
    public Transform head;
    public Transform core;
    private Transform HandIK => swordArm.data.target;
    private Transform Shoulder => swordArm.data.root;
    
    private Vector3 P0 => Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up;
    private Vector3 P1 => Vector3.Lerp(P0,P3,startBend/2) + Vector3.forward * curveHeight;
    private Vector3 P2 => Vector3.Lerp(P0,P3,1-endBend/2) + Vector3.forward * curveHeight;
    private Vector3 P3 => Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.down + Vector3.forward;
    
    private void OnEnable()
    {
        body = GetComponent<Collider>();
    }

    #region Attack
    
    public bool ChargeAttack(float time) // Go from neutral to P0
    {
        swordArm.data.targetPositionWeight = time / attackChargeTime;
        swordArm.data.targetRotationWeight = time / attackChargeTime;
        
        HandIK.position = Shoulder.position + RelativeRotation(GetCurvePosition(0));
        up = Quaternion.AngleAxis(angle+90, core.forward) * head.up;
        forward = RotateVecAroundPoint(GetCurveTangent(0), Quaternion.AngleAxis(core.transform.eulerAngles.y, Vector3.up), Vector3.zero );
            
        HandIK.rotation = Quaternion.LookRotation(up, forward);
        
        return time / attackChargeTime >= 1;
    }
    
    public bool HoldAttack(float time) // Stay at P0
    {
        swordArm.data.targetPositionWeight = 1;
        swordArm.data.targetRotationWeight = 1;
        
        HandIK.position = Shoulder.position + RelativeRotation(GetCurvePosition(0));
        up = Quaternion.AngleAxis(angle+90, core.forward) * head.up;
        forward = RotateVecAroundPoint(GetCurveTangent(0), Quaternion.AngleAxis(core.transform.eulerAngles.y, Vector3.up), Vector3.zero );
            
        HandIK.rotation = Quaternion.LookRotation(up, forward);
        
        return time / attackHoldTime >= 1;
    }
    
    public bool Swing(float time) // Swing along bezier curve
    {
        swordArm.data.targetPositionWeight = 1;
        swordArm.data.targetRotationWeight = 1;
        
        HandIK.position = Shoulder.position + RelativeRotation(GetCurvePosition(time / attackSwingTime));
        up = Quaternion.AngleAxis(angle+90, core.forward) * head.up;
        forward = RotateVecAroundPoint(GetCurveTangent(time / attackSwingTime), Quaternion.AngleAxis(core.transform.eulerAngles.y, Vector3.up), Vector3.zero );
            
        HandIK.rotation = Quaternion.LookRotation(up, forward);

        return time / attackSwingTime >= 1;
    }
    
    public bool ReturnAttack(float time) // Go back to Neutral from P3
    {
        swordArm.data.targetPositionWeight = 1 - time/attackResetTime;
        swordArm.data.targetRotationWeight = 1 - time/attackResetTime;
        
        HandIK.position = Shoulder.position + RelativeRotation(GetCurvePosition(1));
        up = Quaternion.AngleAxis(angle+90, core.forward) * head.up;
        forward = RotateVecAroundPoint(GetCurveTangent(1), Quaternion.AngleAxis(core.transform.eulerAngles.y, Vector3.up), Vector3.zero );
            
        HandIK.rotation = Quaternion.LookRotation(up, forward);
        
        return time / attackResetTime >= 1;
    }
    
    public bool RecoilAttack(float time) // Bounce back along curve to P0
    {
        swordArm.data.targetPositionWeight = 1;
        swordArm.data.targetRotationWeight = 1;
        
        HandIK.position = Shoulder.position + RelativeRotation(GetCurvePosition(time / attackRecoilTime));
        
        up = Quaternion.AngleAxis(angle, core.forward) * head.up;
        forward = RotateVecAroundPoint(GetCurveTangent(time / attackRecoilTime), Quaternion.AngleAxis(core.transform.eulerAngles.y, Vector3.up), Vector3.zero );
            
        HandIK.rotation = Quaternion.LookRotation(up, forward);
        
        return time / attackRecoilTime <= 0;
    }
    
    #endregion
    
    #region Block
    
    public bool ChargeBlock(float time) // Go from neutral to BlockPos
    {
        swordArm.data.targetPositionWeight = time / blockChargeTime;
        swordArm.data.targetRotationWeight = time / blockChargeTime;
        
        HandIK.position = head.position + RelativeRotation((blockAnglePos + blockOffsetPos).normalized + Vector3.forward * 0.5f) * blockDistance;
        
        HandIK.rotation = Quaternion.LookRotation(Vector3.forward * angle, RelativeRotation(blockAnglePos + blockOffsetPos * 0.5f));
        
        return time / blockChargeTime > 1;
    }
    
    public bool Block(float time) // Stay at BlockPos
    {
        swordArm.data.targetPositionWeight = 1;
        swordArm.data.targetRotationWeight = 1;
        
        HandIK.position = head.position + RelativeRotation((blockAnglePos + blockOffsetPos).normalized * blockDistance + Vector3.forward * 0.5f);
        
        HandIK.rotation = Quaternion.LookRotation(Vector3.forward * angle, RelativeRotation(blockAnglePos + blockOffsetPos * 0.5f));
        
        return time / blockHoldTime > 1;
    }
    
    public bool ReturnBlock(float time) // Go back to Neutral from BlockPos
    {
        swordArm.data.targetPositionWeight = 1 - time / blockReturnTime;
        swordArm.data.targetRotationWeight = 1 - time / blockReturnTime;
        
        HandIK.position = head.position + RelativeRotation((blockAnglePos + blockOffsetPos).normalized + Vector3.forward * 0.5f) * blockDistance;
        
        HandIK.rotation = Quaternion.LookRotation(Vector3.forward * angle, RelativeRotation(blockAnglePos + blockOffsetPos * 0.5f));
        
        return time / blockReturnTime > 1;
    }
    
    #endregion

    public void SetActive(bool value)
    {
        body.enabled = value;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter {other.gameObject.name}");
        if(dealDamage && !transform.IsChildOf(other.transform) && other.TryGetComponent(out Health health))
        {
            onDamage?.Invoke();
            health.TakeDamage(damage);
            LoseDurability(health.DurabilityDamage);
            SetActive(false);
            Debug.Log($"The target {other.gameObject.name} health is " + health.CurrentHealth);
        }
        if (!unblockable && other.TryGetComponent(out Weapon weapon))
        {
            onBlocked?.Invoke();
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
    private Vector3 RelativeRotation(Vector3 rotation)
    {
        return Quaternion.AngleAxis(core.transform.eulerAngles.y, Vector3.up) * Quaternion.AngleAxis(Mathf.Asin(head.forward.y) * Mathf.Rad2Deg, Vector3.left) * rotation;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(head.position + RelativeRotation(blockAnglePos), 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(head.position + RelativeRotation(blockOffsetPos), 0.1f);
    }
}
