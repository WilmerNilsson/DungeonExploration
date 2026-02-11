using System;
using UnityEngine;

public class CrazedIK : MonoBehaviour
{
    public bool attack;
    
    public Animator animator;
    [SerializeField, Tooltip("Where the avatar should look")] private Transform lookObj = null;
    
    [Header("Keyframe animation")]
    [SerializeField] private AttackState currentState = AttackState.Start;
    [SerializeField, Tooltip("List of keyframes for the animation to run through")] private CustomKeyframe[] Keyframes;
    [SerializeField] private CustomKeyframe targetKeyframe;
    private CustomKeyframe currentKeyframe;
    [SerializeField] private int currentKeyIndex = 0;
    
    private float startTime = 0;
    private float time = 0;
    
    

    private enum AttackState
    {
        Start,
        Swing,
        Return
    }
    
    //a callback for calculating IK
    void OnAnimatorIK(int layerIndex)
    {
        if(animator) {
            
            if(animator.GetBool("Attack")) {
                if(lookObj != null) {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }
                
                if (currentKeyframe == null) targetKeyframe = Keyframes[0];
                if (startTime == 0) startTime = Time.time;
                
                time = (Time.time - startTime) / targetKeyframe.Duration;

                if (currentState == AttackState.Start)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,Mathf.SmoothStep(0, 1, time));
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,Mathf.Lerp(0, 1, time));
                    animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(targetKeyframe.Position));
                    animator.SetIKRotation(AvatarIKGoal.RightHand,RelativeRotation(targetKeyframe.Rotation));
                }
                else if (currentState == AttackState.Swing)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(Vector3.Slerp(currentKeyframe.Position, targetKeyframe.Position, time)));
                    animator.SetIKRotation(AvatarIKGoal.RightHand,RelativeRotation(Quaternion.Slerp(currentKeyframe.Rotation, targetKeyframe.Rotation, time)));
                }
                else if (currentState == AttackState.Return)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,Mathf.SmoothStep(1, 0, time));
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,Mathf.Lerp(1, 0, time));
                    animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(currentKeyframe.Position));
                    animator.SetIKRotation(AvatarIKGoal.RightHand,RelativeRotation(currentKeyframe.Rotation));
                }

                if (time > 1f) //if its near the goal switch goal or end the attack
                {
                    if (currentKeyIndex == 0) // Start Swing
                    {
                        currentKeyframe = Keyframes[currentKeyIndex];
                        targetKeyframe = Keyframes[currentKeyIndex + 1];
                        currentState = AttackState.Swing;
                        currentKeyIndex++;
                    }
                    else if (currentKeyIndex < Keyframes.Length - 1) // Swing
                    {
                        currentKeyframe = Keyframes[currentKeyIndex];
                        targetKeyframe = Keyframes[currentKeyIndex + 1];
                        currentKeyIndex++;
                    }
                    else if (currentKeyIndex == Keyframes.Length - 1) // return
                    {
                        currentKeyframe = Keyframes[currentKeyIndex];
                        currentState = AttackState.Return;
                        currentKeyIndex++;
                    }
                    else // stop
                    {
                        Debug.Log("Keyframe animation finished");
                        currentKeyIndex = 0;
                        animator.SetBool("Attack",false);
                        attack = false;
                        Reset();
                    }
                    startTime = Time.time;
                }
            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
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
        targetKeyframe = null;
        currentKeyframe = null;
        currentKeyIndex = 0;
        startTime = 0;
        time = 0;
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
