using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private float cameraDropOffset;
    [SerializeField] private ItemLibrarySO itemLibrary;
    [SerializeField] private SimpleItem myItem;

#nullable enable

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (itemLibrary == null)
        {
            Debug.LogWarning("item library is null", this);
        }
        else if(myItem != null && myItem.PrefabID != null && !itemLibrary!.TryGetItemPairByName(myItem.PrefabID, out _))
        {
            Debug.LogWarning("item library does not have entry of: " + myItem?.PrefabID, this);
        }

        
        if (myItem == null) Debug.LogWarning("my item is null", this);
    }
#endif

    public void Drop()
    {
        Vector3 dropPos = Camera.main.transform.position;

        dropPos.y += cameraDropOffset;

        //since we check this is valid on validate we can assume it will not be null
        itemLibrary.TryGetItemPairByName(myItem.PrefabID, out ItemPairing? pair);

        Instantiate(pair?.WorldPrefab, dropPos, Quaternion.identity);

        InvMaster.Instance.DestroyItem(myItem);
    }
}
