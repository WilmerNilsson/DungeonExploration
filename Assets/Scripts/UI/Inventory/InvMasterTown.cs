using UnityEngine;

public class InvMasterTown : InvMasterBase
{
    [SerializeField] private InventoryGrid merchantGrid;

    protected override void Start()
    {
        base.Start();
    }

#if DEBUG
    protected override void OnValidate()
    {
        base.OnValidate();
        if (merchantGrid == null) Debug.LogWarning("merchant grid is null", this);
    }
#endif

    public override bool TryPlaceItem(SimpleItem item)
    {
        //perhaps have a reference in SimpleItem to current inventory that we can remove it from
        //when we move it from one inventory to another.


        // we may want to send a predicate to try and take the cash
        if (PlayerInventoryGrid.TryPlaceItem(item))
        {
            merchantGrid.TryRemoveSlottedItem(item);
            return true;
        }
        else if(merchantGrid.TryPlaceItem(item))
        {
            PlayerInventoryGrid.TryRemoveSlottedItem(item);
            return true;
        }
        else
        {
            return false;
        }
    }
}
