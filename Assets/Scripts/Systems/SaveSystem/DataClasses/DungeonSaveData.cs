using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class DungeonSaveData
{
    public List<DroppedItem> DroppedItems = new();
    public List<Enemy> Enemies = new();
    public List<Container> Containers = new();
    public List<int> EnabledObjects = new();
    public List<MinimapComponentData> minimapComponentData = new();
    public bool Initialized = false;

    internal DungeonSaveData Clone()
    {
        DungeonSaveData cloneData = new();
        cloneData.DroppedItems = new(DroppedItems);
        cloneData.Enemies = new(Enemies);
        cloneData.EnabledObjects = new(EnabledObjects);

        cloneData.Containers = new(Containers.Count);
        cloneData.Containers.AddRange(Containers.Select(i => i.Clone()));
        
        cloneData.minimapComponentData = new(minimapComponentData);

        cloneData.Initialized = Initialized;
        return cloneData;
    }

    [Serializable]
    public struct Enemy
    {
        public int UniqueID;
        public Vector3 Position;
        public Quaternion Rotation;
        public int CurrentHP;
        public string PrefabID;

        public Enemy(int uniqueID, Vector3 pos, Quaternion rotation, int currentHP, string prefabID)
        {
            UniqueID = uniqueID;
            Position = pos;
            Rotation = rotation;
            CurrentHP = currentHP;
            PrefabID = prefabID;
        }
    }

    [Serializable]
    public class Container
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public InventorySaveData Inventory;
        public string PrefabID;
        public float RefillChance;
        public List<LootPoolData> LootPoolDatas;

        public Container(Vector3 pos, Quaternion rotation, InventorySaveData inventory, string prefabID,
            float refillChance, List<LootPoolData> lootPoolDatas, bool clone = false)
        {
            Position = pos;
            Rotation = rotation;
            PrefabID = prefabID;
            RefillChance = refillChance;

            if(clone)
            {
                LootPoolDatas = new(lootPoolDatas);
                Inventory = inventory.Clone();
            }
            else
            {
                LootPoolDatas = lootPoolDatas;
                Inventory = inventory;
            }
        }

        public Container Clone()
        {
            return new Container(Position, Rotation, Inventory.Clone(), PrefabID,
                RefillChance, LootPoolDatas, true);
        }
    }

    [Serializable]
    public struct DroppedItem
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public string ItemID;

        public DroppedItem(Vector3 pos, Quaternion rotation, string ID)
        {
            Position = pos;
            Rotation = rotation;
            this.ItemID = ID;
        }
    }
}
