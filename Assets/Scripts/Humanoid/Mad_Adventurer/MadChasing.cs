using UnityEngine;

[System.Serializable]
public class MadChasing : MadState
{
    public override void Exit()
    {
        base.Exit();
        mad.controller.isSprinting = false;
    }
}
