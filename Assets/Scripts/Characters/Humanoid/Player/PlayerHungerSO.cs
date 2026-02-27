using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerHungerSO", menuName = "Scriptable Objects/PlayerHungerSO")]
public class PlayerHungerSO : ScriptableObject
{
    [Tooltip("The maximum amount of hunger")]
    [SerializeField] public int MaxHunger = 100;
    public int CurrentHunger = 0;
    public int HungerDamage = 0;
    [Tooltip("The amount of time between hunger ticks in seconds")]
    public float HungerCooldown = 10f;
    public UnityEvent OnChangeHunger;
    [Tooltip("The calculated amount of time the player can survive in seconds, based on max hunger and hunger cooldown")]
    public float SurvivalTime = 0;

    private void OnValidate()
    {
        SurvivalTime = MaxHunger * HungerCooldown;
    }


    public void ResetValues()
    {
        CurrentHunger = MaxHunger;
        HungerDamage = 0;
    }
    
    public bool ChangeHunger(int amount)
    {
        CurrentHunger += amount;
        OnChangeHunger?.Invoke();

        if (CurrentHunger < 0)
        {
            CurrentHunger = 0;
            return false;
        }
        if (CurrentHunger > MaxHunger)
        {
            CurrentHunger = MaxHunger;
        }

        return true;
    }
}