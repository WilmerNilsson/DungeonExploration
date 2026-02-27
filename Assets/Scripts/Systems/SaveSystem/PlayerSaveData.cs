using System;
using UnityEngine;

[Serializable]
public class PlayerSaveData
{
    public InventorySaveData Inventory;

    public bool FromTown;

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

        FromTown = false;
    }

    public PlayerSaveData(InventorySaveData inventory, bool fromTown, int maxHP, int currentHP, int sanity, int hunger)
    {
        Inventory = inventory;
        FromTown = fromTown;
        MaxHP = maxHP;
        CurrentHP = currentHP;
        Sanity = sanity;
        Hunger = hunger;
    }

    private PlayerSaveData(InventorySaveData inventory, Vector3 position, Quaternion rotation, bool fromTown, int maxHP, int currentHP, int sanity, int hunger)
    {
        Inventory = inventory;
        Position = position;
        Rotation = rotation;
        FromTown = fromTown;
        MaxHP = maxHP;
        CurrentHP = currentHP;
        Sanity = sanity;
        Hunger = hunger;
    }

    public PlayerSaveData Clone()
    {
        return new(Inventory.Clone(), Position, Rotation, FromTown, MaxHP, CurrentHP, Sanity, Hunger);
    }
}
