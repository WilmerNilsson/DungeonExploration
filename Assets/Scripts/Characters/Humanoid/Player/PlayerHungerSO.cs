using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerHungerSO", menuName = "Scriptable Objects/PlayerHungerSO")]
public class PlayerHungerSO : ScriptableObject
{
    [Tooltip("The maximum amount of hunger")]
    [SerializeField] public int maxHunger = 100;
    public int currentHunger = 0;
    public int hungerDamage = 0;
    [Tooltip("The amount of time between hunger ticks in seconds")]
    public float hungerCooldown = 10f;
    public UnityEvent OnChangeHunger;


    public void ResetValues()
    {
        currentHunger = maxHunger;
        hungerDamage = 0;
    }
    
    public bool ChangeHunger(int amount)
    {
        currentHunger += amount;
        OnChangeHunger?.Invoke();

        if (currentHunger < 0)
        {
            currentHunger = 0;
            return false;
        }
        if (currentHunger > maxHunger)
        {
            currentHunger = maxHunger;
        }

        return true;
    }
}