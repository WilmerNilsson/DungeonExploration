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
        target = mad.player.position;
    }
    
    public override void Update()
    {
        target = mad.player.position;
        mad.controller.Rotate(Quaternion.LookRotation((target-mad.transform.position)+Vector3.up));
        
        distance = Vector3.Distance(mad.transform.position, mad.player.position);
        
        if (distance > maxMeleeRange)
        {
            mad.Transit(mad.chasingState);
        }
        else if (distance > minMeleeRange) // just smack :)
        {
            avatarIK.Attack();
        }
        
        Stop();
    }
}
