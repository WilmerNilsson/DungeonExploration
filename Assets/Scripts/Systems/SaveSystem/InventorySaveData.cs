using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventorySaveData
{
    public List<InventoryItem> Items;

    public InventorySaveData(List<InventoryItem> items)
    {
        Items = items;
    }

    public InventorySaveData Clone()
    {
        InventorySaveData cloneData = new InventorySaveData(new(Items));
        return cloneData;
    }

    //if we want to add a check for equality like: bool Equal(InventorySaveData a, InventorySaveData b)
    //check tests, since it is already done.

    [Serializable]
    public struct InventoryItem
    {
        public string Name;
        public int Slot; 
    }
}
