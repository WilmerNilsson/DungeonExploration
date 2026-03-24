using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Sanity : MonoBehaviour
{
    [SerializeField] private PlayerSanitySO playerSanitySO;
    [SerializeField] private SanityLightProbe sanityLightProbe;
    //[SerializeField] private Health health;
    
    //[Tooltip("The amount of time between hunger ticks in seconds")]
    //[SerializeField] private float hungerCooldown = 10;

    public UnityEvent<float> OnLoseSanity;
    public UnityEvent OnGainSanity;

    public static Sanity Instance;
    [SerializeField] private bool ResetOnAwake = true;

    [SerializeField, Min(0), Tooltip("How much sanity is lost over time in Light")] private int inLightSanityDamage;
    [SerializeField, Min(0), Tooltip("How much sanity is lost over time in Dark")] private int inDarkSanityDamage;
    [SerializeField, Tooltip("How much the damage to health is divided by before being applied to sanity")] private int damageToSanityMod = 10;
    [SerializeField, Tooltip("brightness level, if its lower its considered dark"), Min(0.01f)] private float lightThreshold;
    [SerializeField, Tooltip("How long between sanity ticks, in seconds"), Min(0)] private float sanityTickSpeed;
    private int damage;
    
    
    Coroutine sanityTick;

    public int CurrentSanity => playerSanitySO.CurrentSanity;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (playerSanitySO == null) Debug.LogWarning("playerSanitySO is null", this);
        if (sanityLightProbe == null) Debug.LogWarning("sanityLightprobe is null", this);
    }
#endif

    private void Awake()
    {
        Instance = this;
        if (ResetOnAwake)
        {
            ResetSanity();
        }
    }

    private void Start()
    {
        StartSanity();
    }

    /// <summary>
    /// returns true if camera is in any light
    /// </summary>
    private bool IsInLightCheck()
    {
        return sanityLightProbe.Sample() > lightThreshold;
    }

    private void StartSanity()
    {
        if (sanityTick != null)
        {
            StopCoroutine(sanityTick);
        }

        StartCoroutine(SanityTickCoroutine());
        
        IEnumerator SanityTickCoroutine()
        {
            while (CurrentSanity > 0)
            {
                yield return new WaitForSeconds(sanityTickSpeed);
                damage = IsInLightCheck() ? inLightSanityDamage : inDarkSanityDamage;
                LoseSanity(damage);
            }
        }
    }
    
    public void ResetSanity()
    {
        playerSanitySO.ResetValues();
    }

    public void Initialize(int newCurrentSanity)
    {
        ResetOnAwake = false;
        playerSanitySO.ResetValues();
        SetSanity(newCurrentSanity);
    }

    public int GetSanityValue()
    {
        return playerSanitySO.CurrentSanity;
    }

    /// <summary>
    /// takes a positive number
    /// </summary>
    public void LoseSanity(int amount)
    {
        playerSanitySO.ChangeSanity(-amount);
        OnLoseSanity?.Invoke((float)playerSanitySO.CurrentSanity / playerSanitySO.MaxSanity);
    }

    public void DamageSanity(int amount)
    {
        playerSanitySO.ChangeSanity(-amount/damageToSanityMod);
        OnLoseSanity?.Invoke((float)playerSanitySO.CurrentSanity / playerSanitySO.MaxSanity);
    }

    public void GainSanity(int amount)
    {
        playerSanitySO.ChangeSanity(amount);
        OnGainSanity?.Invoke();
    }

    public void SetSanity(int newValue)
    {
        int diff = newValue - CurrentSanity;

        if (diff > 0)
        {
            GainSanity(diff);
        }
        else if (diff < 0)
        {
            LoseSanity(-diff);
        }
    }
}
