using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventorySaveData
{
    public List<InventoryItem> Items = new();

    public InventorySaveData Clone()
    {
        InventorySaveData cloneData = new InventorySaveData();
        cloneData.Items = new(Items);
        return cloneData;
    }

    [Serializable]
    public struct InventoryItem
    {
        public string Name;
        public int Slot; 
    }
}
