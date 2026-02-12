using System;
using System.Collections;
using UnityEngine;

public class Hunger : MonoBehaviour
{
    [Tooltip("The maximum amount of hunger")]
    [SerializeField] private int maxHunger = 100;
    public int currentHunger = 0;
    [Tooltip("The health component of the player")]
    [SerializeField] private Health health;
    private int hungerDamage = 0;
    
    [Tooltip("The amount of time between hunger ticks in seconds")]
    [SerializeField] private float hungerCooldown = 10;
    
    public static Hunger instance;

    private IEnumerator HungerCoroutine()
    {
        yield return new WaitForSeconds(hungerCooldown);
        LoseHunger(1);
    }

    private void Awake()
    {
        instance = this;
        currentHunger = maxHunger;
        StartCoroutine(HungerCoroutine());
    }

    public void LoseHunger(int amount)
    {
        currentHunger -= amount;
        if (currentHunger < 0)
        {
            currentHunger = 0;
            hungerDamage++;
            health.TakeDamage(hungerDamage);
        }

        StartCoroutine(HungerCoroutine());
    }

    public void Eat(int amount)
    {
        StopAllCoroutines();
        currentHunger += amount;
        if (currentHunger > maxHunger)
        {
            currentHunger = maxHunger;
        }

        StartCoroutine(HungerCoroutine());
    }
}
