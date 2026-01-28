using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField, Min(1)] private int maxHealth = 1;
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public event Action<int>? OnTakeDamage;
    public event Action? OnDeath;
    /// <summary>
    /// if health or max health changes this will activate.<br/>
    /// order is: current health, max health. <br/>
    /// it is the new values that are given, not the difference in values.
    /// </summary>
    public event Action<int, int>? OnChangeHealths;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

    public bool Dead { private set; get; } = false;

    private void OnDestroy()
    {
        OnTakeDamage = null;
        OnDeath = null;
    }

    private void Start()
    {
        health = maxHealth;
    }

    public int GetHealth() { return health; }
    public int GetMaxHealth() { return maxHealth; }

    public void Kill()
    {
        health = 0;
        Die();
    }

    /// <summary>
    /// positive number heals, negative damages. <br/>
    /// fails and returns false if dead already.
    /// </summary>
    public bool ChangeHealth(int amount)
    {
        if(Dead) return false;

        health += amount;

        OnChangeHealths?.Invoke(health, maxHealth);

        if (health <= 0)
        {
            Die();
        }
        else if (health > maxHealth)
        {
            health = maxHealth;
        }

        return true;
    }

    /// <summary>
    /// takes a positive number and reduces health by that amount. <br/>
    /// activates events like OnTakeDamage and invokes Die if hp gets bellow 0.
    /// </summary>
    /// <param name="amount">a number above 0</param>
    /// <returns></returns>
    public bool TakeDamage(int amount)
    {
#if DEBUG
        if (amount < 0) Debug.LogWarning("taking positive damage", this);
        if (amount == 0) Debug.LogWarning("taking 0 damage", this);
#endif

        if (Dead || amount <= 0) return false;

        OnTakeDamage?.Invoke(amount);

        ChangeHealth(-amount);

        return true;
    }
    private void Die()
    {
        Dead = true;
        OnDeath?.Invoke();
    }
}
