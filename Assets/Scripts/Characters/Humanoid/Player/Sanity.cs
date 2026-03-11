using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Sanity : MonoBehaviour
{
    [SerializeField] private PlayerSanitySO playerSanitySO;
    //[SerializeField] private Health health;
    
    //[Tooltip("The amount of time between hunger ticks in seconds")]
    //[SerializeField] private float hungerCooldown = 10;

    public UnityEvent<float> OnLoseSanity;
    public UnityEvent OnGainSanity;

    public static Sanity Instance;
    [SerializeField] private bool ResetOnAwake = true;
    [SerializeField] private int damageToSanityMod = 10;
    [SerializeField] private SanityLightProbe sanityLightProbe;

    Coroutine sanityTick;

    public int CurrentSanity
    {
        get
        {
            return playerSanitySO.CurrentSanity;
        }
    }

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
        ResetSanityTick();
        if (ResetOnAwake)
        {
            ResetSanity();
        }
    }

    /// <summary>
    /// returns true if camera is in any light
    /// </summary>
    private bool IsInLightCheck()
    {
        return sanityLightProbe.Sample() > 0f;
    }

    private void ResetSanityTick()
    {
        if(sanityTick != null)
        {
            StopCoroutine(sanityTick);
        }
        sanityTick = StartCoroutine(SanityTickCoroutine());

        IEnumerator SanityTickCoroutine()
        {
            yield return new WaitForSeconds(playerSanitySO.GetCurrentCooldown(IsInLightCheck()));
            LoseSanity(1);
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

        ResetSanityTick();
    }

    public void DamageSanity(int amount)
    {
        playerSanitySO.ChangeSanity(-amount/damageToSanityMod);
        OnLoseSanity?.Invoke((float)playerSanitySO.CurrentSanity / playerSanitySO.MaxSanity);

        ResetSanityTick();
    }

    public void GainSanity(int amount)
    {
        playerSanitySO.ChangeSanity(amount);
        OnGainSanity?.Invoke();

        ResetSanityTick();
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
