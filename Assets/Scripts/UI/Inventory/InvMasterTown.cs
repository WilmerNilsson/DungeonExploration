using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;

public class InvMasterTown : InvMasterBase
{
#nullable enable

    private MerchantInventory? merchantInventory;
    private InventoryGrid? QuickLootInventory
    {
        get
        {
            if(merchantInventory == null)
            {
                return null;
            }
            else
            {
                return merchantInventory.ActiveGrid;
            }
        }
    }


    protected override void Start()
    {
        base.Start();
    }

#if DEBUG && UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif

    public void SetActiveMerchantInventory(MerchantInventory merchant)
    {
        merchantInventory = merchant;
        descriptionText = merchant.GetDescriptionTextField();
    }

    public void RemoveActiveMerchantInventory(MerchantInventory merchant)
    {
        if(merchantInventory == merchant)
        {
            merchantInventory = null;
            descriptionText = null;
        }
    }

    public override void ChangeHover(SimpleItem simpleItem, bool startHover)
    {
        base.ChangeHover(simpleItem, startHover);
        merchantInventory?.ChangeHover(simpleItem, startHover);
    }

    public override bool TryQuickLootItem(SimpleItem item)
    {
        if (!DoQuickLoot || merchantInventory == null) return false;

        if (item.GridIsCurrent(PlayerInventory) || item.GridIsCurrent(EquipmentGrid))
        {
            return merchantInventory.TryInsertItemInMerchantGrid(item);
        }
        else if(item.GridIsCurrent(merchantInventory.ActiveGrid) && merchantInventory.CanAfford(item))
        {
            if (item.QuickLootToEquip)
            {
                if (EquipmentGrid.TryInsertItem(item))
                {
                    merchantInventory.BuyItem(item);
                    return true;
                }
                else if(PlayerInventory.TryInsertItem(item))
                {
                    merchantInventory.BuyItem(item);
                    return true;
                }
            }
            else if (PlayerInventory.TryInsertItem(item))
            {
                merchantInventory.BuyItem(item);
                return true;
            }

            return false;
        }
        return false;
    }


    public override bool TryPlaceItem(SimpleItem item, [NotNullWhen(true)] out InventoryGrid? inventoryGrid)
    {
        // we check if merchant has the item first so player can re-arrange thier inventory
        // without interfearense

        if(merchantInventory != null && merchantInventory.HasItem(item) && merchantInventory.CanAfford(item))
        {
            if(PlayerInventory.TryPlaceItem(item) || EquipmentGrid.TryPlaceItem(item))
            {
                merchantInventory.BuyItem(item);
                inventoryGrid = PlayerInventory;
                return true;
            }
        }
        else if(PlayerInventory.HasItem(item) || EquipmentGrid.HasItem(item))
        {
            if(PlayerInventory.TryPlaceItem(item)) //re-arrenge
            {
                inventoryGrid = PlayerInventory;
                return true;
            }
            else if(EquipmentGrid.TryPlaceItem(item))
            {
                inventoryGrid = EquipmentGrid;
                return true;
            }
            else if(merchantInventory != null)
            {
                if(merchantInventory.TryPutItemInMerchantGrid(item))
                {
                    inventoryGrid = merchantInventory.ActiveGrid;
                    return true;
                }
            }
        }
        inventoryGrid = null;
        return false;
    }
}
