using UnityEngine;

[System.Serializable]
public class MadChasing : MadState
{
    [SerializeField] private float minDistanceToPlayer;
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
}
