using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Sword : Weapon
{
    [Header("Sword")] 
    [SerializeField] private bool attacking = false;
    [SerializeField] private TwoBoneIKConstraint swordArm; 
    [SerializeField] private Transform core;
    [SerializeField] private Transform headObj;
    [SerializeField] private float curveHeight;
    [SerializeField, Tooltip("Attacks per Second")] private float attackSpeed;
    [SerializeField] private float rightRotation;

    private float time;
    private Vector3 swingStart => core.position + (core.right + Vector3.up).normalized;
    private Vector3 swingEnd => core.position - (core.right + Vector3.up - core.forward).normalized;
    
    private Vector3 heightPoint => swingStart + (swingEnd - swingStart) / 2 + headObj.forward * curveHeight;
    
    private Transform hand => swordArm.data.target;
    private Transform shoulder => swordArm.data.root;
    private Vector3 ShoulderToHand => hand.position - shoulder.position;

    // Update is called once per frame
    void Update()
    {
        if (attacking)
        {
            if (time >= attackSpeed) time = 0;
            time += Time.deltaTime;
            hand.position = GetCurvePosition(time / attackSpeed);
            hand.right = -ShoulderToHand;
        }
        else
        {
            time = 0;
        }
    }
    
    private Vector3 GetCurvePosition(float t)
    {
        return (Mathf.Pow(1 - t, 2) * swingStart) + (2 * (1 - t) * t * heightPoint) + (t * t * swingEnd);
    }

    private Vector3 GetCurveTangent(float t)
    {
        Vector3 heighPoint = swingStart + (swingEnd - swingStart) / 2 + headObj.forward * curveHeight;
        Vector3 tangent = 2*(1-t) * (heighPoint-swingStart) + 2*t*(swingEnd-heighPoint);
        return tangent.normalized;
    }
    
    private Vector3 GetCurveNormal(float t)
    {
        Vector3 cross = Vector3.Cross(GetCurveTangent(t), heightPoint);
        Vector3 normal = Vector3.Cross(cross, GetCurveTangent(t));
        return normal.normalized;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(swingStart, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(heightPoint, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(swingEnd, 0.1f);
        
        Gizmos.color = Color.green;
        Gizmos.DrawRay(hand.position, ShoulderToHand);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(hand.position, -hand.right);
        Gizmos.color = Color.blue;
        if (attacking) Gizmos.DrawRay(hand.position, GetCurveNormal(time));
    }
}
