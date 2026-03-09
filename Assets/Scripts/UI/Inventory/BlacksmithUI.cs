using System.Collections.Generic;
using UnityEngine;
using static Codice.Client.Common.Connection.AskCredentialsToUser;

public class BlacksmithUI : MerchantInventory
{
    [SerializeField] private GameObject deliverWeaponPanel;
    [SerializeField] private string weaponAlreadyDonatedText = "Already have those.";
    [SerializeField] private string couldDonateText = "Interesting, i will keep this stocked.";
    [SerializeField] private string couldRepairWeaponText = "CLANK CLANK CLANK CLANK GET FIXED GET FIXED GET FIXED";
    [SerializeField] private string weaponTypeCantBeDonatedText = "That knife too cool for my school bro.";
    [SerializeField] private string weaponAlreadyFullDurabilityText = "Looks fine to me.";
    [SerializeField] private string weaponCostsToMuchToRepairText = "Sorry, Ingrid. I can't give credit. Come back when you're a little... mmmmm... richer!";
#nullable enable

    private UIWeapon? weaponInDonateGrid;
    private SimpleItem? weaponInDonateGridSI;

    private List<string> donatedWeapons = new();

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (deliverWeaponPanel == null) Debug.LogWarning("deliver weapon panel is null", this);
    }
#endif

    public List<string> GetSaveData()
    {
        return donatedWeapons; 
    }

    public void GiveSaveData(List<string> donatedWeapons)
    {
        this.donatedWeapons = donatedWeapons;
        foreach(string weapon in donatedWeapons)
        {
            if (weapon != null)
            {
                if(itemLibrary.TryGetItemPairByName(weapon, out var pair))
                {
                    if (!buyGrid.TryInsertItem(pair.UIPrefab.GetComponent<SimpleItem>(), true))
                    {
                        Debug.LogWarning("failed to instanciate item in buy grid", this);
                    }
                }
                else
                {
                    Debug.LogWarning("invalid weapon id given in save data to BlacksmithUI, id: " + weapon, this);
                }
            }
            else
            {
                Debug.LogWarning("null weapon given in save data to BlacksmithUI", this);
            }
        }
    }

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

    public void OnGetNewItemInDonationGrid(SimpleItem item)
    {
        if(item.TryGetComponent(out UIWeapon weapon))
        {
            weaponInDonateGrid = weapon;
            weaponInDonateGridSI = item;
        }
        else
        {
            Debug.LogWarning("non weapon put in donation grid", this);
        }
    }

    public void OnRemoveItemFromDonatioNGrid(SimpleItem item)
    {
        if (item.TryGetComponent(out UIWeapon weapon))
        {
            if(weaponInDonateGrid == weapon)
            {
                weaponInDonateGrid = null;
                weaponInDonateGridSI = null;
            }
            else
            {
                Debug.LogWarning("weapon taken from donation grid that was not the tracked weapon", this);
            }
        }
        else
        {
            Debug.LogWarning("non weapon removed from donation grid", this);
        }
    }

    public override bool TryPutItemInMerchantGrid(SimpleItem item)
    {
        if (buyIsActiveGrid)
        {
            return false;
        }
        else if (item.TryGetComponent<UIWeapon>(out _) && ActiveGrid.TryPlaceItem(item))
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
        if(buyIsActiveGrid)
        {
            Debug.LogWarning("active grid is buy, but on donate invoked", this);
        }
        else if (weaponInDonateGridSI != null && weaponInDonateGrid != null)
        {
            if(!weaponInDonateGrid.BlacksmithHelper.CanBeDonated)
            {
                SetDescriptionText(weaponTypeCantBeDonatedText);
                return;
            }

            if(donatedWeapons.Contains(weaponInDonateGridSI.PrefabID))
            {
                SetDescriptionText(weaponAlreadyDonatedText);
            }
            else
            {
                SetDescriptionText(couldDonateText);
                donatedWeapons.Add(weaponInDonateGridSI.PrefabID);

                if (itemLibrary.TryGetItemPairByName(weaponInDonateGridSI.PrefabID, out var pair))
                {
                    if(!buyGrid.TryInsertItem(pair.UIPrefab.GetComponent<SimpleItem>(), true))
                    {
                        Debug.LogWarning("failed to instanciate item in buy grid", this);
                    }
                }
                else
                {
                    Debug.LogWarning("invalid weapon id donated to blacksmit, id: " + weaponInDonateGridSI.PrefabID, this);
                }
            }
        }
    }

    public void OnRepair()
    {
        if (buyIsActiveGrid)
        {
            Debug.LogWarning("active grid is buy, but on repair invoked", this);
        }
        else if (weaponInDonateGrid != null)
        {
            int repairCost = (weaponInDonateGrid.BlacksmithHelper.MaxDurability - weaponInDonateGrid.Durability) * weaponInDonateGrid.BlacksmithHelper.CostPerDurability;

            if(weaponInDonateGrid.Durability == weaponInDonateGrid.BlacksmithHelper.MaxDurability)
            {
                SetDescriptionText(weaponAlreadyFullDurabilityText);
            }
            else if(playerCashSO.TryBuy(repairCost))
            {
                SetDescriptionText(couldRepairWeaponText);
                weaponInDonateGrid.Durability = weaponInDonateGrid.BlacksmithHelper.MaxDurability;
            }
            else
            {
                SetDescriptionText(weaponCostsToMuchToRepairText);
            }
        }
    }
}
