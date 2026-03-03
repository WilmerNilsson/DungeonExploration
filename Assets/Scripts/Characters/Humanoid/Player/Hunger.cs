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
    [SerializeField] private bool resetOnAwake = true;

    private Coroutine hungerTick;

    private void Awake()
    {
        instance = this;
        ResetHungerTick();
        if(resetOnAwake)
        {
            ResetHunger();
        }
    }

    private void ResetHungerTick()
    {
        if(hungerTick != null)
        {
            StopCoroutine(hungerTick);
        }
        hungerTick = StartCoroutine(HungerTickCoroutine());

        IEnumerator HungerTickCoroutine()
        {
            yield return new WaitForSeconds(playerHungerSO.HungerCooldown);
            LoseHunger(1);
        }
}


    public void ResetHunger()
    {
        playerHungerSO.ResetValues();
    }

    public void Initialize(int newCurrentHunger)
    {
        resetOnAwake = false;
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
        if (!playerHungerSO.ChangeHunger(-amount))
        {
            health.TakeDamage(1);
        }
        OnHunger?.Invoke((float)playerHungerSO.CurrentHunger / playerHungerSO.MaxHunger);

        ResetHungerTick();
    }

    public void Eat(int amount)
    {
        StopAllCoroutines();
        playerHungerSO.ChangeHunger(amount);
        MinimapMaster.Instance.SpawnMinimap();
        OnEat?.Invoke();

        ResetHungerTick();
    }
}