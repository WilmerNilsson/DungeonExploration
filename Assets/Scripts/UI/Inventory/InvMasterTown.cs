using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;

public class InvMasterTown : InvMasterBase
{
    [SerializeField] private MerchantInventory merchantInventory;
    [SerializeField] private TextMeshProUGUI itemGoldValueText;
#nullable enable

    protected override void Start()
    {
        base.Start();
    }

#if DEBUG && UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (merchantInventory == null) Debug.LogWarning("merchant inventory is null", this);
    }
#endif

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

        if(merchantInventory.HasItem(item) && merchantInventory.CanAfford(item))
        {
            if(PlayerInventory.TryPlaceItem(item))
            {
                inventoryGrid = PlayerInventory;
                return true;
            }
            else if (EquipmentGrid.TryPlaceItem(item))
            {
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
            else //try sell
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
