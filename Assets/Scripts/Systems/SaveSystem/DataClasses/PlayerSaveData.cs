using System;
using UnityEngine;

[Serializable]
public class PlayerSaveData
{
    public InventorySaveData Inventory;
    public InventorySaveData Equipment;

    public bool FromTown;

    public Vector3 Position;
    public Quaternion Rotation;

    public int CurrentHP;
    public int Sanity;
    public int Hunger;
    public int RunCount;

    public PlayerSaveData(InventorySaveData inventory, InventorySaveData equipment, Vector3 position, Quaternion rotation, int currentHP, int sanity, int hunger, int runCount)
    {
        Inventory = inventory;
        Equipment = equipment;
        Position = position;
        Rotation = rotation;
        CurrentHP = currentHP;
        Sanity = sanity;
        Hunger = hunger;
        RunCount = runCount;

        FromTown = false;
    }

    public PlayerSaveData(InventorySaveData inventory, InventorySaveData equipment, bool fromTown, int currentHP, int sanity, int hunger, int runCount)
    {
        Inventory = inventory;
        Equipment = equipment;
        FromTown = fromTown;
        CurrentHP = currentHP;
        Sanity = sanity;
        Hunger = hunger;
        RunCount = runCount;
    }

    private PlayerSaveData(InventorySaveData inventory, InventorySaveData equipment, Vector3 position, Quaternion rotation, bool fromTown, int currentHP, int sanity, int hunger, int runCount)
    {
        Inventory = inventory;
        Equipment = equipment;
        Position = position;
        Rotation = rotation;
        FromTown = fromTown;
        CurrentHP = currentHP;
        Sanity = sanity;
        Hunger = hunger;
        RunCount = runCount;
    }

    public PlayerSaveData Clone()
    {
        return new(Inventory.Clone(), Equipment.Clone(), Position, Rotation, FromTown, CurrentHP, Sanity, Hunger, RunCount);
    }
}
