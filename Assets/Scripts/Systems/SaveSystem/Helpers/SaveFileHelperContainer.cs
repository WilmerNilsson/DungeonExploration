using UnityEngine;


public class SaveFileHelperContainer : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float refillChance = 0.2f;
    [Header("Save file references")]
    [SerializeField] private InventoryGrid grid;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private string prefabID;
    [SerializeField] private ItemLibrarySO itemLibrary;
    [SerializeField] private RandomItemGiver randomItemGiver;

#nullable enable

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (grid == null) Debug.LogWarning("Helper grid is null", this);
        if (spawnTransform == null) Debug.LogWarning("Helper spawn tranform is null", this);
        if (prefabID == null || prefabID == string.Empty) Debug.LogWarning("Helper prefab ID is empty", this);
    }
#endif

    public bool ShouldRespawn(float dataRespawnChance)
    {
        return Random.value <= dataRespawnChance;
    }

    public void Intialize(DungeonSaveData.Container data)
    {
        spawnTransform.position = data.Position;
        spawnTransform.rotation = data.Rotation;

        randomItemGiver.StopSelfIntialize();
        refillChance = data.RefillChance;

        if(data.Inventory.IsEmpty() && data.RefillChance != 0f && ShouldRespawn(data.RefillChance))
        {
            //note that we could stop self initialize even if it should not respawn, but since prefab will not have a loot pool it is fine not to.
            //if the prefab does have a loot pool then remove that instead of changing this.

            randomItemGiver.Initialize(data.LootPoolNames, data.MinItemSpawn, data.MaxItemSpawn);
        }
        else
        {
            PopulateInventory(itemLibrary, data.Inventory, grid);
        }
    }
    
    public static void PopulateInventory(ItemLibrarySO library, InventorySaveData inventory, InventoryGrid grid)
    {
        foreach (InventorySaveData.InventoryItem item in inventory.Items)
        {
            if(library.TryGetItemPairByName(item.PrefabID, out var pair))
            {
                if (grid.TryInstantiateItemInSlot(item.Slot, pair.UIPrefab, out SimpleItem? newItem))
                {
                    if(newItem.TryGetComponent(out IExtraDataHelper helper))
                    {
                        helper.GiveExtraData(item.ExtraJsonSerializeData);
                    }
                }
                else
                {
                    Debug.LogError($"failed to initialize item by name {item.PrefabID} in slot {item.Slot}");
                    continue;
                }
            }
            else
            {
                Debug.LogError("failed to get item by name: " + item.PrefabID);
                continue;
            }

        }
    }

    public DungeonSaveData.Container GetData()
    {
        Vector3 pos = spawnTransform.position;
        Quaternion rot = spawnTransform.rotation;
        InventorySaveData inv = new(grid.GetInventoryData());

        Vector2Int range = randomItemGiver.GetLootAmountRange();

        DungeonSaveData.Container data = new(pos, rot, inv, prefabID, refillChance, range.x, range.y, randomItemGiver.GetLootPoolNames());

        return data;
    }
}
