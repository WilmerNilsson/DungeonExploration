using System.Collections.Generic;
using UnityEngine;

public class DungeonSaveData
{
    public List<DroppedItem> DroppedItems = new();
    public List<Enemy> Enemies = new();
    public List<Container> containers = new();

    public struct Enemy
    {
        public Vector3 Pos;
        public Quaternion Rotation;
        public int CurrentHP;

        public Enemy(Vector3 pos, Quaternion rotation, int currentHP)
        {
            Pos = pos;
            Rotation = rotation;
            CurrentHP = currentHP;
        }
    }

    public struct Container
    {
        public Vector3 Pos;
        public Quaternion Rotation;
        public InventorySaveData inventory;

        public Container(Vector3 pos, Quaternion rotation, InventorySaveData inventory)
        {
            Pos = pos;
            Rotation = rotation;
            this.inventory = inventory;
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
