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

    [Serializable]
    public struct InventoryItem
    {
        public string Name;
        public int Slot; 
    }
}
