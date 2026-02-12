using System;
using System.Collections;
using UnityEngine;

public class Hunger : MonoBehaviour
{
    [SerializeField] private PlayerHungerSO playerHungerSO;
    [SerializeField] private Health health;
    
    [Tooltip("The amount of time between hunger ticks in seconds")]
    [SerializeField] private float hungerCooldown = 10;

    public static Hunger instance;
    
    private IEnumerator HungerCoroutine()
    {
        yield return new WaitForSeconds(hungerCooldown);
        LoseHunger(1);
    }

    public void ResetHunger()
    {
        playerHungerSO.currentHunger = playerHungerSO.maxHunger;
        playerHungerSO.hungerDamage = 0;
    }

    private void Awake()
    {
        instance = this;
        StartCoroutine(HungerCoroutine());
    }

    public void LoseHunger(int amount)
    {
        playerHungerSO.currentHunger -= amount;
        if (playerHungerSO.currentHunger < 0)
        {
            playerHungerSO.currentHunger = 0;
            playerHungerSO.hungerDamage++;
            health.TakeDamage(playerHungerSO.hungerDamage);
        }

        StartCoroutine(HungerCoroutine());
    }

    public void Eat(int amount)
    {
        StopAllCoroutines();
        playerHungerSO.hungerDamage = 0;
        playerHungerSO.currentHunger += amount;
        if (playerHungerSO.currentHunger > playerHungerSO.maxHunger)
        {
            playerHungerSO.currentHunger = playerHungerSO.maxHunger;
        }

        StartCoroutine(HungerCoroutine());
    }
}
