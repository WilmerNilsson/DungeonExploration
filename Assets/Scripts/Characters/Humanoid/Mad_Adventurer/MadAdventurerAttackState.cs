using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class MadAdventurerAttackState : MadAventurerBaseState
{
    [SerializeField, Tooltip("How long it should wait hold attacks"), Min(0f)] public float holdTime;
    [SerializeField, Tooltip("max distance to target before Chasing"), Min(1f)] public float chaseRange = 4f;
    [SerializeField, Tooltip("max distance to target before moving closer"), Min(1f)] public float maxMeleeRange = 2.5f;
    [SerializeField, Tooltip("minimum distance to target"), Min(0f)] private float minMeleeRange = 0.5f;

    private float distance;
    

    public override void Enter()
    {
        TryFindPath(MyMadAdventurerStateMachine.PlayerTransform.position);
        target = GetNextCorner();
    }
    
    public override void Update()
    {
        if (TryFindPath(MyMadAdventurerStateMachine.PlayerTransform.position))
        {
            if (Vector3.Distance(MyPosition, NavMeshPath.corners[^1]) < 1f)
            {
                MyMadAdventurerStateMachine.Controller.Rotate(Quaternion.LookRotation((PlayerPosition-MyPosition)));
                Stop();
            }
            else
            {
                target = GetNextCorner();
            }
        }
        else // Stand and stare at player if you cant reach them
        {
            MyMadAdventurerStateMachine.Controller.Rotate(Quaternion.LookRotation((PlayerPosition-MyPosition)));
            Stop();
        }
        
        distance = Vector3.Distance(MyPosition, PlayerPosition);

        if (distance > chaseRange)
        {
            MyMadAdventurerStateMachine.Transit(MyMadAdventurerStateMachine.ChasingState);
        }
        else if (distance > maxMeleeRange)
        {
            Move(Vector3.forward);
        }
        else if (distance > minMeleeRange) // just smack :)
        {
            Stop();
            MyMadAdventurerStateMachine.Controller.Rotate(Quaternion.LookRotation((PlayerPosition-MyPosition)));
            TryAttack();
        }
        else
        {
            Move(Vector3.back);
        }
    }

    private bool TryAttack()
    {
        if (MyMadAdventurerStateMachine.isAttacking)
        {
            return false;
        }
        else
        {
            MyMadAdventurerStateMachine.Attack();
            return true;
        }
    }
}
