using UnityEngine;
using UnityEngine.Events;

public class PlayerIK : HumanoidIK
{
    
    
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
            weapon.dealDamage = true;
            swingAngle = Mathf.Deg2Rad * angle;
            swingStart = shoulderObj.localPosition + (new Vector3(armLenght * Mathf.Cos(swingAngle), armLenght * Mathf.Sin(swingAngle), 0).normalized * armLenght);
            swingEnd = shoulderObj.localPosition - (new Vector3(armLenght * Mathf.Cos(swingAngle), armLenght * Mathf.Sin(swingAngle), -1).normalized * armLenght);
            nodes = GetQuadraticBezierPoints(swingStart, swingEnd, curveHeight);
            attacking = true;
        }
    }

    public override void Block(float angle = 0)
    {
        if (weapon == null)
        {
            Debug.LogWarning("No weapon to block with", this);
            return;
        }
        if (!blocking && !attacking)
        {
            Reset();
            blockAngle = Mathf.Deg2Rad * angle;
            blockPos = Vector3.forward + shoulderObj.localPosition + (new Vector3(armLenght * Mathf.Cos(blockAngle), armLenght * Mathf.Sin(blockAngle), 0).normalized * handOffset);
            blocking = true;
        }
    }
    
    protected override void OnAnimatorIK(int layerIndex)
    {
        if(animator) {
            if(attacking && !blocking) {
                
                // Math for rotating the sword arm correctly
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
                    time = (Time.time - startTime) / (nodeTime/100);
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
                
                if (time > 1f) //if it's near the goal switch goal or end the attack
                {
                    if (nodeIndex == 0) // Start Swing
                    {
                        weapon.SetActive(true);
                        current = nodes[nodeIndex];
                        target = nodes[nodeIndex + 1];
                        attackState = AttackState.Swing;
                        onAttackStateChange.Invoke(attackState);
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
                        attackState = AttackState.Return;
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
            else if (blocking)
            {
                if (startTime == 0) startTime = Time.time;
                
                z = (Mathf.Atan2(blockPos.y - shoulderObj.localPosition.y, blockPos.x - shoulderObj.localPosition.x) * Mathf.Rad2Deg) - 90;
                if (Mathf.Abs(blockAngle) > Mathf.PI/2) z += 180;
                
                if (blockState == BlockState.Start)
                {
                    time = (Time.time - startTime) / blockChargeTime;
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,Mathf.SmoothStep(0, 1, time));
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,Mathf.Lerp(0, 1, time));
                    animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(blockPos + blockPosMod));
                    animator.SetIKRotation(AvatarIKGoal.RightHand,RelativeRotation(Quaternion.AngleAxis(z, Vector3.forward)));
                }
                else if (blockState == BlockState.Block)
                {
                    time = (Time.time - startTime) / blockTime;
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(blockPos + blockPosMod));
                    animator.SetIKRotation(AvatarIKGoal.RightHand,RelativeRotation(Quaternion.AngleAxis(z, Vector3.forward)));
                }
                else if (blockState == BlockState.Return)
                {
                    time = (Time.time - startTime) / blockResetTime;
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,Mathf.SmoothStep(1, 0, time));
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,Mathf.Lerp(1, 0, time));
                    animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(blockPos + blockPosMod));
                    animator.SetIKRotation(AvatarIKGoal.RightHand,RelativeRotation(Quaternion.AngleAxis(z, Vector3.forward)));
                }
                
                if (time > 1f) //if it's near the goal switch goal or end the attack
                {
                    if (blockState == BlockState.Start) // Move to Block
                    {
                        weapon.dealDamage = false;
                        weapon.SetActive(true);
                        blockState = BlockState.Block;
                    }
                    else if (blockState == BlockState.Block) // Block
                    {
                        weapon.SetActive(false);
                        weapon.dealDamage = true;
                        blockState = BlockState.Return;
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
        attackState = AttackState.Start;
        blockState = BlockState.Start;
        startTime = 0;
        time = 0;

        current = Vector3.zero;
        target = Vector3.zero;
        nodeIndex = 0;
        
        weapon.SetActive(false);
        weapon.dealDamage = false;
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
