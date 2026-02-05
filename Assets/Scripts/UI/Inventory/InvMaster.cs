using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;


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
    [SerializeField] private Transform drawOntopParent;
    [Header("reading")] //Starting to feel like we may want to split this up further, but prob once we do a pause menu
    [SerializeField] private GameObject readingCanvasParent;
    [SerializeField] private TextMeshProUGUI readingText;
    /// <summary>
    /// collum, row
    /// </summary>

    //dunno what we expect to be the max open container amount, but going with 1 for now
    private List<ContainerController> openContainers = new(1);

    public InventoryGrid PlayerInventory { get { return playerInventoryGrid; } }

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

        if(!PrefabUtility.IsPartOfPrefabAsset(this) && playerController == null)
        {
            //for some reason this activated constantly
            //Debug.LogWarning("no connection to a player controller", this);
        }

        if (drawOntopParent == null) Debug.LogWarning("draw ontop parent is null", this);

        if (!PrefabUtility.IsPartOfPrefabAsset(this) && GameObject.FindAnyObjectByType<EventSystem>() == null)
            Debug.LogWarning("no event system in scene", this);
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
        //perhaps have a reference in SimpleItem to current inventory that we can remove it from
        //when we move it from one inventory to another.

        if(playerInventoryGrid.TryPlaceItem(item))
        {
            foreach (ContainerController container in openContainers)
            {
                if(container.Grid.TryRemoveSlottedItem(item))
                {
                    break;
                }
            }

            return true;
        }
        else
        {
            for (int i = 0; i < openContainers.Count; i++)
            {
                ContainerController container = openContainers[i];
                if (container.Grid.TryPlaceItem(item))
                {
                    if(playerInventoryGrid.TryRemoveSlottedItem(item))
                    {
                        return true;
                    }

                    //we did not move the item from players inventory,
                    //so we need to check if there is another inventory to remove the item from
                    for (int i2 = 0; i2 < openContainers.Count; i2++)
                    {
                        if(i == i2) continue;

                        if (openContainers[i2].Grid.TryRemoveSlottedItem(item)) break;
                    }

                    return true;
                }
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

    public void ParentTransformOntop(Transform transform)
    {
        transform.SetParent(drawOntopParent);
    }

    public void DestroyItem(SimpleItem item)
    {
        if (playerInventoryGrid.TryRemoveSlottedItem(item))
        {
            
        }
        else
        {
            foreach (ContainerController container in openContainers)
            {
                if (container.Grid.TryRemoveSlottedItem(item))
                {
                    break;
                }
            }
        }

        contextMenu.TryDeselectItem(item);
        Destroy(item.gameObject);
    }

    public void OpenText(string newText)
    {
        readingCanvasParent.SetActive(true);
        readingText.text = newText;
    }

    public void CloseText()
    {
        readingCanvasParent.SetActive(false);
    }
}
