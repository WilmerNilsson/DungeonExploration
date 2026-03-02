using System.Collections;
using UnityEngine;

[System.Serializable]
public class MadAdventurerHallucinationState : MadAdventurerIdleState
{
    [Header("Hallucination Stuff")]
    [SerializeField] private int hallucinationLayer;
    [SerializeField] private int regularLayer;
    [SerializeField] private int lifetimeSeconds = 15;

#nullable enable
    Coroutine? lifetimeLimiterCoroutine = null;

    public override void Start()
    {
        base.Start();
    }

    protected override bool DetectPlayer()
    {
        return false;
    }

    private IEnumerator LifetimeLimiter(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        GameObject.Destroy(MyMadAdventurerStateMachine);
    }

    public override void Enter()
    {
        base.Enter();
        MyMadAdventurerStateMachine.gameObject.layer = hallucinationLayer;
        lifetimeLimiterCoroutine = MyMadAdventurerStateMachine.StartCoroutine(LifetimeLimiter(lifetimeSeconds));
    }

    public override void Exit()
    {
        MyMadAdventurerStateMachine.gameObject.layer = regularLayer;
        if(lifetimeLimiterCoroutine != null)
        {
            MyMadAdventurerStateMachine.StopCoroutine(lifetimeLimiterCoroutine);
        }
        base.Exit();
    }
}
