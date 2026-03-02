using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Sanity : MonoBehaviour
{
    [SerializeField] private PlayerSanitySO playerSanitySO;
    //[SerializeField] private Health health;

    //[Tooltip("The amount of time between hunger ticks in seconds")]
    //[SerializeField] private float hungerCooldown = 10;

    [Tooltip("sends out a event with current % sanity")]
    public UnityEvent<float> OnLoseSanity;
    public UnityEvent OnGainSanity;

    public static Sanity instance;
    [SerializeField] private bool ResetOnAwake = true;
    private bool isInLight;

    private Coroutine sanityTimer;

    public int CurrentSanity
    {
        get
        {
            return playerSanitySO.CurrentSanity;
        }
    }

    private IEnumerator SanityCoroutine()
    {
        yield return new WaitForSeconds(playerSanitySO.GetCurrentCooldown(isInLight));
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

    /// <summary>
    /// takes a positive number
    /// </summary>
    public void LoseSanity(int amount)
    {
        playerSanitySO.ChangeSanity(-amount);
        OnLoseSanity?.Invoke((float)playerSanitySO.CurrentSanity / playerSanitySO.MaxSanity);

        StartCoroutine(SanityCoroutine());
    }

    public void GainSanity(int amount)
    {
        StopAllCoroutines();
        playerSanitySO.ChangeSanity(amount);
        OnGainSanity?.Invoke();

        StartCoroutine(SanityCoroutine());
    }

    public void SetSanity(int newValue)
    {
        int diff = newValue - playerSanitySO.CurrentSanity;

        if(diff < 0)
        {
            LoseSanity(-newValue);
        }
        else if (diff > 0)
        {
            GainSanity(newValue);
        }
    }
}
