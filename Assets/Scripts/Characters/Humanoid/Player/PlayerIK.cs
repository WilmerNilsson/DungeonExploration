using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerIK : HumanoidIK
{

    private Vector3 frontPos;
    private Vector3 anglePos;
    private Vector3 offsetPos;
    public override void Attack(float angle = 0)
    {
        if (weapon == null)
        {
            Debug.LogWarning("No weapon to attack with", this);
            return;
        }
        if (!attacking && !blocking)
        {
            Reset();
            x = 0;
            weapon.dealDamage = true;
            swingAngle = Mathf.Deg2Rad * angle;
            swingStart = shoulderObj.localPosition + (new Vector3(armLenght * Mathf.Cos(swingAngle), armLenght * Mathf.Sin(swingAngle), 0).normalized * armLenght);
            swingEnd = shoulderObj.localPosition - (new Vector3(armLenght * Mathf.Cos(swingAngle), armLenght * Mathf.Sin(swingAngle), -1).normalized * armLenght);
            attacking = true;
        }
    }

    public override void Block(float angle = 0) // Angle is relative to Down so 90 is right, -90 is left, 0 is down etc
    {
        if (weapon == null)
        {
            Debug.LogWarning("No weapon to block with", this);
            return;
        }
        if (!blocking && !attacking)
        {
            Reset();
            blockAngle = angle;
            anglePos = new Vector3(Mathf.Sin(blockAngle * Mathf.Deg2Rad), -Mathf.Cos(blockAngle * Mathf.Deg2Rad), 0) * handOffset;
            offsetPos.x = anglePos.y;
            offsetPos.y = -anglePos.x;
            if (blockAngle < 0)
            {
                offsetPos = -offsetPos;
            }
            blocking = true;
        }
    }
    
    protected override void OnAnimatorIK(int layerIndex)
    {
        if(animator) {
            if(attacking && !blocking) {
                
                // Math for rotating the sword arm correctly
                z = (Mathf.Atan2(swingStart.y - shoulderObj.localPosition.y, swingStart.x - shoulderObj.localPosition.x) * Mathf.Rad2Deg) + 180;
                rotation = RelativeRotation(Quaternion.AngleAxis(z, Vector3.forward) * Quaternion.AngleAxis(x, Vector3.up));
                
                if (startTime == 0) startTime = Time.time;
                
                if (attackState == AttackState.Start)
                {
                    time = (Time.time - startTime) / chargeTime;
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,Mathf.SmoothStep(0, 1, time));
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,Mathf.Lerp(0, 1, time));
                    animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(swingStart));
                    animator.SetIKRotation(AvatarIKGoal.RightHand,rotation);
                }
                else if (attackState == AttackState.Swing)
                {
                    time = (Time.time - startTime) / nodeTime;
                    x = Mathf.Clamp(Mathf.SmoothStep(0, 160, time), 0, 160);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(GetCurvePosition(time)));
                    animator.SetIKRotation(AvatarIKGoal.RightHand,rotation);
                }
                else if (attackState == AttackState.Return)
                {
                    time = (Time.time - startTime) / resetTime;
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,Mathf.SmoothStep(1, 0, time));
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,Mathf.Lerp(1, 0, time));
                    animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(swingEnd));
                    animator.SetIKRotation(AvatarIKGoal.RightHand,rotation);
                }
                
                if (time > 1f) //if it's near the goal switch goal or end the attack
                {
                    if (attackState == AttackState.Start) // Start Swing
                    {
                        weapon.SetActive(true);
                        ChangeAttackState(AttackState.Swing);
                    }
                    else if (attackState == AttackState.Swing) // return
                    {
                        weapon.SetActive(false);
                        ChangeAttackState(AttackState.Return);
                    }
                    else if (attackState == AttackState.Return)// stop
                    {
                        attacking = false;
                        Reset();
                    }
                    startTime = Time.time;
                }
            }
            else if (blocking)
            {
                if (startTime == 0) startTime = Time.time;
                
                z = blockAngle;
                if (z > 0) z += 180;
                
                blockPos = headObj.forward + headObj.position + transform.TransformDirection(anglePos + offsetPos);
                
                Quaternion blockRotation = Quaternion.AngleAxis(z, Vector3.forward);
                
                if (blockState == BlockState.Start)
                {
                    time = (Time.time - startTime) / blockChargeTime;
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,Mathf.SmoothStep(0, 1, time));
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,Mathf.Lerp(0, 1, time));
                    animator.SetIKPosition(AvatarIKGoal.RightHand,blockPos);
                    animator.SetIKRotation(AvatarIKGoal.RightHand,RelativeRotation(blockRotation));
                }
                else if (blockState == BlockState.Block)
                {
                    time = (Time.time - startTime) / blockTime;
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand,blockPos);
                    animator.SetIKRotation(AvatarIKGoal.RightHand,RelativeRotation(blockRotation));
                }
                else if (blockState == BlockState.Return)
                {
                    time = (Time.time - startTime) / blockResetTime;
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,Mathf.SmoothStep(1, 0, time));
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,Mathf.Lerp(1, 0, time));
                    animator.SetIKPosition(AvatarIKGoal.RightHand,blockPos);
                    animator.SetIKRotation(AvatarIKGoal.RightHand,RelativeRotation(blockRotation));
                }
                
                if (time > 1f) //if it's near the goal switch goal or end the attack
                {
                    if (blockState == BlockState.Start) // Move to Block
                    {
                        weapon.dealDamage = false;
                        weapon.SetActive(true);
                        ChangeBlockState(BlockState.Block);
                    }
                    else if (blockState == BlockState.Block) // Block
                    {
                        weapon.SetActive(false);
                        weapon.dealDamage = true;
                        ChangeBlockState(BlockState.Return);
                    }
                    else if (blockState == BlockState.Return) // Return
                    {
                        blocking = false;
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

    protected override void Reset()
    {
        base.Reset();
        weapon.dealDamage = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(blockPos, 0.1f);
    }
}
