using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private GameObject inventoryItem;

#if DEBUG
    private void OnValidate()
    {
        if (inventoryItem == null)
        {
            Debug.LogWarning("inventory item is null", this);
        }
        else if(inventoryItem.GetComponent<SimpleItem>() == null)
        {
            Debug.LogWarning("inventory item is not a SimpleItem", this);
        }
    }
#endif

    /// <summary>
    /// puts item into player inventory if there is space
    /// </summary>
    public void PickUp()
    {
        SimpleItem item = inventoryItem.GetComponent<SimpleItem>();

        if (InvMaster.Instance.PlayerInventory.TryInsertItem(item, true))
        {
            Destroy(gameObject);
        }
    }
}
