using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerIK : HumanoidIK
{
    [SerializeField] AnimationCurve curve;
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
            yRot = 0;
            weapon.dealDamage = true;
            swingAngle = Mathf.Deg2Rad * angle;
            float x = Mathf.Cos(swingAngle) + .1f;
            float y = Mathf.Sin(swingAngle);
            swingStart = new Vector3(x, y, 0) * armLenght;
            swingEnd = new Vector3(-x, -y, 0) * armLenght;
            
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
            if (angle < 0) offsetPos.x++;
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
                zRot = (Mathf.Atan2(swingStart.y, swingStart.x) * Mathf.Rad2Deg) + 180;
                xRot = Vector3.SignedAngle(transform.forward, headObj.forward, Vector3.left);
                rotation = RelativeRotation(Quaternion.AngleAxis(zRot, Vector3.forward) * Quaternion.AngleAxis(yRot, Vector3.up) * Quaternion.AngleAxis(xRot, Vector3.right));
                
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
                    yRot = Mathf.Clamp(Mathf.SmoothStep(0, 160, time), 0, 160);
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
                
                zRot = blockAngle;
                if (zRot > 0) zRot += 180;
                
                blockPos = headObj.forward + headObj.position + transform.TransformDirection(anglePos + offsetPos);
                
                Quaternion blockRotation = Quaternion.AngleAxis(zRot, Vector3.forward);
                
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
        if (attackState == AttackState.Swing)
        {
            Vector3 pos = RelativePosition(GetCurvePosition(time));
            Gizmos.DrawSphere(pos, 0.1f);
            Gizmos.DrawRay(pos, GetCurveNormal(time));
        }
        
    }
}
