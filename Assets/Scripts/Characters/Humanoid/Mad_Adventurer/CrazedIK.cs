using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrazedIK : HumanoidIK
{
    public override void Attack(float angle = 0)
    {
        if (weapon == null)
        {
            Debug.LogWarning("No weapon to attack with", this);
            return;
        }
        if (!attacking)
        {
            swingAngle = Mathf.Deg2Rad * (Random.Range(angleLimit, 360 - angleLimit) - 90);
            swingStart = shoulderObj.localPosition + (new Vector3(armLenght * Mathf.Cos(swingAngle), armLenght * Mathf.Sin(swingAngle), 0).normalized * armLenght);
            swingEnd = shoulderObj.localPosition - (new Vector3(armLenght * Mathf.Cos(swingAngle), armLenght * Mathf.Sin(swingAngle), -1).normalized * armLenght);
            nodes = GetQuadraticBezierPoints(swingStart, swingEnd, curveHeight);
            attacking = true;
        }
    }

    public void Interrupt()
    {
        Debug.Log("Interrupt");
        ChangeAttackState(AttackState.Interrupt);
        weapon.SetActive(false);
    }

    //a callback for calculating IK
    protected override void OnAnimatorIK(int layerIndex)
    {
        if(animator) {
            if(attacking) {
                
                if(lookObj != null) {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }
                
                z = (Mathf.Atan2(swingStart.y - shoulderObj.localPosition.y, swingStart.x - shoulderObj.localPosition.x) * Mathf.Rad2Deg) + 180;
                x = Mathf.Clamp(Mathf.SmoothStep(0,90,(float)nodeIndex/100), 0, 90);
                rotation = RelativeRotation(Quaternion.AngleAxis(z, Vector3.forward) * Quaternion.AngleAxis(x, Vector3.up));
                
                if (current == Vector3.zero) target = nodes[0];
                if (startTime == 0) startTime = Time.time;
                
                if (attackState == AttackState.Start)
                {
                    time = (Time.time - startTime) / chargeTime;
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,Mathf.SmoothStep(0, 1, time));
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,Mathf.Lerp(0, 1, time));
                    animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(target));
                    animator.SetIKRotation(AvatarIKGoal.RightHand,rotation);
                }
                else if (attackState == AttackState.Swing)
                {
                    time = (Time.time - startTime) / (nodeTime/100f);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(Vector3.Slerp(current, target, time)));
                    animator.SetIKRotation(AvatarIKGoal.RightHand,rotation);
                }
                else if (attackState == AttackState.Return)
                {
                    time = (Time.time - startTime) / resetTime;
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,Mathf.SmoothStep(1, 0, time));
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,Mathf.Lerp(1, 0, time));
                    animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(current));
                    animator.SetIKRotation(AvatarIKGoal.RightHand,rotation);
                }
                else if (attackState == AttackState.Interrupt)
                {
                    time = (Time.time - startTime) / (staggerTime/100);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(Vector3.Slerp(current, target, time)));
                    animator.SetIKRotation(AvatarIKGoal.RightHand,rotation);
                }
                
                if (time > 1f) //if it's near the goal switch goal or end the attack
                {
                    if (attackState == AttackState.Interrupt)
                    {
                        if (nodeIndex > 0)
                        {
                            current = nodes[nodeIndex];
                            target = nodes[nodeIndex - 1];
                            nodeIndex--;
                        }
                        else if (nodeIndex == 0)
                        {
                            attacking = false;
                            Reset();
                        }
                    }
                    else
                    {
                        if (nodeIndex == 0) // Start Swing
                        {
                            current = nodes[nodeIndex];
                            target = nodes[nodeIndex + 1];
                            ChangeAttackState(AttackState.Swing);
                            weapon.SetActive(true);
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
                            weapon.SetActive(false);
                            current = nodes[nodeIndex];
                            ChangeAttackState(AttackState.Return);
                            nodeIndex++;
                        }
                        else // stop
                        {
                            nodeIndex = 0;
                            attacking = false;
                            Reset();
                        }
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
}

[System.Serializable]
class CustomKeyframe
{
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private float time;
    
    public Vector3 Position { get { return position; } }
    public Quaternion Rotation { get { return Quaternion.Euler(rotation); } }
    public float Time { get { return time; } }
}
