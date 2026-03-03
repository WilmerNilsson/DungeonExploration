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

    public static Sanity instance;
    [SerializeField] private bool ResetOnAwake = true;
    private bool light;

    private IEnumerator SanityCoroutine()
    {
        yield return new WaitForSeconds(playerSanitySO.GetCurrentCooldown(light));
        LoseSanity(1);
    }

    private void Awake()
    {
        instance = this;
        StartCoroutine(SanityCoroutine());
        if(ResetOnAwake)
        {
            ResetSanity();
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

    public void LoseSanity(int amount)
    {
        OnLoseSanity?.Invoke((float)playerSanitySO.CurrentSanity / playerSanitySO.MaxSanity);
        if (!playerSanitySO.ChangeSanity(-1))
        {
            
        }

        StartCoroutine(SanityCoroutine());
    }

    public void GainSanity(int amount)
    {
        StopAllCoroutines();
        playerSanitySO.ChangeSanity(amount);
        OnGainSanity?.Invoke();

        StartCoroutine(SanityCoroutine());
    }
}
