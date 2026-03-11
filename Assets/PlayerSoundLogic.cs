using FMOD.Studio;
using UnityEngine;

public class PlayerSoundLogic : MonoBehaviour
{
    [SerializeField] private string sanityPath;
    
    [SerializeField] private string hungerPath;
    
    [SerializeField] private string exhaustionPath;
    [SerializeField] private string exhaustionParameter;
    [SerializeField] private float exhaustionMax;
    [Tooltip("X = increase speed, Y = decrease speed")]
    [SerializeField] private Vector2 exhaustionSpeed;
    [SerializeField] private float swingExhaustionDelta;
    
    private float _exhaustion;
    private HumanoidMovement.moveActions _currentMoveAction;
    

    private void Start()
    {
        if (!AudioManager.IsValid) return;
        AudioManager.Instance.CreateInstance(sanityPath, gameObject);
        AudioManager.Instance.StartEvent(sanityPath, gameObject);
        
        AudioManager.Instance.CreateInstance(exhaustionPath, gameObject);
        AudioManager.Instance.StartEvent(exhaustionPath, gameObject);
    }
    
    public void OnLoseSanity(float sanity)
    {
        if (!AudioManager.IsValid) return;
        AudioManager.Instance.SetGlobalParameter("Sanity", sanity);
    }

    public void ChangeExhaustion(float delta)
    {
        _exhaustion += delta;
        if (!AudioManager.IsValid) return;
        AudioManager.Instance.SetGlobalParameter(exhaustionParameter, _exhaustion);
    }

    private void FixedUpdate()
    {
        if (!AudioManager.IsValid) return;
        if (_currentMoveAction == HumanoidMovement.moveActions.Sprinting)
        {
            _exhaustion = Mathf.MoveTowards(_exhaustion, exhaustionMax, exhaustionSpeed.x * Time.fixedDeltaTime);
        }
        else
        {
            _exhaustion = Mathf.MoveTowards(_exhaustion, 0f, exhaustionSpeed.y * Time.fixedDeltaTime);
        }
        AudioManager.Instance.SetGlobalParameter(exhaustionParameter, _exhaustion);
    }

    public void OnMoveStateChange(HumanoidMovement.moveActions moveAction)
    {
        _currentMoveAction = moveAction;
        Debug.Log(_currentMoveAction);
    }

    public void OnAttackStateChange(HumanoidAttackAnimatorCompanion.AttackState state)
    {
        if (state == HumanoidAttackAnimatorCompanion.AttackState.Swing)
        {
            ChangeExhaustion(swingExhaustionDelta);
        }
    }
    
    public void OnHungerChange(float hungerRatio)
    {
        if (!AudioManager.IsValid) return;
        AudioManager.Instance.SetGlobalParameter("Hunger", hungerRatio);
        AudioManager.Instance.PlayOneShot(hungerPath);
    }

    public void OnDeath()
    {
        AudioManager.Instance.StopEvent(sanityPath, STOP_MODE.ALLOWFADEOUT, gameObject);
        AudioManager.Instance.ReleaseInstance(sanityPath, gameObject);
        
        AudioManager.Instance.StopEvent(exhaustionPath, STOP_MODE.ALLOWFADEOUT, gameObject);
        AudioManager.Instance.ReleaseInstance(exhaustionPath, gameObject);
    }

    private void OnDestroy()
    {
        OnDeath();
    }
}
