using System;
using UnityEngine;

public class CrazedIK : MonoBehaviour
{
    [SerializeField] private bool KeyframeAnimation = false;

    public bool attack;
    
    public Animator animator;
    [SerializeField, Tooltip("Where the avatar should look")] private Transform lookObj = null;
    
    [SerializeField, Tooltip("How fast the animation runs")] private float lerpSpeed = 1.0f;
    
    [Header("Hand transform targets")]
    [SerializeField] private Vector3 currentPos;
    [SerializeField] private Quaternion currentRot;
    
    [Header("Transform Animation")]
    [SerializeField, Tooltip("List of Transforms for the animation to run through")] private Transform[] targets;
    private Transform target;
    private int targetIndex = 0;
    
    [Header("Keyframe animation")]
    [SerializeField, Tooltip("List of keyframes for the animation to run through")] private CustomKeyframe[] Keyframes;
    private int currentNode = 0;
    
    //a callback for calculating IK
    void OnAnimatorIK(int layerIndex)
    {
        if(animator) {
            
            if(attack) {
                if(lookObj != null) {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }

                if (KeyframeAnimation)
                {
                    currentPos = Vector3.Slerp(currentPos, transform.TransformDirection(Keyframes[currentNode].Position) + transform.position , lerpSpeed);
                    currentRot = Quaternion.Slerp(currentRot, Keyframes[currentNode].Rotation, lerpSpeed);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);  
                    animator.SetIKPosition(AvatarIKGoal.RightHand,currentPos);
                    animator.SetIKRotation(AvatarIKGoal.RightHand,currentRot);

                    if (Vector3.Distance(currentPos, Keyframes[currentNode].Position) < 0.1f)
                    {
                        currentNode = currentNode + 1;
                        if (currentNode >= Keyframes.Length)
                        {
                            currentNode = 0;
                            animator.SetBool("Attack",false);
                        }
                    }
                }
                else
                {
                    if (target == null)
                    {
                        targetIndex = 0;
                        target = targets[0];
                    }
                    
                    if(target != null) {
                        
                        currentPos = Vector3.Slerp(currentPos, target.position, lerpSpeed);
                        currentRot = Quaternion.Slerp(currentRot, target.rotation, lerpSpeed);
                        
                        animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
                        animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);  
                        animator.SetIKPosition(AvatarIKGoal.RightHand,currentPos);
                        animator.SetIKRotation(AvatarIKGoal.RightHand,currentRot);
                        
                        if (Vector3.Distance(currentPos, target.position) < 0.1f)
                        {
                            targetIndex++;
                            if (targetIndex >= targets.Length)
                            {
                                targetIndex = 0;
                                target = null;
                                animator.SetBool("Attack",false);
                            }
                            else
                            {
                                target = targets[targetIndex];
                            }
                        }
                    }
                }
                
            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else {          
                currentNode = 0;
                targetIndex = 0;
                // animator.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
                // animator.SetIKRotationWeight(AvatarIKGoal.RightHand,0);
                // animator.SetLookAtWeight(0);
                // animationWeight = 0;
            }
        }
    }
}

[System.Serializable]
class CustomKeyframe
{
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 rotation;
    
    public Vector3 Position { get { return position; } }
    public Quaternion Rotation { get { return Quaternion.Euler(rotation); } }
}
