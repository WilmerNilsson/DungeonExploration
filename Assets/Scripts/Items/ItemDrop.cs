using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private float floorOffset;
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
        Physics.Raycast(Camera.main.transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground", "Object"));
        Vector3 dropPos = hit.point;

        dropPos.y += floorOffset;

        //since we check this is valid on validate we can assume it will not be null
        itemLibrary.TryGetItemPairByName(myItem.PrefabID, out ItemPairing? pair);

        if(TryGetComponent(out IHaveDurability myDurability))
        {
            if(Instantiate(pair?.WorldPrefab, dropPos, Quaternion.identity)!.TryGetComponent(out IHaveDurability droppedDurability))
            {
                droppedDurability.StopSelfIntialize();
                droppedDurability.Durability = myDurability.Durability;
            }
        }
        else
        {
            Instantiate(pair?.WorldPrefab, dropPos, Quaternion.identity);
        }

        InvMaster.Instance.DestroyItem(myItem);
    }
}
