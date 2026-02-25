using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Hunger : MonoBehaviour
{
    [SerializeField] private PlayerHungerSO playerHungerSO;
    [SerializeField] private Health health;
    
    //[Tooltip("The amount of time between hunger ticks in seconds")]
    //[SerializeField] private float hungerCooldown = 10;

    public UnityEvent<float> OnHunger;
    public UnityEvent OnEat;

    public static Hunger instance;
    private bool selfInitialize = true;

    private IEnumerator HungerCoroutine()
    {
        yield return new WaitForSeconds(playerHungerSO.HungerCooldown);
        LoseHunger(1);
    }

    private void Awake()
    {
        instance = this;
        StartCoroutine(HungerCoroutine());
        if(selfInitialize)
        {
            ResetHunger();
        }
    }

    public void ResetHunger()
    {
        playerHungerSO.ResetValues();
    }

    public void Initialize(int newCurrentHunger)
    {
        selfInitialize = false;
        playerHungerSO.ResetValues();
        playerHungerSO.CurrentHunger = newCurrentHunger;
        OnHunger?.Invoke((float)playerHungerSO.CurrentHunger / playerHungerSO.MaxHunger);
    }

    public int GetHungerValue()
    {
        return playerHungerSO.CurrentHunger;
    }

    public void LoseHunger(int amount)
    {
        OnHunger?.Invoke((float)playerHungerSO.CurrentHunger / playerHungerSO.MaxHunger);
        if (!playerHungerSO.ChangeHunger(-1))
        {
            health.TakeDamage(1);
        }

        StartCoroutine(HungerCoroutine());
    }

    public void Eat(int amount)
    {
        StopAllCoroutines();
        playerHungerSO.ChangeHunger(amount);
        OnEat?.Invoke();

        StartCoroutine(HungerCoroutine());
    }
}