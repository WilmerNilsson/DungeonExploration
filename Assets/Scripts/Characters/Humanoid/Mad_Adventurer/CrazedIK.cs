using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrazedIK : MonoBehaviour
{
    private bool attacking = false;
    public Animator animator;
    [SerializeField, Tooltip("Where the avatar should look")] private Transform lookObj = null;
    
    [SerializeField] private AttackState currentState = AttackState.Start;
    
    [Header("Curve animation")]
    [SerializeField, Tooltip("Might use later, shit's cool")] private AnimationCurve curve;
    [SerializeField] private Vector3 startNode;
    [SerializeField] private Vector3 endNode;
    [SerializeField] private float curveHeight;
    [SerializeField] private float nodeDuration;
    [SerializeField] private float chargeDuration;
    [SerializeField] private float resetDuration;
    private Vector3[] nodes;
    private Vector3 current;
    private Vector3 target;
    private int nodeIndex;
    
    private float startTime = 0;
    private float time = 0;
    
    private enum AttackState
    {
        Start,
        Swing,
        Return
    }

    private void Start()
    {
        Attack();
    }

    public void Attack()
    {
        if (!attacking)
        {
            startNode.y = Random.Range(-1f, 2f);
            endNode.y = -startNode.y;
            nodes = GetQuadraticBezierPoints(startNode, endNode, curveHeight);
            attacking = true;
        }
    }

    //a callback for calculating IK
    void OnAnimatorIK(int layerIndex)
    {
        if(animator) {
            
            if(attacking) {
                
                if(lookObj != null) {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }
                
                if (current == Vector3.zero) target = nodes[0];
                if (startTime == 0) startTime = Time.time;
                
                if (currentState == AttackState.Start)
                {
                    time = (Time.time - startTime) / chargeDuration;
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,Mathf.SmoothStep(0, 1, time));
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,Mathf.Lerp(0, 1, time));
                    animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(target));
                    //animator.SetIKRotation(AvatarIKGoal.RightHand,RelativeRotation(targetKeyframe.Rotation));
                }
                else if (currentState == AttackState.Swing)
                {
                    time = (Time.time - startTime) / nodeDuration;
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(Vector3.Slerp(current, target, time)));
                    //animator.SetIKRotation(AvatarIKGoal.RightHand,RelativeRotation(Quaternion.Slerp(currentKeyframe.Rotation, targetKeyframe.Rotation, time)));
                }
                else if (currentState == AttackState.Return)
                {
                    time = (Time.time - startTime) / resetDuration;
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,Mathf.SmoothStep(1, 0, time));
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,Mathf.Lerp(1, 0, time));
                    animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(current));
                    //animator.SetIKRotation(AvatarIKGoal.RightHand,RelativeRotation(currentKeyframe.Rotation));
                }
                
                if (time > 1f) //if it's near the goal switch goal or end the attack
                {
                    if (nodeIndex == 0) // Start Swing
                    {
                        current = nodes[nodeIndex];
                        target = nodes[nodeIndex + 1];
                        currentState = AttackState.Swing;
                        nodeIndex++;
                    }
                    else if (nodeIndex < nodes.Length - 1) // Swing
                    {
                        current = nodes[nodeIndex];
                        target = nodes[nodeIndex + 1];
                        nodeIndex++;
                    }
                    else if (nodeIndex == nodes.Length - 1) // return
                    {
                        current = nodes[nodeIndex];
                        currentState = AttackState.Return;
                        nodeIndex++;
                    }
                    else // stop
                    {
                        Debug.Log("Keyframe animation finished");
                        nodeIndex = 0;
                        attacking = false;
                        Reset();
                    }
                    startTime = Time.time;
                }
            }
            else {          
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand,0);
                animator.SetLookAtWeight(0);
                Reset();
            }
        }
    }

    private Vector3 RelativePosition(Vector3 position)
    {
        return transform.TransformDirection(position) + animator.rootPosition;
    }

    private Quaternion RelativeRotation(Quaternion rotation)
    {
        Vector3 euler = rotation.eulerAngles;
        euler.y += transform.parent.eulerAngles.y;
        return Quaternion.Euler(euler);
    }

    private void Reset()
    {
        currentState = AttackState.Start;
        startTime = 0;
        time = 0;

        current = Vector3.zero;
        target = Vector3.zero;
        nodeIndex = 0;
    }
    
    public static Vector3[] GetQuadraticBezierPoints(Vector3 startpoint, Vector3 endPoint, float curveHeigh) {
        Vector3 heighPoint = startpoint + (endPoint - startpoint) / 2 + Vector3.forward * curveHeigh;

        Vector3[] res = new Vector3[100];
        int maxT = 1;
        int index = 0;

        for (float t = 0; t <= maxT; t += 0.01f) {
            Vector3 newPoint = (Mathf.Pow(1 - t, 2) * startpoint) + (2 * (1 - t) * t * heighPoint) + (t * t * endPoint);
            try {
                res[index++] = newPoint;
            }
            catch {
                break;
            }
        }
        return res;
    }

    private void OnDrawGizmos()
    {
        foreach (var node in nodes)
        {
            Gizmos.DrawSphere(RelativePosition(node), 0.1f);
        }
    }
}

[System.Serializable]
class CustomKeyframe
{
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private float duration;
    
    public Vector3 Position { get { return position; } }
    public Quaternion Rotation { get { return Quaternion.Euler(rotation); } }
    public float Duration { get { return duration; } }
}
