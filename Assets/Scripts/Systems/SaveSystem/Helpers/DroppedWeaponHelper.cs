using UnityEngine;

public class DroppedWeaponHelper : ItemPickup, IHaveDurability
{
#nullable enable

    public int Durability { get; set; } = -1;

    public void StopSelfIntialize()
    {
        
    }

    public override void PickUp()
    {
        if(itemLibrary.TryGetItemPairByName(prefabID, out ItemPairing? pair))
        {
            if (InvMasterBase.Instance.EquipmentGrid.TryInsertItem(pair.UIPrefab.GetComponent<SimpleItem>(), out SimpleItem? instanciateEquipment, true))
            {
                Destroy(gameObject);
                
                if (Durability != -1 && instanciateEquipment != null && instanciateEquipment.TryGetComponent(out IHaveDurability durability))
                {
                    durability.StopSelfIntialize();
                    durability.Durability = Durability;
                }
                return;
            }
            if (InvMasterBase.Instance.PlayerInventory.TryInsertItem(pair.UIPrefab.GetComponent<SimpleItem>(), out SimpleItem? instanciateItem, true))
            {
                Destroy(gameObject);

                if (Durability != -1 && instanciateItem != null && instanciateItem.TryGetComponent(out IHaveDurability durability))
                {
                    durability.StopSelfIntialize();
                    durability.Durability = Durability;
                }
            }

        }
    }
}
