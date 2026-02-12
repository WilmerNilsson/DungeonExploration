using System;
using UnityEngine;

[Serializable]
public class PlayerSaveData
{
    public InventorySaveData Inventory = new();

    public int MaxHP;
    public int CurrentHP;

    public int Sanity;
    public int Hunger;

    public PlayerSaveData() { }

    public PlayerSaveData(InventorySaveData inventory, int maxHP, int currentHP, int sanity, int hunger)
    {
        Inventory = inventory;
        MaxHP = maxHP;
        CurrentHP = currentHP;
        Sanity = sanity;
        Hunger = hunger;
    }

    public PlayerSaveData Clone()
    {
        return new(Inventory.Clone(), MaxHP, CurrentHP, Sanity, Hunger);
    }
}
