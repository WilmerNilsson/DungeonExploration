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
    [SerializeField] private Transform drawOntopParent;
    [Header("reading")] //Starting to feel like we may want to split this up further, but prob once we do a pause menu
    [SerializeField] private OpenBookController openBookController;
    [SerializeField] private TextMeshProUGUI descriptionText;
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
        if (drawOntopParent == null) Debug.LogWarning("draw ontop parent is null", this);
        if (openBookController == null) Debug.LogWarning("open book controller is null", this);
        if (descriptionText == null) Debug.LogWarning("description text field is null", this);

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

        container.Grid.transform.SetParent(worldContainerParent, false);
    }

    public void RemoveWorldContainerFromSystem(ContainerController container)
    {
        if(openContainers.Remove(container))
        {
            container.Close();
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

        openContainers.Clear();
    }

    public void ParentTransformOntop(Transform transform)
    {
        transform.SetParent(drawOntopParent);

        contextMenu.Deselect();
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
        contextMenu.Deselect();
        openBookController.OpenText(newText);
    }

    public void CloseText()
    {
        openBookController.CloseText();
    }

    public void SetDescriptionText(string newText)
    {
        descriptionText.text = newText;
    }
}
