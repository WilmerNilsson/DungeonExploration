using System;
using UnityEngine;

[Serializable]
public class PlayerSaveData
{
    public InventorySaveData Inventory;

    public int MaxHP;
    public int CurrentHP;

    public int Sanity;
    public int Hunger;
}
