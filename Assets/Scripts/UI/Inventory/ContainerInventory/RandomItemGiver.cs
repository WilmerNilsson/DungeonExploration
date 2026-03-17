using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomItemGiver : MonoBehaviour
{
    private InventoryGrid inventoryGrid;
    [SerializeField] private LootPoolSO lootPool;
    [SerializeField] private List<string> lootPoolName;
    [SerializeField] private Vector2Int lootAmountRange = new Vector2Int(0, 1);

    private void Awake()
    {
        if (TryGetComponent(out inventoryGrid))
        {
            GiveRandomLoot(lootPoolName);
        }
        else
        {
            Debug.LogWarning("No InventoryGrid found", gameObject);
        }
    }

    private void GiveRandomLoot(List<string> lootName)
    {
        if (lootName.Count == 0)
        {
            Debug.LogWarning("This chest has no loot pool", this);
            return;
        }
        List<LootPool> currentLootPools = new List<LootPool>();
        LootPool currentPool = null;
        for (int i = 0; i < lootName.Count; i++)
        {
            currentLootPools.Add(lootPool.lootPools.Find(x => x.name == lootName[i]));
        }
        
        int totalWeight = 0;
        for (int i = 0; i < currentLootPools.Count; i++)
        {
            totalWeight += currentLootPools[i].weight;
        }
        
        int itemAmount = Random.Range(lootAmountRange.x, lootAmountRange.y);
        for (int i = 0; i < itemAmount; i++)
        {
            int randomIndex = Random.Range(1, totalWeight + 1);
            int currentWeight = 0;

            for (int j = 0; currentWeight <= totalWeight; j++)
            {
                if (currentWeight <= randomIndex && randomIndex <= currentWeight + currentLootPools[j].weight) //check if index is in range
                {
                    currentPool = currentLootPools[j];
                }
                currentWeight += currentPool.items[j].weight;
            }
        }
        
        if (currentPool == null)
        {
            Debug.LogWarning("No LootPool found", gameObject);
            return;
        }
        
        totalWeight = 0;
        for (int i = 0; i < currentPool.items.Count; i++)
        {
            totalWeight += currentPool.items[i].weight;
        }
        
        itemAmount = Random.Range(lootAmountRange.x, lootAmountRange.y);
        for (int i = 0; i < itemAmount; i++)
        {
            int randomIndex = Random.Range(1, totalWeight + 1);
            int currentWeight = 0;

            for (int j = 0; currentWeight <= totalWeight; j++)
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
