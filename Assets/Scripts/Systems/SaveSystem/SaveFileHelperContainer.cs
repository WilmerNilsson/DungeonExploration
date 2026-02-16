using System;
using UnityEngine;


public class SaveFileHelperContainer : MonoBehaviour
{
    [SerializeField] private InventoryGrid grid;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private string prefabID;
    [SerializeField] private ContainerDLibrarySO containerLibrary;
    [SerializeField] private ItemLibrarySO itemLibrary;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (grid == null) Debug.LogWarning("Helper grid is null", this);
        if (spawnTransform == null) Debug.LogWarning("Helper spawn tranform is null", this);

        bool nameEmpty = prefabID == null || prefabID == string.Empty;

        if (nameEmpty) Debug.LogWarning("Helper prefab ID is empty", this);
        if (containerLibrary == null) Debug.LogWarning("Helper library is null", this);
        else if (!nameEmpty && !containerLibrary.TryGetPrefabByName(prefabID, out _)) Debug.LogWarning("library has no id of " + prefabID, this);
    }
#endif

    public void Intialize(DungeonSaveData.Container data)
    {
        spawnTransform.position = data.Pos;
        spawnTransform.rotation = data.Rotation;

        foreach(var item in data.Inventory.Items)
        {
#if DEBUG
            bool couldGetItem = itemLibrary.TryGetItemPairByName(item.Name, out var pair);

            if(!couldGetItem)
            {
                Debug.LogError("failed to get item by name: " + item.Name, this);
                continue;
            }
            if(!grid.TryInstantiateItemInSlot(item.Slot, pair.UIPrefab))
            {
                Debug.LogError($"failed to initialize item by name {item.Name} in slot {item.Slot}", this);
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
