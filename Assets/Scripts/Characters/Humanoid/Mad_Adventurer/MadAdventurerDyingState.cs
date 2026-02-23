using UnityEngine;

[System.Serializable]
public class MadAdventurerDyingState : MadAventurerBaseState
{
    [SerializeField] private float timeUntilDeactivate = 4f;
    [SerializeField] private float deactivateTimer = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Intialize(MadAdventurerStateMachine madAdventurer)
    {
        base.Intialize(madAdventurer);
        deactivateTimer = timeUntilDeactivate;
    }

    public override void Enter()
    {
        base.Enter();
        Stop();
    }

    // Update is called once per frame
    public override void Update()
    {
        deactivateTimer -= Time.deltaTime;
        if (deactivateTimer <= 0)
        {
            GameObject.Destroy(MyMadAdventurerStateMachine.gameObject);
        }
    }
}
