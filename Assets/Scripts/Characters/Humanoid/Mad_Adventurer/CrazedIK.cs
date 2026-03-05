// using System;
// using UnityEngine;
// using Random = UnityEngine.Random;
//
// public class CrazedIK : HumanoidIK
// {
//     private float interruptedSwingTime;
//     private Vector3 returnTarget;
//     public override void Attack(float angle = 0)
//     {
//         if (weapon == null)
//         {
//             Debug.LogWarning("No weapon to attack with", this);
//             return;
//         }
//         if (!attacking)
//         {
//             yRot = 0;
//             swingAngle = Mathf.Deg2Rad * (Random.Range(angleLimit, 360 - angleLimit) - 90);
//             swingStart = shoulderObj.localPosition + (new Vector3(armLenght * Mathf.Cos(swingAngle), armLenght * Mathf.Sin(swingAngle), 0).normalized * armLenght) - transform.forward;
//             swingEnd = shoulderObj.localPosition - (new Vector3(armLenght * Mathf.Cos(swingAngle), armLenght * Mathf.Sin(swingAngle), -1).normalized * armLenght) - transform.forward;
//             returnTarget = swingEnd;
//             attacking = true;
//         }
//     }
//
//     public void Interrupt()
//     {
//         Debug.Log("Interrupt");
//         interruptedSwingTime = time;
//         returnTarget = swingStart;
//         ChangeAttackState(AttackState.Interrupt);
//         weapon.SetActive(false);
//     }
//
//     //a callback for calculating IK
//     protected override void OnAnimatorIK(int layerIndex)
//     {
//         if(animator) {
//             if(lookObj != null) {
//                 animator.SetLookAtWeight(1);
//                 animator.SetLookAtPosition(lookObj.position);
//             }
//             if(attacking) {
//                 zRot = (Mathf.Atan2(swingStart.y - shoulderObj.localPosition.y, swingStart.x - shoulderObj.localPosition.x) * Mathf.Rad2Deg) + 180;
//                 rotation = RelativeRotation(Quaternion.AngleAxis(zRot, Vector3.forward) * Quaternion.AngleAxis(yRot, Vector3.up));
//                 
//                 if (startTime == 0) startTime = Time.time;
//                 
//                 if (attackState == AttackState.Start)
//                 {
//                     time = (Time.time - startTime) / chargeTime;
//                     animator.SetIKPositionWeight(AvatarIKGoal.RightHand,Mathf.SmoothStep(0, 1, time));
//                     animator.SetIKRotationWeight(AvatarIKGoal.RightHand,Mathf.Lerp(0, 1, time));
//                     animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(swingStart));
//                     animator.SetIKRotation(AvatarIKGoal.RightHand,rotation);
//                 }
//                 else if (attackState == AttackState.Swing)
//                 {
//                     time = (Time.time - startTime) / nodeTime;
//                     yRot = Mathf.Clamp(Mathf.SmoothStep(0, 160, time), 0, 160);
//                     animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
//                     animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);
//                     animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(GetCurvePosition(time)));
//                     animator.SetIKRotation(AvatarIKGoal.RightHand,rotation);
//                 }
//                 else if (attackState == AttackState.Return)
//                 {
//                     time = (Time.time - startTime) / resetTime;
//                     animator.SetIKPositionWeight(AvatarIKGoal.RightHand,Mathf.SmoothStep(1, 0, time));
//                     animator.SetIKRotationWeight(AvatarIKGoal.RightHand,Mathf.Lerp(1, 0, time));
//                     animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(returnTarget));
//                     animator.SetIKRotation(AvatarIKGoal.RightHand,rotation);
//                 }
//                 else if (attackState == AttackState.Interrupt)
//                 {
//                     time = interruptedSwingTime - (Time.time - startTime) / recoilTime;
//                     yRot = Mathf.Clamp(Mathf.SmoothStep(0, 160, time), 0, 160);
//                     animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
//                     animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);
//                     animator.SetIKPosition(AvatarIKGoal.RightHand,RelativePosition(GetCurvePosition(time)));
//                     animator.SetIKRotation(AvatarIKGoal.RightHand,rotation);
//                 }
//                 
//                 if (time > 1f || (attackState == AttackState.Interrupt && time < 0)) //if it's near the goal switch goal or end the attack
//                 {
//                     if (attackState == AttackState.Interrupt)
//                     {
//                         weapon.SetActive(false);
//                         ChangeAttackState(AttackState.Return);
//                     }
//                     else
//                     {
//                         if (attackState == AttackState.Start) // Start Swing
//                         {
//                             weapon.SetActive(true);
//                             ChangeAttackState(AttackState.Swing);
//                         }
//                         else if (attackState == AttackState.Swing) // return
//                         {
//                             weapon.SetActive(false);
//                             ChangeAttackState(AttackState.Return);
//                         }
//                         else if (attackState == AttackState.Return)// stop
//                         {
//                             attacking = false;
//                             Reset();
//                         }
//                     }
//                     startTime = Time.time;
//                 }
//             }
//             else {          
//                 animator.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
//                 animator.SetIKRotationWeight(AvatarIKGoal.RightHand,0);
//                 Reset();
//             }
//         }
//     }
// }
