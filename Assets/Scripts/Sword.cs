using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Random = UnityEngine.Random;

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
    private Vector3 swingStart;
    private Vector3 swingEnd;
    
    private Vector3 heightPoint => swingStart + (swingEnd - swingStart) / 2 + headObj.forward * curveHeight;
    
    private Transform hand => swordArm.data.target;
    private Transform shoulder => swordArm.data.root;
    private Vector3 ShoulderToHand => hand.position - shoulder.position;

    // Update is called once per frame
    private void Start()
    {
        RandomiseStartEnd();
    }

    void Update()
    {
        if (attacking)
        {
            if (time >= attackSpeed)
            {
                time = 0;
                RandomiseStartEnd();
            }
            time += Time.deltaTime;
            hand.position = GetCurvePosition(time / attackSpeed);
            Vector3 forward = GetCurveTangent(time / attackSpeed);
            Vector3 upward = GetCurveNormal(time / attackSpeed);
            hand.rotation = Quaternion.LookRotation(forward, upward);
            hand.rotation *= Quaternion.AngleAxis(90, hand.right);
        }
        else
        {
            time = 0;
        }
    }

    private void RandomiseStartEnd()
    {
        float angle = Random.Range(-45, 45);
        Vector3 direction = Quaternion.AngleAxis(angle, core.forward) * core.right;
        swingStart = shoulder.position + (direction - core.forward).normalized;
        swingEnd = shoulder.position - direction.normalized;;
    }
    
    private Quaternion RelativeRotation(Quaternion rotation)
    {
        Vector3 euler = rotation.eulerAngles;
        euler.y += core.eulerAngles.y;
        return Quaternion.Euler(euler);
    }
    
    private Vector3 GetCurvePosition(float t)
    {
        return (Mathf.Pow(1 - t, 2) * swingStart) + (2 * (1 - t) * t * heightPoint) + (t * t * swingEnd);
    }

    private Vector3 GetCurveTangent(float t)
    {
        Vector3 tangent = 2*(1-t) * (heightPoint-swingStart) + 2*t*(swingEnd-heightPoint);
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
        
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(hand.position, -hand.forward);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(hand.position, -hand.right);
        
        if (attacking)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(hand.position, GetCurveTangent(time / attackSpeed));
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(hand.position, GetCurveNormal(time / attackSpeed));
        }
    }
}
