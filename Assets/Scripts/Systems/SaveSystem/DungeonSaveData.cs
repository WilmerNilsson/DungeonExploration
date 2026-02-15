using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonSaveData
{
    public List<DroppedItem> DroppedItems = new();
    public List<Enemy> Enemies = new();
    public List<Container> containers = new();

    internal DungeonSaveData Clone()
    {
        DungeonSaveData cloneData = new();
        cloneData.DroppedItems = new(DroppedItems);
        cloneData.Enemies = new(Enemies);
        cloneData.containers = new(containers.Count);

        cloneData.containers.AddRange(containers.Select(i => i.Clone()));
        return cloneData;
    }

    public struct Enemy
    {
        public Vector3 Pos;
        public Quaternion Rotation;
        public int CurrentHP;
        //enum or string ID for type

        public Enemy(Vector3 pos, Quaternion rotation, int currentHP)
        {
            Pos = pos;
            Rotation = rotation;
            CurrentHP = currentHP;
        }
    }

    public class Container
    {
        public Vector3 Pos;
        public Quaternion Rotation;
        public InventorySaveData Inventory;

        public Container(Vector3 pos, Quaternion rotation, InventorySaveData inventory)
        {
            Pos = pos;
            Rotation = rotation;
            Inventory = inventory;
        }

        public Container Clone()
        {
            return new Container(Pos, Rotation, Inventory.Clone());
        }
    }

    public struct DroppedItem
    {
        public Vector3 Pos;
        public string Name;

        public DroppedItem(Vector3 pos, string name)
        {
            Pos = pos;
            Name = name;
        }
    }
}
