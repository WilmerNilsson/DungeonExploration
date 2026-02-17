using System;
using UnityEngine;


public class SaveFileHelperContainer : MonoBehaviour
{
    [SerializeField] private InventoryGrid grid;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private string prefabID;
    [SerializeField] private ItemLibrarySO itemLibrary;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (grid == null) Debug.LogWarning("Helper grid is null", this);
        if (spawnTransform == null) Debug.LogWarning("Helper spawn tranform is null", this);
        if (prefabID == null || prefabID == string.Empty) Debug.LogWarning("Helper prefab ID is empty", this);
    }
#endif

    public void Intialize(DungeonSaveData.Container data)
    {
        spawnTransform.position = data.Position;
        spawnTransform.rotation = data.Rotation;

        PopulateInventory(itemLibrary, data.Inventory, grid);
    }

    public static void PopulateInventory(ItemLibrarySO library, InventorySaveData inventory, InventoryGrid grid)
    {
        foreach (var item in inventory.Items)
        {
#if DEBUG
            bool couldGetItem = library.TryGetItemPairByName(item.Name, out var pair);

            if (!couldGetItem)
            {
                Debug.LogError("failed to get item by name: " + item.Name);
                continue;
            }
            if (!grid.TryInstantiateItemInSlot(item.Slot, pair.UIPrefab))
            {
                Debug.LogError($"failed to initialize item by name {item.Name} in slot {item.Slot}");
            }
#else
            itemLibrary.TryGetItemPairByName(item.Name, out var pair);
            grid.TryInstantiateItemInSlot(item.Slot, pair.UIPrefab);
#endif
        }
    }

    public DungeonSaveData.Container GetData()
    {
        Vector3 pos = spawnTransform.position;
        Quaternion rot = spawnTransform.rotation;
        InventorySaveData inv = new(grid.GetInventoryData());

        DungeonSaveData.Container data = new(pos, rot, inv, prefabID);

        return data;
    }
}
