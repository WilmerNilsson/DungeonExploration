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
    
    [Header("State Stats")]
    [SerializeField] protected float chargeTime = 0.5f;
    [SerializeField] protected float holdTime = 2f;
    [SerializeField] protected float swingTime = 1f;
    [SerializeField] protected float resetTime = 1f;
    [SerializeField] protected float recoilTime = 3f;
    private float cutoffTime;
    
    private Vector3 up;
    private Vector3 forward;
    private Vector3 right;
    
    [HideInInspector] public HumanoidAttackAnimatorCompanion companion;
    [HideInInspector] public TwoBoneIKConstraint swordArm;
    [HideInInspector] public Transform head;
    [HideInInspector] public Transform core;
    private Transform HandIK => swordArm.data.target;
    private Transform Shoulder => swordArm.data.root;
    
    private Vector3 P0 => Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
    private Vector3 P1 => Vector3.Lerp(P0,P3,startBend/2) + Vector3.forward * curveHeight;
    private Vector3 P2 => Vector3.Lerp(P0,P3,1-endBend/2) + Vector3.forward * curveHeight;
    private Vector3 P3 => Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.left + Vector3.forward;
    
    private void OnEnable()
    {
        body = GetComponent<Collider>();
    }

    #region Attack
    
    public bool ChargeAttack(float time) // Go from neutral to P0
    {
        swordArm.data.targetPositionWeight = time / chargeTime;
        swordArm.data.targetRotationWeight = time / chargeTime;
        HandIK.position = Shoulder.position + P0;
        return time / chargeTime >= 1;
    }
    
    public bool HoldAttack(float time) // Stay at P0
    {
        swordArm.data.targetPositionWeight = 1;
        swordArm.data.targetRotationWeight = 1;
        HandIK.position = Shoulder.position + P0;
        return time / holdTime >= 1;
    }
    
    public bool Swing(float time) // Swing along bezier curve
    {
        swordArm.data.targetPositionWeight = 1;
        swordArm.data.targetRotationWeight = 1;
        
        HandIK.position = Shoulder.position + RelativeRotation(GetCurvePosition(time / swingTime));
        up = Quaternion.AngleAxis(angle, core.forward) * head.up;
        forward = RotateVecAroundPoint(GetCurveTangent(time / swingTime), Quaternion.AngleAxis(core.transform.eulerAngles.y, Vector3.up), Vector3.zero );
            
        HandIK.rotation = Quaternion.LookRotation(up, forward);

        return time / swingTime >= 1;
    }
    
    public bool ReturnAttack(float time) // Go back to Neutral from P3
    {
        swordArm.data.targetPositionWeight = 1 - time/recoilTime;
        swordArm.data.targetRotationWeight = 1 - time/recoilTime;
        return time / resetTime >= 1;
    }
    
    public bool RecoilAttack(float time) // Bounce back along curve to P0
    {
        swordArm.data.targetPositionWeight = 1;
        swordArm.data.targetRotationWeight = 1;
        
        HandIK.position = Shoulder.position + RelativeRotation(GetCurvePosition(time / recoilTime));
        up = Quaternion.AngleAxis(angle, core.forward) * head.up;
        forward = RotateVecAroundPoint(GetCurveTangent(time / recoilTime), Quaternion.AngleAxis(core.transform.eulerAngles.y, Vector3.up), Vector3.zero );
            
        HandIK.rotation = Quaternion.LookRotation(up, forward);
        
        return time / recoilTime <= 0;
    }
    
    #endregion
    
    #region Block
    
    public void ChargeBlock(float time) // Go from neutral to P0
    {
        HandIK.position = Shoulder.position + P0;
        
    }
    
    public void Block(float time) // Stay at P0
    {
        
    }
    
    public void ReturnBlock(float time) // Go back to Neutral from P3
    {
        
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
}
