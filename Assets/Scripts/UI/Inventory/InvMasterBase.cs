using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public abstract class InvMasterBase : MonoBehaviour
{
    [field: SerializeField] public InventoryGrid PlayerInventory
    {
        get; protected set; 
    }
    [SerializeField] private Transform drawOntopParent;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [field: SerializeField, FormerlySerializedAs("equipmentGrid")] public InventoryGrid EquipmentGrid
    {
        get; private set;
    }
#nullable enable

    public static InvMasterBase Instance
    {
        get; private set;
    }

    protected virtual void Start()
    {
        Instance = this;
    }

#if DEBUG
    protected virtual void OnValidate()
    {
        if (PlayerInventory == null)
        {
            Debug.LogWarning("player inventory grid rect is null", this);
        }
        if (drawOntopParent == null) Debug.LogWarning("draw ontop parent is null", this);
        if (descriptionText == null) Debug.LogWarning("description text field is null", this);
        if (EquipmentGrid == null) Debug.LogWarning("equipment grid is null", this);

        if (!PrefabUtility.IsPartOfPrefabAsset(this) && GameObject.FindAnyObjectByType<EventSystem>() == null)
            Debug.LogWarning("no event system in scene", this);
    }
#endif

    public Vector2 GetSlotSize()
    {
        return PlayerInventory.GetSlotSize();
    }

    public virtual bool TryPlaceItem(SimpleItem item, [NotNullWhen(true)]out InventoryGrid? inventoryGrid)
    {
        if (PlayerInventory.TryPlaceItem(item))
        {
            inventoryGrid = PlayerInventory;
            return true;
        }
        else if (EquipmentGrid.TryPlaceItem(item))
        {
            inventoryGrid = EquipmentGrid;
            return true;
        }
        inventoryGrid = null;
        return false;
    }

    public virtual void ParentTransformOntop(Transform transform)
    {
        transform.SetParent(drawOntopParent);
    }

    public virtual void DestroyItem(SimpleItem item)
    {
        item.ReadyForDestory();
        Destroy(item.gameObject);
    }

    public virtual void ChangeHover(SimpleItem simpleItem, bool startHover)
    {
        if(startHover)
        {
            SetDescriptionText(simpleItem.GetDescription());
        }
        else
        {
            SetDescriptionText(string.Empty);
        }
    }

    protected void SetDescriptionText(string newText)
    {
        descriptionText.text = newText;
    }
}
