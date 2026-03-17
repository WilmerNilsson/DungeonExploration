using FMOD.Studio;
using UnityEngine;

public class PlayerSoundLogic : MonoBehaviour
{

    private float lastHealth;
    [SerializeField] private string sanityPath;
    [SerializeField] private string hungerPath;

    [SerializeField] private string exhaustionPath;
    [SerializeField] private string exhaustionParameter;
    [SerializeField] private float exhaustionMax;
    [Tooltip("X = increase speed, Y = decrease speed")]
    [SerializeField] private Vector2 exhaustionSpeed;
    [SerializeField] private float swingExhaustionDelta;
    
    public float exhaustion;
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
        exhaustion += delta;
        if (!AudioManager.IsValid) return;
        AudioManager.Instance.SetGlobalParameter(exhaustionParameter, exhaustion);
    }

    private void FixedUpdate()
    {
        if (!AudioManager.IsValid) return;
        if (_currentMoveAction == HumanoidMovement.moveActions.Sprinting)
        {
            exhaustion = Mathf.MoveTowards(exhaustion, exhaustionMax, exhaustionSpeed.x * Time.fixedDeltaTime);
        }
        else
        {
            exhaustion = Mathf.MoveTowards(exhaustion, 0f, exhaustionSpeed.y * Time.fixedDeltaTime);
        }
        exhaustion = Mathf.Clamp(exhaustion, 0f, exhaustionMax);
        AudioManager.Instance.SetGlobalParameter(exhaustionParameter, exhaustion);
    }

    public void OnMoveStateChange(HumanoidMovement.moveActions moveAction)
    {
        _currentMoveAction = moveAction;
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

    public void OnHealthChange(HealthData healthData)
    {
        if (!AudioManager.IsValid) return;
        AudioManager.Instance.SetGlobalParameter("HP", healthData.CurrentHealth);
        AudioManager.Instance.SetGlobalParameter("hpRatio", (float)healthData.CurrentHealth / healthData.MaxHealth);
    }

    private void OnDestroy()
    {
        OnDeath();
    }

}
