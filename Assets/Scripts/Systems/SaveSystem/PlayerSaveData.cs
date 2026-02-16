using System;
using UnityEngine;

[Serializable]
public class PlayerSaveData
{
    public InventorySaveData Inventory;

    public Vector3 Position;
    public Quaternion Rotation;

    public int MaxHP;
    public int CurrentHP;

    public int Sanity;
    public int Hunger;

    public PlayerSaveData(InventorySaveData inventory, Vector3 position, Quaternion rotation, int maxHP, int currentHP, int sanity, int hunger)
    {
        Inventory = inventory;
        Position = position;
        Rotation = rotation;
        MaxHP = maxHP;
        CurrentHP = currentHP;
        Sanity = sanity;
        Hunger = hunger;
    }

    public PlayerSaveData Clone()
    {
        return new(Inventory.Clone(), Position, Rotation, MaxHP, CurrentHP, Sanity, Hunger);
    }
}
