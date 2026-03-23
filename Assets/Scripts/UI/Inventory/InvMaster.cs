using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;


public class InvMaster : InvMasterBase
{
    [SerializeField] private ItemContextMenu contextMenu;
    [SerializeField] private Transform worldContainerParent;
    [SerializeField] private GameObject playerInventory;
    [Header("reading")] //Starting to feel like we may want to split this up further, but prob once we do a pause menu
    [SerializeField] private OpenBookController openBookController;
    /// <summary>
    /// collum, row
    /// </summary>

#nullable enable

    //dunno what we expect to be the max open container amount, but going with 1 for now
    private List<ContainerController> openContainers = new(1);

    protected override void Start()
    {
        base.Start();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (contextMenu == null) Debug.LogWarning("context menu is null", this);
        if (worldContainerParent == null) Debug.LogWarning("world container parent is null", this);
        if (playerInventory == null) Debug.LogWarning("player inventory object is null", this);
        if (openBookController == null) Debug.LogWarning("open book controller is null", this);
    }
#endif

    public ItemContextMenu GetContextMenu()
        { return contextMenu; }

    public override bool TryPlaceItem(SimpleItem item, [NotNullWhen(true)] out InventoryGrid? inventoryGrid)
    {
        //perhaps have a reference in SimpleItem to current inventory that we can remove it from
        //when we move it from one inventory to another.

        if(base.TryPlaceItem(item, out inventoryGrid))
        {
            return true;
        }
        else
        {
            for (int i = 0; i < openContainers.Count; i++)
            {
                ContainerController container = openContainers[i];
                if (container.Grid.TryPlaceItem(item))
                {
                    inventoryGrid = container.Grid;

                    return true;
                }
            }
        }

        inventoryGrid = null;
        return false;
    }

    public void AddOpenWorldContainerToSystem(ContainerController container)
    {
        if (openContainers.Contains(container)) return;

        openContainers.Add(container);

        OpenPlayerInventory();

        container.Grid.transform.SetParent(worldContainerParent, false);

        if(quickLootInventory == null)
        {
            quickLootInventory = container.Grid;
        }
    }

    public void RemoveWorldContainerFromSystem(ContainerController container)
    {
        if(openContainers.Remove(container))
        {
            container.Close();

            if(quickLootInventory == container.Grid)
            {
                quickLootInventory = null;
            }
        }
    }

    public void ToggleInventory()
    {
        if (GameManagerSO.Instance.IsGameFrozen) return;
        if(playerInventory.activeSelf)
        {
            ClosePlayerInventory();
        }
        else
        {
            OpenPlayerInventory();
        }
    }

    public void OpenPlayerInventory()
    {
        if (playerInventory.activeSelf) return;
        
        if (MinimapMaster.Instance)
        {
            MinimapMaster.Instance.CloseMinimap();
        }

        contextMenu.Deselect();
        playerInventory.SetActive(true);

        GameManagerSO.Instance.LockMouse(true);
    }

    public void ClosePlayerInventory()
    {
        if (!playerInventory.activeSelf) return;

        contextMenu.Deselect();
        CloseText();

        playerInventory.SetActive(false);
        GameManagerSO.Instance.LockMouse(false);

        foreach (ContainerController container in openContainers)
        {
            container.Close();
        }

        quickLootInventory = null;
        openContainers.Clear();
    }

    public override void ParentTransformOntop(Transform transform)
    {
        base.ParentTransformOntop(transform);

        contextMenu.Deselect();
    }

    public override void DestroyItem(SimpleItem item)
    {
        base.DestroyItem(item);

        contextMenu.TryDeselectItem(item);
    }

    public void OpenText(string newText)
    {
        contextMenu.Deselect();
        openBookController.OpenText(newText);
    }

    public void CloseText()
    {
        openBookController.CloseText();
    }

    public bool GetIsActive()
    {
        return playerInventory.activeSelf;
    }
}
