using System.Collections.Generic;
using UnityEngine;

public class BlacksmithUI : MerchantInventory
{
    public override void ChangeHover(SimpleItem simpleItem, bool startHover)
    {
        if(buyIsActiveGrid)
        {
            base.ChangeHover(simpleItem, startHover);
        }
        else
        {
            SetDescriptionText("repair cost is OwO and 3 fiddy");
        }
    }

    public override void SelectGrid(bool buy)
    {
        base.SelectGrid(buy);

        if(buyIsActiveGrid)
        {
            //return item to player inventory
            List<SimpleItem> items = sellGrid.EmptyInventory();
            foreach(SimpleItem item in items)
            {
                if(!InvMasterBase.Instance.EquipmentGrid.TryInsertItem(item) && !InvMasterBase.Instance.PlayerInventory.TryInsertItem(item))
                {
                    Debug.LogError("failed to return item from donation grid in blacksmith", this);
                }
            }
        }
    }

    public void OnDonate()
    {
        SetDescriptionText("yoink");
    }

    public void OnRepair()
    {
        SetDescriptionText("looks fine to me");
    }
}
