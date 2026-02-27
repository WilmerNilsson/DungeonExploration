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
    [SerializeField, Tooltip("distance from middle toward start"), Range(0,1)] private float startBend;
    [SerializeField, Tooltip("distance from middle toward end"), Range(0,1)] private float endBend;
    [SerializeField, Tooltip("Attacks per Second")] private float attackSpeed;
    [SerializeField] private float angle;
    [SerializeField] private bool x, y, z;
    [SerializeField, Range(-360,360)] float xMod, yMod, zMod;

    private float time;
    private Vector3 swingStart;
    private Vector3 swingEnd;
    private Vector3 direction;
    
    private Vector3 heightPoint => swingStart + (swingEnd - swingStart) / 2 + headObj.forward * curveHeight;
    
    private Transform hand => swordArm.data.target;
    private Transform shoulder => swordArm.data.root;
    private Transform arm => swordArm.data.mid;
    private Vector3 ShoulderToHand => hand.position - shoulder.position;

    private Vector3 P0 => Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
    private Vector3 P1 => Vector3.Lerp(P0,P3,startBend)+ Vector3.forward * curveHeight;
    private Vector3 P2 => Vector3.Lerp(P0,P3,endBend)+ Vector3.forward * curveHeight;
    private Vector3 P3 => Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.left + Vector3.forward;
    // Update is called once per frame
    private void Start()
    {
        RandomiseStartEnd();
    }

    void Update()
    {
        if (attacking)
        {
            swordArm.data.targetPositionWeight = 1;
            swordArm.data.targetRotationWeight = 1;
            if (time >= attackSpeed)
            {
                time = 0;
                RandomiseStartEnd();
            }
            time += Time.deltaTime;
            Vector3 position = RotateVecAroundPoint(GetCurvePosition(time / attackSpeed), Quaternion.AngleAxis(core.transform.eulerAngles.y, Vector3.up), Vector3.zero );
            hand.position = shoulder.position + position;
            
            Vector3 forward = GetCurveTangent(time / attackSpeed);
            Vector3 upward = GetCurveNormal(time / attackSpeed);
            
            
            float xRot = x ? Vector3.Angle(core.forward, headObj.forward) + xMod: 0;
            float yRot = y ? core.transform.eulerAngles.y + yMod: 0;
            float zRot = z ? angle + zMod: 0;
            //Quaternion rotation = Quaternion.AngleAxis(zRot, core.forward) * Quaternion.AngleAxis(yRot, headObj.up) * Quaternion.AngleAxis(xRot, core.right);
            //hand.rotation = rotation;
            
            
            //hand.up = position;
            //hand.localRotation = Quaternion.LookRotation(Quaternion.AngleAxis(angle, core.forward) * Vector3.up,position);
            hand.localRotation = Quaternion.Euler(xRot,yRot,zRot);
        }
        else
        {
            time = 0;
            swordArm.data.targetPositionWeight = 0;
            swordArm.data.targetRotationWeight = 0;
        }
    }

    private void RandomiseStartEnd()
    {
        //angle = Random.Range(-45, 45);
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
        Vector3 cross = Vector3.Cross(GetCurveTangent(t), GetCurvePosition(t));
        Vector3 normal = Vector3.Cross(cross, GetCurveTangent(t));
        return normal.normalized;
    }
    
    private Vector3 RotateVecAroundPoint(Vector3 vector, Quaternion rotation, Vector3 point)
    {
        return rotation * (vector - point) + point;
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
