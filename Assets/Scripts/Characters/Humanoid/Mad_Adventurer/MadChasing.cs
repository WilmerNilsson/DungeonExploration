using UnityEngine;

[System.Serializable]
public class MadChasing : MadState
{
    [SerializeField] private float minDistanceToPlayer;
    
    [Header("Player detection")] 
    [SerializeField] private float maxSightRange;
    [SerializeField] private float maxSoundRange;
    [SerializeField] private float sightThreshold;
    [SerializeField] private float soundThreshold;
    
    public override void Enter()
    {
        mad.controller.isSprinting = true;
        FindPath(mad.player.position);
        target = GetNextCorner();
    }

    public override void Exit()
    {
        base.Exit();
        mad.controller.isSprinting = false;
    }

    public override void Update()
    {
        if (!DetectPlayer())
        {
            mad.Transit(mad.idleState);
        }
        if (Vector3.Distance(mad.player.position, mad.transform.position) <= minDistanceToPlayer)
        {
            mad.Transit(mad.meleeState);
        }
        else
        {
            FindPath(mad.player.position);
            target = GetNextCorner();
            Move();
        }
        
    }
    
    private bool DetectPlayer()
    {
        return mad.vision.SightDetection(maxSightRange) > sightThreshold || mad.vision.SoundDetection(maxSoundRange) > soundThreshold;
    }
}
