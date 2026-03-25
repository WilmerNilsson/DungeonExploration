using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField, Tooltip("leave as null to use internal ints")]
    private PlayerHealthSO playerHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField, Min(1)] private int _maxHealth = 1;
    [SerializeField, Min(1)] private float minimumFallDamageHeight;
    [SerializeField, Min(1), Tooltip("how much to multiply the fall distance with")] private float fallDamageMultiplier;
    [SerializeField, Min(0)] public int DurabilityDamage;
    private bool selfInitialize = true;
    public static Health PlayerHealthInstance { get; private set;  }
    
    public UnityEvent<HealthData> OnChangeHealths;
    public UnityEvent<int> OnTakeDamage;
    public UnityEvent OnDeath;
    /// <summary>
    /// if health or max health changes this will activate.<br/>
    /// order is: current health, max health. <br/>
    /// it is the new values that are given, not the difference in values.
    /// </summary>

    public bool Dead { private set; get; } = false;
    public int CurrentHealth { 
        get
        {
            if (playerHealth == null)
                return _currentHealth;
            else 
                return playerHealth.CurrentHealth;
        } 
        private set
        {
            if (playerHealth == null)
            {
                _currentHealth = value;
            }

            else
                playerHealth.CurrentHealth = value;
        }
    }

    public int MaxHealth
    {
        get
        {
            if (playerHealth == null)
                return _maxHealth;
            else
                return playerHealth.MaxHealth;
        }
        private set
        {
            if (playerHealth == null)
                _maxHealth = value;
            else
                playerHealth.MaxHealth = value;
        }
    }

    private void OnDestroy()
    {
        OnTakeDamage = null;
        OnDeath = null;
    }

    private void Start()
    {
        if(selfInitialize)
        {
            SetCurrentHealth(MaxHealth);
        }

        if (!(playerHealth == null))
        {
            PlayerHealthInstance = this;
        }
    }

    public void StopSelfInitialize()
    {
        selfInitialize = false;
    }

    public void Kill()
    {
        CurrentHealth = 0;
        Die();
    }

    //used for events
    /// <summary>
    /// positive number heals, negative damages.
    /// </summary>
    public void ChangeHealthVoid(int amount)
    {
        ChangeHealth(amount);
    }

    /// <summary>
    /// fails and returns false if dead already.
    /// </summary>
    public bool SetCurrentHealth(int newValue)
    {
        return ChangeHealth(newValue - CurrentHealth);
    }

    /// <summary>
    /// fails and returns false if dead already.
    /// </summary>
    public bool SetMaxHealth(int newValue)
    {
        if(Dead) return false;
        if (newValue == MaxHealth) return true;

        MaxHealth = newValue;

        if(CurrentHealth > MaxHealth)
        {
            return SetCurrentHealth(MaxHealth);
        }
        else
        {
            OnChangeHealths?.Invoke(new HealthData(CurrentHealth, MaxHealth));
            return true;
        }
    }

    /// <summary>
    /// positive number heals, negative damages. <br/>
    /// fails and returns false if dead already.
    /// </summary>
    public bool ChangeHealth(int amount)
    {
        if(Dead) return false;

        CurrentHealth += amount;

        if (CurrentHealth <= 0)
        {
            Die();
        }
        else if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }

        OnChangeHealths?.Invoke(new HealthData(CurrentHealth, MaxHealth));

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

    public void FallingDamage(float amount)
    {
        if (amount <= minimumFallDamageHeight) return;
        
        TakeDamage(Mathf.RoundToInt((amount - minimumFallDamageHeight) * fallDamageMultiplier));
    }
    private void Die()
    {
        Dead = true;
        OnDeath?.Invoke();
    }
}
