using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonSaveData
{
    public List<DroppedItem> DroppedItems = new();
    public List<Enemy> Enemies = new();
    public List<Container> Containers = new();

    internal DungeonSaveData Clone()
    {
        DungeonSaveData cloneData = new();
        cloneData.DroppedItems = new(DroppedItems);
        cloneData.Enemies = new(Enemies);
        cloneData.Containers = new(Containers.Count);

        cloneData.Containers.AddRange(Containers.Select(i => i.Clone()));
        return cloneData;
    }

    public struct Enemy
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public int CurrentHP;
        public string PrefabID;

        public Enemy(Vector3 pos, Quaternion rotation, int currentHP, string prefabID)
        {
            Position = pos;
            Rotation = rotation;
            CurrentHP = currentHP;
            PrefabID = prefabID;
        }
    }

    public class Container
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public InventorySaveData Inventory;
        public string PrefabID;

        public Container(Vector3 pos, Quaternion rotation, InventorySaveData inventory, string prefabID)
        {
            Position = pos;
            Rotation = rotation;
            Inventory = inventory;
            PrefabID = prefabID;
        }

        public Container Clone()
        {
            return new Container(Position, Rotation, Inventory.Clone(), PrefabID);
        }
    }

    public struct DroppedItem
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public string ID;

        public DroppedItem(Vector3 pos, Quaternion rotation, string ID)
        {
            Position = pos;
            Rotation = rotation;
            this.ID = ID;
        }
    }
}
