using UnityEngine;

public class PlayerIK : MonoBehaviour
{
    [SerializeField,Tooltip("the rotation of the hand, readonly as they are set in code")] private float x, z;
    
    [SerializeField] private bool attacking = false;
    public Animator animator;
    [SerializeField, Tooltip("The Shoulder node of the weapon arm")] private Transform shoulderObj = null;
    [SerializeField] private AttackState currentState = AttackState.Start;
    
    [Header("Attack Settings")]
    [SerializeField, Tooltip("the min angle between the attack and straight up/down")] private float angleLimit = 30f;
    [SerializeField, Tooltip("the lenght of the weapon arm")] private float armLenght = 2f;
    
    [Header("Curve animation")]
    [SerializeField, Tooltip("Might use later, shit's cool")] private AnimationCurve curve;
    [SerializeField] private Vector3 swingStart;
    [SerializeField] private Vector3 swingEnd;
    [SerializeField] private float swingAngle;
    [SerializeField] private float curveHeight = 2.5f;
    [SerializeField] private float nodeDuration = 0.5f;
    [SerializeField] private float chargeDuration = 2f;
    [SerializeField] private float resetDuration = 1f;
    private Vector3[] nodes;
    private Vector3 current;
    private Vector3 target;
    private int nodeIndex;
    private Quaternion rotation;
    
    private float startTime = 0;
    private float time = 0;
    
    private enum AttackState
    {
        Start,
        Swing,
        Return
    }
    
    public void Attack(float angle)
    {
        if (!attacking)
        {
            swingAngle = Mathf.Deg2Rad * angle;
            swingStart = shoulderObj.localPosition + (new Vector3(armLenght * Mathf.Cos(swingAngle), armLenght * Mathf.Sin(swingAngle), 0).normalized * armLenght);
            swingEnd = shoulderObj.localPosition - (new Vector3(armLenght * Mathf.Cos(swingAngle), armLenght * Mathf.Sin(swingAngle), -1).normalized * armLenght);
            nodes = GetQuadraticBezierPoints(swingStart, swingEnd, curveHeight);
            attacking = true;
        }
    }
    
    void OnAnimatorIK(int layerIndex)
    {
        if(animator) {
            if(attacking) {
                
                // Math for rotating the sword arm correctly
                z = (Mathf.Atan2(swingStart.y - shoulderObj.localPosition.y, swingStart.x - shoulderObj.localPosition.x) * Mathf.Rad2Deg) + 180;
                x = Mathf.Clamp(Mathf.SmoothStep(0,90,(float)nodeIndex/100), 0, 90);
                rotation = RelativeRotation(Quaternion.AngleAxis(z, Vector3.forward) * Quaternion.AngleAxis(x, Vector3.up));
                
                if (current == Vector3.zero) target = nodes[0];
                if (startTime == 0) startTime = Time.time;
                
                if (currentState == AttackState.Start)
                {
                    time = (Time.time - startTime) / chargeDuration;
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,Mathf.SmoothStep(0, 1, time));
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,Mathf.Lerp(0, 1, time));
                    animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(target));
                    animator.SetIKRotation(AvatarIKGoal.RightHand,rotation);
                }
                else if (currentState == AttackState.Swing)
                {
                    time = (Time.time - startTime) / (nodeDuration/100);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(Vector3.Slerp(current, target, time)));
                    animator.SetIKRotation(AvatarIKGoal.RightHand,rotation);
                }
                else if (currentState == AttackState.Return)
                {
                    time = (Time.time - startTime) / resetDuration;
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,Mathf.SmoothStep(1, 0, time));
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,Mathf.Lerp(1, 0, time));
                    animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(current));
                    animator.SetIKRotation(AvatarIKGoal.RightHand,rotation);
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
    
    private static Vector3[] GetQuadraticBezierPoints(Vector3 startpoint, Vector3 endPoint, float curveHeigh) {
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
}
