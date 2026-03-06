using System.Collections.Generic;
using UnityEngine;

public class BlacksmithUI : MerchantInventory
{
    [SerializeField] private GameObject deliverWeaponPanel;


#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (deliverWeaponPanel == null) Debug.LogWarning("deliver weapon panel is null", this);
    }
#endif

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

    public override bool TryPutItemInMerchantGrid(SimpleItem item)
    {
        if (buyIsActiveGrid)
        {
            return false;
        }
        else if (ActiveGrid.TryPlaceItem(item))
        {
            return true;
        }
        return false;
    }

    public override bool CanAfford(SimpleItem item)
    {
        if (buyIsActiveGrid)
        {
            return base.CanAfford(item);
        }
        else return true;
    }

    public override void BuyItem(SimpleItem item)
    {
        if (buyIsActiveGrid)
        {
            base.BuyItem(item);
        }
        //else do nothing since we are not going to buy from repair grid
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

            deliverWeaponPanel.SetActive(false);
        }
        else
        {
            deliverWeaponPanel.SetActive(true);
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
