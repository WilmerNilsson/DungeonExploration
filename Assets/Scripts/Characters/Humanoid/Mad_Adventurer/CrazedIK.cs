using System;
using UnityEngine;

public class CrazedIK : MonoBehaviour
{
    public Animator animator;
    public Avatar avatar;

    public Transform handObj;
    public Transform lookObj = null;
    public Transform Start = null;
    public Transform End = null;
    private Transform IKtarget = null;
    
    [SerializeField] private float lerpSpeed = 1.0f;

    private float animationWeight;
    
    [SerializeField] private Node[] nodes;
    private int currentNode = 0;

    
    [SerializeField] private attackPhase currentPhase = attackPhase.Neutral;
    private enum attackPhase
    {
        Neutral,
        Start,
        Swing,
        Reset
    }
    
    //a callback for calculating IK
    void OnAnimatorIK(int layerIndex)
    {
        if(animator) {
       
            //if the IK is active, set the position and rotation directly to the goal.
            if(animator.GetBool("Attack")) {
                if (currentPhase == attackPhase.Neutral)
                {
                    currentPhase = attackPhase.Start;
                }

                // Set the look target position, if one has been assigned
                if(lookObj != null) {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if(currentPhase != attackPhase.Neutral) {
                    switch (currentPhase)
                    {
                        case attackPhase.Start: // Neutral to Start
                            IKtarget = Start;
                            break;
                        case attackPhase.Swing: // Start to End
                            IKtarget = End;
                            break;
                        case attackPhase.Reset: // End to Neutral
                            IKtarget = animator.GetBoneTransform(HumanBodyBones.RightHand);
                            break;
                    }
                    
                    animationWeight = Mathf.SmoothStep(animationWeight, 1, lerpSpeed);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,animationWeight);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,animationWeight);  
                    animator.SetIKPosition(AvatarIKGoal.RightHand,IKtarget.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand,IKtarget.rotation);
                    
                    if (animationWeight > .95f)
                    {
                        animationWeight = 0;
                        switch (currentPhase)
                        {
                            case attackPhase.Start: // Neutral to Start
                                handObj = Start;
                                currentPhase = attackPhase.Swing;
                                break;
                            case attackPhase.Swing: // Start to End
                                currentPhase = attackPhase.Reset;
                                break;
                            case attackPhase.Reset: // End to Neutral
                                currentPhase = attackPhase.Neutral;
                                animator.SetBool("Attack",false);
                                break;
                        }
                    }
                }
            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else {          
                // animator.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
                // animator.SetIKRotationWeight(AvatarIKGoal.RightHand,0);
                // animator.SetLookAtWeight(0);
                // animationWeight = 0;
            }
        }
    }
}

[System.Serializable]
class Node
{
    public Vector3 position;
    public Quaternion rotation;
    
    public Vector3 Position { get { return position; } }
    public Quaternion Rotation { get { return rotation; } }
}
