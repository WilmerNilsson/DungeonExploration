using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomItemGiver : MonoBehaviour
{
    [SerializeField] private InventoryGrid inventoryGrid;
    [SerializeField] private LootPoolSO lootPool;
    [SerializeField] private List<string> lootPoolName;
    [SerializeField] private Vector2Int lootAmountRange = new Vector2Int(0, 1);

    private bool selfInitialize = true;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (inventoryGrid == null) Debug.LogWarning("inventory grid is null", this);
        if (lootPool == null) Debug.LogWarning("loot pool is null", this);

        if(lootAmountRange.x < 0)
        {
            Debug.LogWarning("loot amount range min value is less than 0",this);
            lootAmountRange.x = 0;
        }
        if(lootAmountRange.y < lootAmountRange.x)
        {
            Debug.LogWarning("loot amount range max is less than min", this);
            lootAmountRange.y = lootAmountRange.x;
        }
    }
#endif

    private void Start()
    {
        if (selfInitialize)
        {
            GiveRandomLoot(lootPoolName);
        }
    }

    public void StopSelfIntialize()
    {
        selfInitialize = false;
    }

    public void Initialize(List<string> inLootPoolName, int inMinLoot, int inMaxLoot)
    {
        selfInitialize = false;

        lootPoolName = inLootPoolName;
        lootAmountRange = new(inMinLoot, inMaxLoot);

        GiveRandomLoot(lootPoolName);
    }

    /// <summary>
    /// not a copy, be carefull not to change anything
    /// </summary>
    public List<string> GetLootPoolNames()
    {
        return lootPoolName;
    }

    public Vector2Int GetLootAmountRange()
    {
        return lootAmountRange;
    }

    private void GiveRandomLoot(List<string> lootName)
    {
        if (lootName.Count == 0)
        {
            //commenting this out since 
            //Debug.LogWarning("This chest has no loot pool", this);
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
