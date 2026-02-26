using UnityEngine;

#nullable enable

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private float cameraDropOffset;
    [SerializeField] private string prefabID;
    [SerializeField] private ItemLibrarySO itemLibrary;
    [SerializeField] private SimpleItem myItem;

#nullable enable

#if DEBUG
    private void OnValidate()
    {
        bool hasPrefabID = !(prefabID == null || prefabID == string.Empty);
        bool hasLibrary = itemLibrary != null;

        if (!hasPrefabID)
        {
            Debug.LogWarning("prefabID is empty", this);
        }

        if (!hasLibrary)
        {
            Debug.LogWarning("item library is null", this);
        }
        else if(hasPrefabID && !itemLibrary!.TryGetItemPairByName(prefabID, out _))
        {
            Debug.LogWarning("item library does not have entry of: " + prefabID, this);
        }

        
        if (myItem == null) Debug.LogWarning("my item is null", this);
    }
#endif

    public void Drop()
    {
        Vector3 dropPos = Camera.main.transform.position;

        dropPos.y += cameraDropOffset;

        //since we check this is valid on validate we can assume it will not be null
        itemLibrary.TryGetItemPairByName(prefabID, out ItemPairing? pair);

        Instantiate(pair?.WorldPrefab, dropPos, Quaternion.identity);

        InvMaster.Instance.DestroyItem(myItem);
    }
}
