using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onDamage;
    public UnityEvent<string, Vector3> onDeflectCollision;
    
    [Header("Weapon Stats")]
    [SerializeField, Min(1)] private int damage = 1;
    [SerializeField, Min(1)] private int durability = 1;
    [SerializeField] private bool dealDamage = false;
    [SerializeField] private bool isBlocking = false;
    [SerializeField] private bool unbreakable;
    [SerializeField] private bool unblockable = false;
    [SerializeField] private Collider body;
    
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

    #region Attack
    
    public bool ChargeAttack(float time) // Go from neutral to P0
    {
        swordArm.data.targetPositionWeight = time / attackChargeTime;
        swordArm.data.targetRotationWeight = time / attackChargeTime;
        
        SetPositionRotation(0);
        
        return time / attackChargeTime >= 1;
    }
    
    public bool HoldAttack(float time) // Stay at P0
    {
        swordArm.data.targetPositionWeight = 1;
        swordArm.data.targetRotationWeight = 1;
        
        SetPositionRotation(0);
        
        return time / attackHoldTime >= 1;
    }
    
    public bool Swing(float time) // Swing along bezier curve
    {
        swordArm.data.targetPositionWeight = 1;
        swordArm.data.targetRotationWeight = 1;
        
        SetPositionRotation(time / attackSwingTime);
        
        SetDamageActive(time / attackSwingTime > .1 && time / attackSwingTime < .9);
        
        return time / attackSwingTime >= 1;
    }
    
    public bool ReturnAttack(float time, float returnTime) // Go back to Neutral from P3
    {
        swordArm.data.targetPositionWeight = 1 - time/attackResetTime;
        swordArm.data.targetRotationWeight = 1 - time/attackResetTime;
        
        SetPositionRotation(returnTime);
        
        return time / attackResetTime >= 1;
    }
    
    public bool RecoilAttack(float time, float cutoffTime) // Bounce back along curve to P0
    {
        float localTime = (cutoffTime - time) / attackSwingTime;
        
        swordArm.data.targetPositionWeight = 1;
        swordArm.data.targetRotationWeight = 1;
        
        SetPositionRotation(localTime);
        
        return localTime <= 0;
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
                    onDamage?.Invoke();
                    health.TakeDamage(damage);
                    LoseDurability(health.DurabilityDamage);
                    Debug.Log($"The target {other.gameObject.name} health is " + health.CurrentHealth);
                }
                else if (other.gameObject.layer == LayerMask.NameToLayer("Character")) // Potential fix for ragdoll 
                {
                    health = other.gameObject.GetComponentInParent<Health>();
                    if (health != null)
                    {
                        onDamage?.Invoke();
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
                        onDeflectCollision.Invoke("Wood", transform.position);
                        companion.OnGetBlocked();
                        break;
                    case "Stone":
                        Debug.Log("Stone");
                        onDeflectCollision.Invoke("Stone", transform.position);
                        companion.OnGetBlocked();
                        break;
                    case "Metal":
                        Debug.Log("Metal");
                        if (other.TryGetComponent(out Weapon weapon) && !weapon.isBlocking)
                        {
                            Debug.Log("Other weapon not blocking");
                            break;
                        }
                        onDeflectCollision.Invoke("Metal", transform.position);
                        companion.OnGetBlocked();
                        break;
                    case "Player":
                    case "Flesh":
                        Debug.Log("Flesh");
                        onDeflectCollision.Invoke("Flesh", transform.position);
                        companion.ChangeAttackState(HumanoidAttackAnimatorCompanion.AttackState.Return);
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
        
        up = Quaternion.AngleAxis(angle+90, core.forward) * Vector3.ProjectOnPlane(head.up,Vector3.forward); // Doesnt account for head tilt
        forward = RotateVecAroundPoint(GetCurveTangent(time), Quaternion.AngleAxis(core.transform.eulerAngles.y, Vector3.up), Vector3.zero );
            
        HandIK.rotation = Quaternion.LookRotation(up, forward);
    }
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
