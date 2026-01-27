using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField, Min(1)] private int maxHealth = 1;

    public event Action<int> OnTakeDamage;
    public event Action OnDeath;
    public bool Dead { get { return health <= 0; } }

    private void OnDestroy()
    {
        OnTakeDamage = null;
        OnDeath = null;
    }

    private void Start()
    {
        health = maxHealth;
    }

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

        health -= amount;

        OnTakeDamage?.Invoke(amount);

        if (health <= 0)
        {
            Die();
        }

        return true;
    }
    private void Die()
    {
        OnDeath?.Invoke();
    }

#if UNITY_INCLUDE_TESTS
    public int GetHealth() { return health; }
    public int GetMaxHealth() { return maxHealth; }
#endif
}
