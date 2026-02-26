using UnityEngine;

public class MerchantInventory : MonoBehaviour
{
    //the reason i use 2 grids instead of just keeping track of the data is that
    //i want to avoid instansiating and destroying items unneseserarly
    [SerializeField] private InventoryGrid buyGrid;
    [SerializeField] private InventoryGrid sellGrid;
    [SerializeField] private string[] SpawnItems;
    [SerializeField] private ItemLibrarySO itemLibrary;

    private bool buyIsActiveGrid = true;
    private InventoryGrid ActiveGrid
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

#if DEBUG
    private void Start()
    {
        if (sellGrid.isActiveAndEnabled)
        {
            Debug.LogWarning("sell grid is active at start, buy should be the default", this);
        }
    }
#endif

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
    }

    public bool TryPlaceItem(SimpleItem item)
    {
        if(buyIsActiveGrid)
        {
            return false;
        }
        else if(ActiveGrid.TryPlaceItem(item))
        {
            Debug.Log("give player monney");

            //give player cash
            return true;
        }
        else return false;
    }

    public bool TryRemoveSlottedItem(SimpleItem item)
    {
        //if item costs too much return false

        return true;
    }
}
