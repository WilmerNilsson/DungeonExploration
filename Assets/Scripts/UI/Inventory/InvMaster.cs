using Ink.Parsed;
using System.Collections.Generic;
using Unity.Plastic.Antlr3.Runtime;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.EventSystems;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;


public class InvMaster : MonoBehaviour
{
    public static InvMaster Instance
    {
        get; private set; 
    }
    [SerializeField] private InventoryGrid playerInventoryGrid;
    [SerializeField] private ItemContextMenu contextMenu;
    [SerializeField] private Transform worldContainerParent;
    [SerializeField] private GameObject playerInventory;
    [SerializeField] private PlayerController playerController;

    /// <summary>
    /// collum, row
    /// </summary>

    //dunno what we expect to be the max open container amount, but going with 1 for now
    private List<ContainerController> openContainers = new(1);

    private void Start()
    {
        Instance = this;
    }

#if DEBUG
    private void OnValidate()
    {
        if(playerInventoryGrid == null)
        {
            Debug.LogWarning("inventory grid rect is null", this);
        }
        if (contextMenu == null) Debug.LogWarning("context menu is null", this);
        if (worldContainerParent == null) Debug.LogWarning("world container parent is null", this);
        if (playerInventory == null) Debug.LogWarning("player inventory object is null", this);
        //if (playerController == null) Debug.LogWarning("no connection to a player controller", this) ;

        if (GameObject.FindAnyObjectByType<EventSystem>() == null) Debug.LogWarning("no event system in scene", this);
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
            foreach(ContainerController container in openContainers)
            {
                if(container.Grid.TryPlaceItem(item)) return true;
            }
        }

        return false;
    }

    public void AddOpenWorldContainerToSystem(ContainerController container)
    {
        if (openContainers.Contains(container)) return;

        openContainers.Add(container);

        OpenPlayerInventory();

#if DEBUG
        if(openContainers.Count > 1)
        {
            Debug.LogWarning("2 or more world containers are open, but support is currently just for 1", this);
        }
#endif

        container.Grid.transform.SetParent(worldContainerParent);
        container.Grid.transform.localPosition = Vector3.zero;
    }

    public void RemoveWorldContainerFromSystem(ContainerController container)
    {
        openContainers.Remove(container);
    }

    public void ToggleInventory()
    {
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
        playerInventory.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined; //we need to controll curson with a pause menu once implimented
        playerController.LockMovement(true);
    }

    public void ClosePlayerInventory()
    {
        playerInventory.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerController.LockMovement(false);

        foreach (ContainerController container in openContainers)
        {
            container.Close();
        }

        openContainers.Clear();
    }

}
