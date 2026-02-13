using System;
using UnityEngine;

[Serializable]
public class PlayerSaveData
{
    public InventorySaveData Inventory = new();

    public Vector3 Position;

    public int MaxHP;
    public int CurrentHP;

    public int Sanity;
    public int Hunger;

    public PlayerSaveData() { }

    public PlayerSaveData(InventorySaveData inventory, Vector3 position, int maxHP, int currentHP, int sanity, int hunger)
    {
        Inventory = inventory;
        Position = position;
        MaxHP = maxHP;
        CurrentHP = currentHP;
        Sanity = sanity;
        Hunger = hunger;
    }

    public PlayerSaveData Clone()
    {
        return new(Inventory.Clone(), Position, MaxHP, CurrentHP, Sanity, Hunger);
    }
}
