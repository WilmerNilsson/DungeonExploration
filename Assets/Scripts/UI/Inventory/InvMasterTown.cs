using TMPro;
using UnityEngine;

public class InvMasterTown : InvMasterBase
{
    [SerializeField] private MerchantInventory merchantInventory;
    [SerializeField] private TextMeshProUGUI itemGoldValueText;

    protected override void Start()
    {
        base.Start();
    }

#if DEBUG
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

    public override bool TryPlaceItem(SimpleItem item)
    {
        //perhaps have a reference in SimpleItem to current inventory that we can remove it from
        //when we move it from one inventory to another.


        // we check if merchant has the item first so player can re-arrange thier inventory
        // without interfearense

        if(merchantInventory.HasItem(item))
        {
            if(merchantInventory.CanAfford(item) && PlayerInventory.TryPlaceItem(item))
            {
                merchantInventory.TryRemoveSlottedItem(item);
                return true;
            }
        }
        else if(PlayerInventory.HasItem(item))
        {
            if(PlayerInventory.TryPlaceItem(item)) //re-arrenge
            {
                return true;
            }
            else //try sell
            {
                if(merchantInventory.TrySellItem(item))
                {
                    PlayerInventory.TryRemoveSlottedItem(item);
                    return true;
                }
            }
        }
        return false;
    }
}
