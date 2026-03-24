using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class MadAdventurerChasingState : MadAventurerBaseState
{
    [SerializeField] private float minDistanceToPlayer;
    [SerializeField] private float chasingDelay;
    private bool isChasing = false;
    public override void Enter()
    {
        MyMadAdventurerStateMachine.Controller.isSprinting = true;
        target = MyMadAdventurerStateMachine.PlayerTransform.position;
        isChasing = false;
    }

    public override void Exit()
    {
        base.Exit();
        MyMadAdventurerStateMachine.Controller.isSprinting = false;
        isChasing = false;
    }

    public override void Update()
    {
        if (isChasing)
        {
            TryTransit();
            
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
                    Move(Vector3.forward);
                }
            }
            else // Stand and stare at player if you cant reach them
            {
                MyMadAdventurerStateMachine.Controller.Rotate(Quaternion.LookRotation((PlayerPosition-MyPosition)));
                Stop();
            }
        }
        else if (chasingDelay > 0)
        {
            chasingDelay -= Time.deltaTime;
            Stop();
            MyMadAdventurerStateMachine.Controller.Rotate(Quaternion.LookRotation((target-MyPosition)));
        }
        else
        {
            isChasing = true;
        }
    }

    private void TryTransit()
    {
        if (!DetectPlayer()) // If it cant detect the player, move back to Searching
        {
            MyMadAdventurerStateMachine.Transit(MyMadAdventurerStateMachine.SearchingState);
        }
        if (Vector3.Distance(MyMadAdventurerStateMachine.PlayerTransform.position, MyMadAdventurerStateMachine.transform.position) <= minDistanceToPlayer) // If its close enough move to Attacking
        {
            MyMadAdventurerStateMachine.Transit(MyMadAdventurerStateMachine.AttackState);
        }
    }
}
