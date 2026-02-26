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
    [SerializeField, Tooltip("distance from middle toward start")] private float startBend;
    [SerializeField, Tooltip("distance from middle toward end")] private float endBend;
    [SerializeField, Tooltip("Attacks per Second")] private float attackSpeed;
    [SerializeField] private float rightRotation;

    private float time;
    private Vector3 swingStart;
    private Vector3 swingEnd;
    private Vector3 direction;
    
    private Vector3 heightPoint => swingStart + (swingEnd - swingStart) / 2 + headObj.forward * curveHeight;
    
    private Transform hand => swordArm.data.target;
    private Transform shoulder => swordArm.data.root;
    private Transform arm => swordArm.data.mid;
    private Vector3 ShoulderToHand => hand.position - shoulder.position;

    private Vector3 P0 => shoulder.position + -core.forward;
    private Vector3 P1 => shoulder.position + swingStart + (swingEnd - swingStart) * (.5f - startBend) + headObj.forward * curveHeight;
    private Vector3 P2 => shoulder.position + swingStart + (swingEnd - swingStart) * (.5f + endBend) + headObj.forward * curveHeight;
    private Vector3 P3 => shoulder.position - core.right;
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
            hand.rotation = RelativeRotation(Quaternion.LookRotation(forward, arm.up));
            //hand.rotation *= Quaternion.AngleAxis(90, hand.right);
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
        swingEnd = -direction.normalized;
    }
    
    private Quaternion RelativeRotation(Quaternion rotation)
    {
        Vector3 euler = rotation.eulerAngles;
        euler.y += core.eulerAngles.y;
        return Quaternion.Euler(euler);
    }
    
    private Vector3 GetCurvePosition(float t)
    {
        Vector3 cubic = (Mathf.Pow(1 - t, 3) * P0) + 3 * Mathf.Pow(1 - t, 2)*t*P1 + 3*(1-t)*t*t * P2 + t*t*t * P3;
        return cubic;
    }

    private Vector3 GetCurveTangent(float t)
    {
        Vector3 tangent = (-3 * Mathf.Pow(1 - t, 2) * P0) + (3 * Mathf.Pow(1 - t, 2) * P1) - (6 * t * (1 - t) * P1) - (3 * Mathf.Pow(t, 2) * P2 + 6 * t * (1 - t) * P2) + (3 * Mathf.Pow(t, 2) * P3);
        return tangent.normalized;
    }
    
    private Vector3 GetCurveNormal(float t) // Doesnt really work atm
    {
        Vector3 cross = Vector3.Cross(GetCurveTangent(t+0.0001f), GetCurveTangent(t));
        Vector3 normal = Vector3.Cross(cross, GetCurveTangent(t));
        return normal.normalized;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(Vector3.zero, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(P0, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(P1, 0.1f);
        Gizmos.DrawSphere(P2, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(P3, 0.1f);
        
        
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
