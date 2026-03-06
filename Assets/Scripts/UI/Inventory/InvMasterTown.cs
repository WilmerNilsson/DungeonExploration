using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;

public class InvMasterTown : InvMasterBase
{
    [SerializeField] private TextMeshProUGUI itemGoldValueText;
#nullable enable

    private MerchantInventory? merchantInventory;

    protected override void Start()
    {
        base.Start();
    }

#if DEBUG && UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (itemGoldValueText == null) Debug.LogWarning("item gold value text is null", this);
    }
#endif

    public void SetActiveMerchantInventory(MerchantInventory merchant)
    {
        merchantInventory = merchant;
    }

    public void RemoveActiveMerchantInventory(MerchantInventory merchant)
    {
        if(merchantInventory == merchant)
        {
            merchantInventory = null;
        }
    }

    public override void ChangeHover(SimpleItem simpleItem, bool startHover)
    {
        base.ChangeHover(simpleItem, startHover);
        if(startHover)
        {
            itemGoldValueText.text = simpleItem.CashValue.ToString();
        }
        else
        {
            itemGoldValueText.text = string.Empty;
        }
    }

    public override bool TryPlaceItem(SimpleItem item, [NotNullWhen(true)] out InventoryGrid? inventoryGrid)
    {
        // we check if merchant has the item first so player can re-arrange thier inventory
        // without interfearense

        if(merchantInventory != null && merchantInventory.HasItem(item) && merchantInventory.CanAfford(item))
        {
            if(PlayerInventory.TryPlaceItem(item))
            {
                merchantInventory.BuyItem(item);
                inventoryGrid = PlayerInventory;
                return true;
            }
            else if (EquipmentGrid.TryPlaceItem(item))
            {
                merchantInventory.BuyItem(item);
                inventoryGrid = EquipmentGrid;
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
                if(merchantInventory.TrySellItem(item))
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
