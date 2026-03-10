using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomItemGiver : MonoBehaviour
{
    private InventoryGrid inventoryGrid;
    [SerializeField] private LootPoolSO lootPool;
    [SerializeField] private string lootPoolName;
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

    private void GiveRandomLoot(string lootName)
    {
        LootPool currentPool = lootPool.lootPools.Find(x => x.name == lootName);
        if (currentPool == null)
        {
            Debug.LogWarning("No LootPool found", gameObject);
            return;
        }
        
        int totalWeight = 0;
        for (int i = 0; i < currentPool.items.Count; i++)
        {
            Debug.Log("add weight");
            totalWeight += currentPool.items[i].weight;
        }
        Debug.Log(totalWeight);
        
        int itemAmount = Random.Range(lootAmountRange.x, lootAmountRange.y);
        Debug.Log(itemAmount);
        for (int i = 0; i < itemAmount; i++)
        {
            int randomIndex = Random.Range(1, totalWeight + 1);
            Debug.Log(randomIndex);
            int currentWeight = 0;

            for (int j = 0; currentWeight <= totalWeight; j++)
            {
                if (currentWeight <= randomIndex && randomIndex <= currentWeight + currentPool.items[j].weight) //check if index is in range
                {
                    if (currentPool.items[j].UIPrefab != null)
                    {
                        Debug.Log("true");
                        inventoryGrid.TryInsertItem(currentPool.items[j].UIPrefab, true);
                    }
                    break;
                }
                currentWeight += currentPool.items[j].weight;
            }
        }
        
        
    }
}
