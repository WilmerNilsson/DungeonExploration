using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomItemGiver : MonoBehaviour
{
    [SerializeField] private InventoryGrid inventoryGrid;
    [SerializeField] private LootPoolSO lootPool;
    //[SerializeField] private List<string> lootPoolName;
    //[SerializeField] private Vector2Int lootAmountRange = new Vector2Int(0, 1);
    
    [SerializeField] private List<LootPoolData> lootPoolData;

    private bool selfInitialize = true;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (inventoryGrid == null) Debug.LogWarning("inventory grid is null", this);
        if (lootPool == null) Debug.LogWarning("loot pool is null", this);

        /*if(lootAmountRange.x < 0)
        {
            Debug.LogWarning("loot amount range min value is less than 0",this);
            lootAmountRange.x = 0;
        }
        if(lootAmountRange.y < lootAmountRange.x)
        {
            Debug.LogWarning("loot amount range max is less than min", this);
            lootAmountRange.y = lootAmountRange.x;
        }*/
    }
#endif

    private void Start()
    {
        if (selfInitialize)
        {
            LootThing();
        }
    }

    public void StopSelfIntialize()
    {
        selfInitialize = false;
    }

    public void Initialize(List<LootPoolData> lootPoolDatas)
    {
        selfInitialize = false;

        lootPoolData = new List<LootPoolData>(lootPoolDatas);
        
        LootThing();
    }

    /// <summary>
    /// not a copy, be carefull not to change anything
    /// </summary>
    /*public List<string> GetLootPoolNames()
    {
        return lootPoolName;
    }*/

    public List<LootPoolData> GetLootPoolDatas()
    {
        return lootPoolData;
    }

    /*public Vector2Int GetLootAmountRange()
    {
        return lootAmountRange;
    }*/

    private void LootThing()
    {
        for (int i = 0; i < lootPoolData.Count; i++)
        {
            int lootAmount = Random.Range(lootPoolData[i].itemAmountRange.x, lootPoolData[i].itemAmountRange.y);
            LootPool currentPool = lootPool.lootPools.Find(x => x.name == lootPoolData[i].lootPoolName);
            if (currentPool != null) GiveRandomLoot(currentPool, lootAmount);
        }
    }

    private void GiveRandomLoot(LootPool currentPool, int itemAmount)
    {
        int totalWeight = 0;
        for (int i = 0; i < currentPool.items.Count; i++)
        {
            totalWeight += currentPool.items[i].weight;
        }
        
        for (int i = 0; i < itemAmount; i++)
        {
            int randomIndex = Random.Range(1, totalWeight + 1);
            int currentWeight = 0;

            for (int j = 0; currentWeight < totalWeight; j++)
            {
                if (currentWeight <= randomIndex && randomIndex <= currentWeight + currentPool.items[j].weight) //check if index is in range
                {
                    if (currentPool.items[j].UIPrefab != null)
                    {
                        inventoryGrid.TryInsertItem(currentPool.items[j].UIPrefab, true);
                    }
                    break;
                }
                currentWeight += currentPool.items[j].weight;
            }
        }
    }
}
