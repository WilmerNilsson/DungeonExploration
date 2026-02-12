using UnityEngine;

[System.Serializable]
public class MadMelee : MadState
{
    [SerializeField] private CrazedIK avatarIK;
    [SerializeField, Tooltip("max distance to target before Chasing"), Min(1f)] public float maxMeleeRange = 2.5f;
    [SerializeField, Tooltip("minimum distance to target"), Min(0f)] private float minMeleeRange = 0.5f;

    private float distance;
    public override void Enter()
    {
        base.Enter();
        if (mad.agent.CalculatePath(mad.target.position, path))
        {
            target = path.corners[1];
        }
    }
    
    public override void FixedUpdate()
    {
        if (mad.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            if (mad.agent.CalculatePath(mad.target.position, path))
            {
                target = path.corners[1];
            }
            mad.controller.Rotate(Quaternion.LookRotation((target-mad.transform.position)+Vector3.up));
        }
        
        distance = Vector3.Distance(mad.transform.position, mad.target.position);
        
        if (distance > maxMeleeRange)
        {
            mad.Transit(mad.chasingState);
        }
        else if (distance > minMeleeRange)
        {
            Stop();
            //mad.controller.Attack();
            avatarIK.Attack();
            // just smack :)
        }
    }
}
