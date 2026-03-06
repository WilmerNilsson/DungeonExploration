using UnityEngine;

[System.Serializable]
public class MadAdventurerChasingState : MadAventurerBaseState
{
    [SerializeField] private float minDistanceToPlayer;
    public override void Enter()
    {
        MyMadAdventurerStateMachine.Controller.isSprinting = true;
        FindPath(MyMadAdventurerStateMachine.PlayerTransform.position);
        target = GetNextCorner();
    }

    public override void Exit()
    {
        base.Exit();
        MyMadAdventurerStateMachine.Controller.isSprinting = false;
    }

    public override void Update()
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
}
