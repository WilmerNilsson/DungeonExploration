using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class SimpleItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public const int GridHeight = 4;
    public const int GridWidth = 4;

    [SerializeField, Tooltip("what slot the center is, 0,0 is bottom left")]
    private Vector2Int pivot;
    [HideInInspector] public bool[] itemGridSize = new bool[GridHeight*GridWidth];
    //may just have simple bool for importing the default discard use
#if UNITY_EDITOR
    [SerializeField] private bool debugQuickConnectItemDrop = false;
#endif
    [SerializeField] private ItemUse[] uses;
    [SerializeField] private string descriptionText;
    [SerializeField] private bool descriptionTextIsLibraryName;
    [SerializeField] private TextLibrarySO textLibrary;
    [SerializeField] private string prefabID;
    [field: SerializeField] public int CashValue { get; private set; }

#nullable enable

    public string PrefabID { get { return prefabID; } }

    public UnityEvent? OnStopDrag;
    private InventoryGrid? currentInventory;

    public RectTransform RectTransform { get { return (transform as RectTransform)!; } }
    private bool isDragging;
    private Vector2 returnPos;
    private Transform? returnParent;
    public Vector2Int Pivot {get{ return pivot; } }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(debugQuickConnectItemDrop)
        {
            debugQuickConnectItemDrop = false;

            if(TryGetComponent(out ItemDrop itemDrop))
            {
                if (uses == null)
                {
                    uses = new ItemUse[1];
                    uses[0] = new();
                    uses[0].SetText("Drop");
                    uses[0].OnUse = new();
                    UnityEventTools.AddPersistentListener(uses[0].OnUse, itemDrop.Drop);
                }
                else
                {
                    ItemUse[] newUses = new ItemUse[uses.Length + 1];
                    uses.CopyTo(newUses, 0);
                    newUses[newUses.Length-1]=new();
                    newUses[newUses.Length - 1].SetText("Drop");
                    newUses[newUses.Length - 1].OnUse = new();
                    UnityEventTools.AddPersistentListener(newUses[newUses.Length - 1].OnUse, itemDrop.Drop);
                    uses = newUses;
                }
            }
            else
            {
                Debug.Log("found no item drop script");
            }
        }

        if (descriptionText == null || descriptionText == string.Empty)
        {
            Debug.LogWarning("item description text is empty", this);
        }
        else if(descriptionTextIsLibraryName && textLibrary == null)
        {
            Debug.LogWarning("item description is for library, but library reference is null", this);
        }
        else if (descriptionTextIsLibraryName && !textLibrary.TryGetTextByName(descriptionText, out _))
        {
            Debug.LogWarning("item description could not find text in library by name: " + descriptionText, this);
        }
    }
#endif

    public bool[,] GetSizeMatrix()
    {
        //small optimisazion would be to shrink the matrix if possible
        //slightly better optimisazion would be to cashe it and mark it dirty if changed
        bool[,] tempSize = new bool[GridWidth, GridHeight];

        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                tempSize[x, y] = itemGridSize[x + 4 * y];
            }
        }
        return tempSize;
    }

    private void Update()
    {
        if (isDragging)
        { 
            RectTransform.position = Input.mousePosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            returnPos = RectTransform.position;
            returnParent = transform.parent;

            InvMasterBase.Instance.ParentTransformOntop(transform);

            isDragging = true;
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(InvMasterBase.Instance is InvMaster master)
            {
                master.GetContextMenu().SelectItem(this, uses);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if(isDragging)
            {
                isDragging = false;
                OnStopDrag?.Invoke();
            }
            
            if (!InvMasterBase.Instance.TryPlaceItem(this, out InventoryGrid? newGrid))
            {
                RectTransform.position = returnPos;
                transform.SetParent(returnParent);
            }
        }
    }

    public string GetDescription()
    {
        if (descriptionTextIsLibraryName)
        {
            if (textLibrary.TryGetTextByName(descriptionText, out BookText? book))
            {
                return book.Text;
            }
            else { return descriptionText; }
        }
        else
        {
            return descriptionText;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InvMasterBase.Instance.ChangeHover(this, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InvMasterBase.Instance.ChangeHover(this, false);
    }

    internal void ReadyForDestory()
    {
        currentInventory?.TryRemoveSlottedItem(this);
        currentInventory = null;
    }

    internal void MoveTo(InventoryGrid newGrid)
    {
        if (newGrid != currentInventory)
        {
            currentInventory?.TryRemoveSlottedItem(this);
            currentInventory = newGrid;
        }
    }
}

