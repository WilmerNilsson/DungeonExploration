using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootPool
{
    public string name;
    public int weight = 1;
    public List<Loot> items;
}

[System.Serializable]
public class Loot
{
    [Min(1)]
    public int weight = 1;
    public SimpleItem UIPrefab;

    /*public Loot(int weight, SimpleItem uiPrefab)
    {
        this.weight = Mathf.Max(weight, 1);
        this.UIPrefab = uiPrefab;
    }*/
}