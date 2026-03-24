using UnityEngine;

[System.Serializable]
public class MadAdventurerSearchingState : MadAventurerBaseState
{
    [SerializeField] private float searchDuration;
    private float searchTimer;

    public override void Enter()
    {
        base.Enter();
        searchTimer = searchDuration;
    }

    public override void Exit()
    {
        base.Exit();
        searchTimer = 0;
    }

    public override void Update()
    {
        Stop();
        if (searchTimer > 0) // TODO make it rotate randomly
        {
            searchTimer -= Time.deltaTime;
            if (DetectPlayer())
            {
                MyMadAdventurerStateMachine.Transit(MyMadAdventurerStateMachine.ChasingState);
            }
        }
        else
        {
            MyMadAdventurerStateMachine.Transit(MyMadAdventurerStateMachine.IdleState);
        }
    }
}
