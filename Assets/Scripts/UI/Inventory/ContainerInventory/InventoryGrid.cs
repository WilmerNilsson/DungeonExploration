using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class InventoryGrid : MonoBehaviour
{
    /// <summary>
    /// the standard grid size in pixels at 1920x1080
    /// </summary>
    public const int StandardGridSize = 100;

    [SerializeField, Min(1)] private int collumns = 1;
    [SerializeField, Min(1)] private int rows = 1;
#nullable enable

    private bool hasBeenEnabled = false;

    public UnityEvent<SimpleItem>? OnGetNewItem;

    /// <summary>
    /// collum, row
    /// </summary>
    private ItemWithPiviot?[,] InvData
    {
        get
        {
            if (_invData == null)
            {
                _invData = new ItemWithPiviot[collumns, rows];
            }
            return _invData;
        }
    }
    private ItemWithPiviot[,]? _invData;

    private void OnEnable()
    {
        if(hasBeenEnabled) return;
        hasBeenEnabled = true;

        for(int x = 0; x < InvData.GetLength(0); x++)
        {
            for (int y = 0; y < InvData.GetLength(1); y++)
            {
                if (InvData[x,y] != null && InvData[x,y]!.IsPiviot)
                {
                    InvData[x, y]!.Item.RectTransform.position = GetSlotRect(x, y).center;
                }
            }
        }
    }

#if UNITY_EDITOR
    [Header("gizmos")]
    [SerializeField] private bool drawCenter;
    [SerializeField] private bool drawGrid;
    [SerializeField] private bool drawOccupied;

    private void OnDrawGizmos()
    {
        if (drawOccupied)
        {
            for (int collum = 0; collum < collumns; collum++)
            {
                for (int row = 0; row < rows; row++)
                {
                    if (InvData[collum, row] == null) continue;

                    if (InvData[collum, row]!.IsPiviot)
                    {
                        Gizmos.color = Color.yellow;
                    }
                    else
                    {
                        Gizmos.color = Color.blue;
                    }

                    Rect slot = GetSlotRect(collum, row);

                    Gizmos.DrawWireSphere(slot.center, slot.height / 3f);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(drawGrid)
        {
            Gizmos.color = Color.white;
            for (int collum = 0; collum < collumns; collum++)
            {
                for (int row = 0; row < rows; row++)
                {
                    Rect slot = GetSlotRect(collum, row);

                    Gizmos.DrawWireSphere(slot.center, slot.height / 2f);
                }
            }
        }

        if (drawCenter)
        {
            Rect globalRect = GlobalRect();

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(globalRect.center, globalRect.width / 2f);
            Gizmos.DrawWireSphere(globalRect.center, globalRect.width / 4f);
        }
    }
#endif

#if DEBUG
    private void OnValidate()
    {
        CheckSlotSize();

        if ((transform as RectTransform)!.pivot.y != 0.5f || (transform as RectTransform)!.pivot.x != 0.5f)
            Debug.LogWarning("inventory grid pivot is not 0.5 in x and y", this);

        void CheckSlotSize()
        {
            Vector2 slotSize = SlotSizeWOScaler();

            float delta = 0.0005f;
            if (Mathf.Abs(slotSize.x - slotSize.y) > delta)
            {
                Debug.LogWarning($"inventory slot width and height does not match. width is {slotSize.x}, height is {slotSize.y}, name is {gameObject.name}", this);
            }
            if (Mathf.Abs((float)StandardGridSize - slotSize.y) > delta)
            {
                Debug.LogWarning($"slot size of Y is not the standard: {StandardGridSize}, it is {slotSize.y}", this);
            }
            if (Mathf.Abs((float)StandardGridSize - slotSize.x) > delta)
            {
                Debug.LogWarning($"slot size of X is not the standard: {StandardGridSize}, it is {slotSize.x}", this);
            }
        }

        Vector2 SlotSizeWOScaler()
        {
            Rect bigRect = (transform as RectTransform)!.rect;

            float scaleX = 1f;
            float scaleY = 1f;

            int sanity = 100;

            for (Transform t = transform; t.parent != null; t = t.parent)
            {
                if (t.TryGetComponent<CanvasScaler>(out CanvasScaler scaler))
                {
                    break;
                }
                else
                {
                    scaleX *= t.localScale.x;
                    scaleY *= t.localScale.y;
                }

                sanity--;
                if (sanity <= 0)
                {
                    Debug.Log("hit sanity cap", this);
                    break;
                }
            }
            bigRect.width = bigRect.width * scaleX;
            bigRect.width /= collumns;
            bigRect.height = bigRect.height * scaleY;
            bigRect.height /= rows;

            return bigRect.size;
        }
    }
#endif

    #region Geometry
    private Rect GlobalRect()
    {
        //This will get global rect that is neutral to rotation
        //alternative is to go with the corners, and then counter rotate them
        //i do not think one or the other is much more preformance heavy

        RectTransform rt = (transform as RectTransform)!;

        Rect bigRect = rt.rect;

#if UNITY_EDITOR
        float scaleX = 1f;
        float scaleY = 1f;

        int sanity = 100;

        for (Transform t = transform; t.parent != null; t = t.parent)
        {
            if(t.TryGetComponent<CanvasScaler>(out CanvasScaler scaler))
            {
                scaleX *= scaler.scaleFactor;
                scaleY *= scaler.scaleFactor;
            }
            else
            {
                scaleX *= t.localScale.x;
                scaleY *= t.localScale.y;
            }

            sanity--;
            if (sanity <= 0)
            {
                Debug.Log("hit sanity cap", this);
                break;
            }
        }
        bigRect.width = bigRect.width * scaleX;
        bigRect.height = bigRect.height * scaleY;
#else
        bigRect.width = bigRect.width * rt.lossyScale.x;
        bigRect.height = bigRect.height * rt.lossyScale.y;
#endif

        bigRect.center = rt.position;

        return bigRect;
    }

    private Rect GetSlotRect(int collum, int row)
    {
        Rect bigRect = GlobalRect();

        Rect slot = new();

        slot.xMin = bigRect.xMin + collum * (bigRect.width / collumns);
        slot.xMax = bigRect.xMin + (collum + 1) * (bigRect.width / collumns);
        slot.yMin = bigRect.yMin + row * (bigRect.height / rows);
        slot.yMax = bigRect.yMin + (row + 1) * (bigRect.height / rows);

        return slot;
    }

    public Vector2 GetSlotSize()
    {
        Rect bigRect = GlobalRect();

        return new(bigRect.width / collumns, bigRect.height / rows);
    }

    private bool TryGetSlotOfPos(Vector2 pos, out int collum, out int row, out Rect slot)
    {
        //slot should be nullable, but i could not get the nullable forgiving to work for some reason.

        //big O can also decrease if we do a halving of the possible spaces
        //instead of itterating trough all possible till we get a match
        //not really needed unless the inventory is like 100000 spaces or something.

        if (!GlobalRect().Contains(pos))
        {
            goto fail;
        }

        for (collum = 0; collum < collumns; collum++)
        {
            for (row = 0; row < rows; row++)
            {
                slot = GetSlotRect(collum, row);

                if (slot.Contains(pos))
                {
                    return true;
                }
            }
        }

    fail:
        collum = 0;
        row = 0;
        slot = new();
        return false;
    }

#endregion

    public bool TryInstantiateItemInSlot(int slot, GameObject prefab)
    {
#if DEBUG
        if (prefab.TryGetComponent<SimpleItem>(out SimpleItem component))
        {
            int collum = slot % (collumns);
            int row = (slot - collum) / (collumns);

            //4567890
            //7890123
            //0123456

            return TryPutItemInSlot(component, collum, row, true);
        }
        else
        {
            Debug.LogError($"can't instanciate prefab {prefab}, cause it is not a simple item", this);
            return false;
        }
#else //assume it won't error;
        int row = slot % collumns;
        int collum = slot - (row * collumns);

        return TryPutItemInSlot(prefab.GetComponent<SimpleItem>(), row, collum, true);
#endif
    }

    public List<InventorySaveData.InventoryItem> GetInventoryData()
    {
        List<InventorySaveData.InventoryItem> data = new();

        for (int collum = 0; collum < InvData.GetLength(0); collum++)
        {
            for (int row = 0; row < InvData.GetLength(1); row++)
            {
                if (InvData[collum, row] == null) continue;
                if (InvData[collum, row]!.IsPiviot == false) continue;

                InventorySaveData.InventoryItem item = new();
                item.PrefabID = InvData[collum, row]!.Item.PrefabID;

                //4567
                //0123
                item.Slot = (row * collumns) + collum;

                data.Add(item);
            }
        }

        return data;
    }

    public bool TryPlaceItem(SimpleItem item)
    {
        Vector2 pos = item.RectTransform.position;

        if(!GlobalRect().Contains(pos))
        {
            return false;
        }

        bool[,] itemSlots = item.GetSizeMatrix();

#if DEBUG
        for (int x = 0; x < itemSlots.GetLength(0); x++)
        {
            for (int y = 0; y < itemSlots.GetLength(1); y++)
            {
                //Debug.Log($"item takes up: {x},{y} - {itemSlots[x,y]}");
            }
        }
#endif

        //TODO work in pivot
        if (TryGetSlotOfPos(pos, out int collum, out int row, out Rect slot))
        {
            return TryPutItemInSlot(item, collum, row);
        }

        return false;
    }

    private bool InvSlotExists(int collumn, int row)
    {
        if (collumn < 0 || row < 0 || collumn >= InvData.GetLength(0) || row >= InvData.GetLength(1))
        {
            return false;
        }
        return true;
    }

    public bool TryInsertItem(SimpleItem item, bool instantiate = false)
    {
        for (int collum = 0; collum < InvData.GetLength(0); collum++)
        {
            for (int row = 0; row < InvData.GetLength(1); row++)
            {
                if(TryPutItemInSlot(item, collum, row, instantiate)) return true;
            }
        }
        return false;
    }

    private bool TryPutItemInSlot(SimpleItem item, int collum, int row, bool instantiate = false)
    {
        bool[,] itemSlots = item.GetSizeMatrix();

        for (int x = 0; x < itemSlots.GetLength(0); x++)
        {
            for (int y = 0; y < itemSlots.GetLength(1); y++)
            {
                bool itemSlotActive = itemSlots[x, y] == true;
                if (!itemSlotActive) continue; // we can skip if not active

                bool invSlotExists = InvSlotExists(collum + x - item.Pivot.x, row + y - item.Pivot.y);
                if(! invSlotExists) return false;

                bool spaceIsFreeIfItemIsAbsent = InvData[collum + x - item.Pivot.x, row + y - item.Pivot.y] == null ||
                    InvData[collum + x - item.Pivot.x, row + y - item.Pivot.y]!.Item == item;

                if (!spaceIsFreeIfItemIsAbsent) return false;
            }
        }
        //by this point it is clear that we can place the item

        if ( instantiate )
        {
            item = Instantiate(item.gameObject).GetComponent<SimpleItem>();
        }
        else
        {
            TryRemoveSlottedItem(item);
        }

        for (int x = 0; x < itemSlots.GetLength(0); x++)
        {
            for (int y = 0; y < itemSlots.GetLength(1); y++)
            {
                if (itemSlots[x, y] == true)
                {
                    InvData[collum + x - item.Pivot.x, row + y - item.Pivot.y] = new(item, false);
                }
            }
        }

        InvData[collum, row]!.IsPiviot = true;

        item.MoveTo(this);
        item.RectTransform.SetParent(transform, false);
        item.RectTransform.position = GetSlotRect(collum, row).center;
        return true;
    }

    public bool HasItem(SimpleItem item)
    {
        for (int collum = 0; collum < InvData.GetLength(0); collum++)
        {
            for (int row = 0; row < InvData.GetLength(1); row++)
            {
                if (InvData[collum, row] == null) { continue; }
                else if (InvData[collum, row]!.Item == item)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// generally called from simple item, but also when moving from and to same grid
    /// </summary>
    public bool TryRemoveSlottedItem(SimpleItem item)
    {
        //we have to itterate trough all cause items can be bigger
        //alternativly we can count the size and stopp itterating once we have [size] amount of hits
        bool foundMatch = false;

        for (int collum = 0; collum < InvData.GetLength(0); collum++)
        {
            for (int row = 0; row < InvData.GetLength(1); row++)
            {
                if(InvData[collum, row] == null) { continue; }
                else if (InvData[collum, row]!.Item == item)
                {
                    InvData[collum, row] = null;
                    foundMatch = true;
                }
            }
        }

        return foundMatch;
    }

    private class ItemWithPiviot
    {
        public SimpleItem Item;
        public bool IsPiviot;

        public ItemWithPiviot(SimpleItem item, bool isPiviot)
        {
            Item = item;
            IsPiviot = isPiviot;
        }
    }
}
