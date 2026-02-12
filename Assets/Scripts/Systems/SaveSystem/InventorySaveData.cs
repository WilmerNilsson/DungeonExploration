using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventorySaveData
{
    public List<InventoryItem> Items = new();

    [Serializable]
    public struct InventoryItem
    {
        public string Name;
        public int Slot; 
    }
}
