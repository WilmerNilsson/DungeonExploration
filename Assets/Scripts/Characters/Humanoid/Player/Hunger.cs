using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Hunger : MonoBehaviour
{
    [SerializeField] private PlayerHungerSO playerHungerSO;
    [SerializeField] private Health health;
    
    [Tooltip("The amount of time between hunger ticks in seconds")]
    [SerializeField] private float hungerCooldown = 10;

    public UnityEvent OnHunger;
    public UnityEvent OnEat;

    public static Hunger instance;
    
    private IEnumerator HungerCoroutine()
    {
        yield return new WaitForSeconds(hungerCooldown);
        LoseHunger(1);
    }

    public void ResetHunger()
    {
        playerHungerSO.ResetValues();
    }

    private void Awake()
    {
        instance = this;
        StartCoroutine(HungerCoroutine());
    }

    public void LoseHunger(int amount)
    {
        OnHunger?.Invoke();
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