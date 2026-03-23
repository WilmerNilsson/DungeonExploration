using System.Collections;
using UnityEngine;

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
            if (!DetectPlayer())
            {
                MyMadAdventurerStateMachine.Transit(MyMadAdventurerStateMachine.IdleState);
            }
        
            if (Vector3.Distance(MyMadAdventurerStateMachine.PlayerTransform.position, MyMadAdventurerStateMachine.transform.position) <= minDistanceToPlayer)
            {
                MyMadAdventurerStateMachine.Transit(MyMadAdventurerStateMachine.AttackState);
            }
            else
            {
                FindPath(MyMadAdventurerStateMachine.PlayerTransform.position);
                target = GetNextCorner();
                Move();
            }
        }
        else if (chasingDelay > 0)
        {
            chasingDelay -= Time.deltaTime;
            Stop();
            MyMadAdventurerStateMachine.Controller.Rotate(Quaternion.LookRotation((target-MyMadAdventurerStateMachine.transform.position)));
        }
        else
        {
            isChasing = true;
            FindPath(MyMadAdventurerStateMachine.PlayerTransform.position);
            target = GetNextCorner();
        }
    }
}
