using System;
using UnityEngine;

public class CrazedIK : MonoBehaviour
{
    public bool attack;
    
    public Animator animator;
    [SerializeField, Tooltip("Where the avatar should look")] private Transform lookObj = null;
    
    [Header("Hand transform targets")]
    [SerializeField] private Vector3 currentPos;
    [SerializeField] private Quaternion currentRot;
    
    [Header("Keyframe animation")]
    [SerializeField, Tooltip("List of keyframes for the animation to run through")] private CustomKeyframe[] Keyframes;
    [SerializeField] private CustomKeyframe targetKeyframe;
    private CustomKeyframe currentKeyframe;
    private int currentKeyIndex = 0;
    
    private bool lerpWeight = false;
    private float startTime = 0;
    
    //a callback for calculating IK
    void OnAnimatorIK(int layerIndex)
    {
        if(animator) {
            
            if(attack) {
                if(lookObj != null) {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }
                if (currentKeyframe == null) targetKeyframe = Keyframes[0];
                if (startTime == 0) startTime = Time.time;

                lerpWeight = (currentKeyIndex == 0 || currentKeyIndex == Keyframes.Length - 1);

                if (lerpWeight)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,Mathf.SmoothStep(0, 1, (Time.time - startTime) / targetKeyframe.Duration));
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,Mathf.Lerp(0, 1, (Time.time - startTime) / targetKeyframe.Duration));
                    animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(targetKeyframe.Position));
                    animator.SetIKRotation(AvatarIKGoal.RightHand,RelativeRotation(targetKeyframe.Rotation));
                }
                else
                {
                    currentPos = Vector3.Slerp(currentKeyframe.Position, RelativePosition(targetKeyframe.Position), (Time.time - startTime) / targetKeyframe.Duration);
                    currentRot = Quaternion.Slerp(currentKeyframe.Rotation, RelativeRotation(targetKeyframe.Rotation), (Time.time - startTime) / targetKeyframe.Duration);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand,currentPos);
                    animator.SetIKRotation(AvatarIKGoal.RightHand,currentRot);
                }

                if (((Time.time - startTime) / targetKeyframe.Duration) > 1f) //if its near the goal switch goal or end the attack
                {
                    currentKeyIndex++;
                    startTime = Time.time;
                    if (currentKeyIndex > Keyframes.Length)
                    {
                        currentKeyIndex = 0;
                        animator.SetBool("Attack",false);
                        Reset();
                    }
                    else
                    {
                        currentKeyframe = Keyframes[currentKeyIndex-1];
                        targetKeyframe = Keyframes[currentKeyIndex];
                    }
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
        return rotation * animator.rootRotation;
    }

    private void Reset()
    {
        targetKeyframe = null;
        currentKeyframe = null;
        currentKeyIndex = 0;
        startTime = 0;
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
