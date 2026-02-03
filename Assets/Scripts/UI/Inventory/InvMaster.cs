using Ink.Parsed;
using System.Collections.Generic;
using Unity.Plastic.Antlr3.Runtime;
using UnityEditor.Graphs;
using UnityEngine;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;


public class InvMaster : MonoBehaviour
{
    public static InvMaster Instance
    {
        get; private set; 
    }
    [SerializeField] private InventoryGrid playerInventoryGrid;
    [SerializeField] private ItemContextMenu contextMenu;
    [SerializeField] private InventoryGrid testGrid;

    /// <summary>
    /// collum, row
    /// </summary>

    //dunno what we expect to be the max open container amount, but going with 1 for now
    private List<InventoryGrid> openContainers = new(1);

    private void Start()
    {
        Instance = this;
        openContainers.Add(testGrid);
    }

#if DEBUG
    private void OnValidate()
    {
        if(playerInventoryGrid == null)
        {
            Debug.LogWarning("inventory grid rect is null", this);
        }
        if (contextMenu == null) Debug.LogWarning("context menu is null", this);

    }
#endif

    public ItemContextMenu GetContextMenu()
        { return contextMenu; }

    public Vector2 GetSlotSize()
    {
        return playerInventoryGrid.GetSlotSize();
    }

    public bool TryPlaceItem(SimpleItem item)
    {
        if(playerInventoryGrid.TryPlaceItem(item))
        {
            return true;
        }
        else
        {
            foreach(InventoryGrid grid in openContainers)
            {
                Debug.Log("is grid null: " + grid == null);
                Debug.Log(grid.name);
                if(grid.TryPlaceItem(item)) return true;
            }
        }

        return false;
    }
}
