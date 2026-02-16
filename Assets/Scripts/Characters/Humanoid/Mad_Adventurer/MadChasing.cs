using UnityEngine;

[System.Serializable]
public class MadChasing : MadState
{
    public override void Enter()
    {
        base.Enter();
        mad.controller.isSprinting = true;
        if (mad.agent.CalculatePath(mad.target.position, path))
        {
            target = path.corners[1];
        }
    }

    public override void Exit()
    {
        base.Exit();
        mad.controller.isSprinting = false;
    }

    public override void Update()
    {
        if (Vector3.Distance(target, mad.transform.position) <= mad.meleeState.maxMeleeRange)
        {
            mad.Transit(mad.meleeState);
        }
        else
        {
            if (mad.agent.CalculatePath(mad.target.position, path))
            {
                target = path.corners[1];
            }
            base.Update();
        }
        
    }
}
