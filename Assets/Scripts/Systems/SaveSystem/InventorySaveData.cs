using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventorySaveData
{
    public List<InventoryItem> Items;

    public override string ToString()
    {
        string str = string.Empty;

        foreach (InventoryItem inventoryItem in Items)
        {
            str += inventoryItem.ToString();
        }
        return str;
    }

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
        public string PrefabID;
        public int Slot;

        public override string ToString()
        {
            return $"[prefab ID: {PrefabID}, slot: {Slot}]";
        }
    }
}
