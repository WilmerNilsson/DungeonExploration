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
    private bool isInLight;

    Coroutine sanityTick;

    public int CurrentSanity
    {
        get
        {
            return playerSanitySO.CurrentSanity;
        }
    }

    private void Awake()
    {
        Instance = this;
        ResetSanityTick();
        if (ResetOnAwake)
        {
            ResetSanity();
        }
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
            yield return new WaitForSeconds(playerSanitySO.GetCurrentCooldown(isInLight));
            LoseSanity(1);
        }

    }
    public void ResetSanity()
    {
        playerSanitySO.ResetValues();
    }

    public void Initialize(int newCurrentHunger)
    {
        ResetOnAwake = false;
        playerSanitySO.ResetValues();
        playerSanitySO.CurrentSanity = newCurrentHunger;
        OnLoseSanity?.Invoke((float)playerSanitySO.CurrentSanity / playerSanitySO.MaxSanity);
    }

    public int GetHungerValue()
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
        if (!playerSanitySO.ChangeSanity(-1))
        {
            
        }

        ResetSanityTick();
    }

    public void GainSanity(int amount)
    {
        StopAllCoroutines();
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
