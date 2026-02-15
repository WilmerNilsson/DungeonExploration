using System;
using UnityEngine;


public class SaveFileHelperContainer : MonoBehaviour
{
    [SerializeField] private InventoryGrid grid;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private string prefabID;
    [SerializeField] private ContainerDLibrarySO library;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (grid == null) Debug.LogWarning("Helper grid is null", this);
        if (spawnTransform == null) Debug.LogWarning("Helper spawn tranform is null", this);

        bool nameEmpty = prefabID == null || prefabID == string.Empty;

        if (nameEmpty) Debug.LogWarning("Helper prefab ID is empty", this);
        if (library == null) Debug.LogWarning("Helper library is null", this);
        else if (!nameEmpty && !library.TryGetPrefabByName(prefabID, out _)) Debug.LogWarning("library has no id of " + prefabID, this);
    }
#endif

    public void Intialize(DungeonSaveData.Container data)
    {
        
    }

    public DungeonSaveData.Container GetData()
    {
        throw new NotImplementedException();

        Vector3 pos = spawnTransform.position;
        Quaternion rot = spawnTransform.rotation;
        InventorySaveData inv = new();
        
        //grid.

        //DungeonSaveData.Container data = new();
    }
}
