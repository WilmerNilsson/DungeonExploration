using System.Collections;
using UnityEngine;

[System.Serializable]
public class MadAdventurerAttackState : MadAventurerBaseState
{
    [SerializeField, Tooltip("How long it should wait hold attacks"), Min(0f)] public float holdTime;
    [SerializeField, Tooltip("max distance to target before Chasing"), Min(1f)] public float maxMeleeRange = 2.5f;
    [SerializeField, Tooltip("minimum distance to target"), Min(0f)] private float minMeleeRange = 0.5f;

    private float distance;
    

    public override void Enter()
    {
        target = MyMadAdventurerStateMachine.PlayerTransform.position;
    }
    
    public override void Update()
    {
        target = MyMadAdventurerStateMachine.PlayerTransform.position;
        MyMadAdventurerStateMachine.Controller.Rotate(Quaternion.LookRotation((target-MyMadAdventurerStateMachine.transform.position)));
        
        distance = Vector3.Distance(MyMadAdventurerStateMachine.transform.position, MyMadAdventurerStateMachine.PlayerTransform.position);
        
        if (distance > maxMeleeRange)
        {
            MyMadAdventurerStateMachine.Transit(MyMadAdventurerStateMachine.ChasingState);
        }
        else if (distance > minMeleeRange && !MyMadAdventurerStateMachine.isAttacking) // just smack :)
        {
            MyMadAdventurerStateMachine.Attack();
        }
        Stop();
    }
}
