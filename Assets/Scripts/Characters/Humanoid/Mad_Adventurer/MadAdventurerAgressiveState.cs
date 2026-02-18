using UnityEngine;

[System.Serializable]
public class MadAdventurerAgressiveState : MadAventurerBaseState
{
    [SerializeField] private CrazedIK avatarIK;
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
        MyMadAdventurerStateMachine.Controller.Rotate(Quaternion.LookRotation((target-MyMadAdventurerStateMachine.transform.position)+Vector3.up));
        
        distance = Vector3.Distance(MyMadAdventurerStateMachine.transform.position, MyMadAdventurerStateMachine.PlayerTransform.position);
        
        if (distance > maxMeleeRange)
        {
            MyMadAdventurerStateMachine.Transit(MyMadAdventurerStateMachine.ChasingState);
        }
        else if (distance > minMeleeRange) // just smack :)
        {
            avatarIK.Attack();
        }
        
        Stop();
    }
}
