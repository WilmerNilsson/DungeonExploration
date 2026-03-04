using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private string prefabID;
    [SerializeField] private ItemLibrarySO itemLibrary;

    public string ItemID
    {
        get { return prefabID; }
    }

#if DEBUG && UNITY_EDITOR
    private void OnValidate()
    {
        if (itemLibrary == null)
        {
            Debug.LogWarning("inventory library is null", this);
        }
        else if(!itemLibrary.TryGetItemPairByName(prefabID, out _))
        {
            Debug.LogWarning("item library had no item by name: " + prefabID, this);
        }
    }
#endif

    /// <summary>
    /// puts item into player inventory if there is space
    /// </summary>
    public void PickUp()
    {
        itemLibrary.TryGetItemPairByName(prefabID, out ItemPairing pair);

        if (InvMasterBase.Instance.PlayerInventory.TryInsertItem(pair.UIPrefab.GetComponent<SimpleItem>(), true))
        {
            Destroy(gameObject);
        }
    }
}
