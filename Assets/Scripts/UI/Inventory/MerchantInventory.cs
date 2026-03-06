using System.Diagnostics.Contracts;
using UnityEngine;

public class MerchantInventory : MonoBehaviour
{
    //the reason i use 2 grids instead of just keeping track of the data is that
    //i want to avoid instansiating and destroying items unneseserarly
    [SerializeField] private InventoryGrid buyGrid;
    [SerializeField] private InventoryGrid sellGrid;
    [SerializeField] private string[] SpawnItems;
    [SerializeField] private ItemLibrarySO itemLibrary;
    [SerializeField] private PlayerCashSO playerCashSO;

#nullable enable

    private void OnEnable()
    {
        if(InvMasterBase.Instance is InvMasterTown town)
        {
            town.SetActiveMerchantInventory(this);
        }
        else
        {
            Debug.LogError("inventory master base is not town", this);
        }
    }

    private void OnDisable()
    {
        if (InvMasterBase.Instance is InvMasterTown town)
        {
            town.RemoveActiveMerchantInventory(this);
        }
        else
        {
            Debug.LogError("inventory master base is not town", this);
        }
    }

    private bool buyIsActiveGrid = true;
    public InventoryGrid ActiveGrid
    {
        get
        {
            if(buyIsActiveGrid)
            {
                return buyGrid;
            }
            else
            {
                return sellGrid;
            }
        }
    }


    private void Start()
    {
#if DEBUG
        if (sellGrid.isActiveAndEnabled)
        {
            Debug.LogWarning("sell grid is active at start, buy should be the default", this);
        }
#endif

        foreach(string spawnItem in SpawnItems)
        {
            if(itemLibrary.TryGetItemPairByName(spawnItem, out ItemPairing? pair))
            {
                buyGrid.TryInsertItem(pair.UIPrefab.GetComponent<SimpleItem>(), true);
            }
        }
    }

#if DEBUG
    private void OnValidate()
    {
        if (buyGrid == null) Debug.LogWarning("Buy inventory grid is null", this);
        if (sellGrid == null) Debug.LogWarning("Sell inventory grid is null", this);
        if (itemLibrary == null)
        {
            Debug.LogWarning("item library is null", this);
        }
        else
        {
            foreach (var item in SpawnItems)
            {
                if (item != null)
                {
                    if(!itemLibrary.TryGetItemPairByName(item, out _))
                    {
                        Debug.LogWarning($"{item} is not a item ID in library", this);
                    }
                }
                else
                {
                    Debug.LogWarning("merchant spawn items has null entry", this);
                }
            }
        }
    }
#endif

    public void SelectGrid(bool buy)
    {
        buyIsActiveGrid = buy;
        if(buyIsActiveGrid)
        {
            buyGrid.gameObject.SetActive(true);
            sellGrid.gameObject.SetActive(false);
        }
        else
        {
            buyGrid.gameObject.SetActive(false);
            sellGrid.gameObject.SetActive(true);
        }
    }

    public bool TrySellItem(SimpleItem item)
    {
        if (buyIsActiveGrid)
        {
            return false;
        }
        else if (HasItem(item)) //we don't want the player re-arrenging merchant inventory
        {
            return false;
        }
        else if(ActiveGrid.TryPlaceItem(item))
        {
            playerCashSO.AddCash(item.CashValue);
            return true;
        }
        else return false;
    }

    public bool HasItem(SimpleItem item)
    {
        return ActiveGrid.HasItem(item);
    }
    public bool CanAfford(SimpleItem item)
    {
        return playerCashSO.CanAfford(item.CashValue);
    }

    public void BuyItem(SimpleItem item)
    {
        if(!playerCashSO.TryBuy(item.CashValue))
        {
            Debug.LogError("failed to buy item", this);
        }
    }
}
