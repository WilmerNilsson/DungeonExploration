using UnityEditor.Graphs;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class InventoryGrid : MonoBehaviour
{
    [SerializeField, Min(1)] private int collumns = 1;
    [SerializeField, Min(1)] private int rows = 1;

    /// <summary>
    /// collum, row
    /// </summary>
    private SimpleItem[,] InvData
    {
        get
        {
            if (_invData == null)
            {
                _invData = new SimpleItem[collumns, rows];
            }
            return _invData;
        }
    }
    private SimpleItem[,] _invData;

#if DEBUG
    private void OnValidate()
    {
        Vector2 slotSize = GetSlotSize();

        float delta = 0.000001f;
        if (Mathf.Abs(slotSize.x - slotSize.y) > delta)
        {
            Debug.Log(slotSize.x - slotSize.y);
            Debug.Log(Mathf.Abs(slotSize.x - slotSize.y));
            Debug.LogWarning($"inventory slot width and height does not match. width is {slotSize.x}, height is {slotSize.y}", this);
        }

        if ((transform as RectTransform).pivot.y != 0.5f || (transform as RectTransform).pivot.x != 0.5f)
            Debug.LogWarning("inventory grid piviot is not 0.5 in x and y", this);
        
    }
#endif

    private Rect GlobalRect()
    {
        //This will get global rect that is neutral to rotation
        //alternative is to go with the corners, and then counter rotate them
        //i do not think one or the other is much more preformance heavy

        RectTransform rt = (transform as RectTransform);

        Rect bigRect = rt.rect;

        bigRect.center = rt.position;

        bigRect.width = bigRect.width * rt.lossyScale.x;
        bigRect.height = bigRect.height * rt.lossyScale.y;

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

        //TODO work in piviot
        if (TryGetSlotOfPos(pos, out int collum, out int row, out Rect slot))
        {
            return TryPutItemInSlot(item, collum, row);
        }

        return false;
    }

    private bool InvSlotExists(int collumn, int row)
    {
        if (collumn < 0 || row < 0 || collumn >= InvData.GetLength(0) || row >= InvData.GetLength(1))
        { return false; }
        return true;
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
                if ((InvSlotExists(collum + x - item.Piviot.x, row + y - item.Piviot.y) &&
                    itemSlots[x, y] == true &&
                    (InvData[collum + x - item.Piviot.x, row + y - item.Piviot.y] == null ||
                    InvData[collum + x - item.Piviot.x, row + y - item.Piviot.y] == item)
                    ) ||
                    itemSlots[x, y] == false)
                {
                    //continue;
                }
                else
                {
                    return false;
                }
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
                    InvData[collum + x - item.Piviot.x, row + y - item.Piviot.y] = item;
                }
            }
        }
        item.RectTransform.SetParent(transform);
        item.RectTransform.position = GetSlotRect(collum, row).center;
        return true;
    }

    public bool TryRemoveSlottedItem(SimpleItem item)
    {
        //we have to itterate trough all cause items can be bigger
        //alternativly we can count the size and stopp itterating once we have [size] amount of hits
        bool foundMatch = false;

        for (int collum = 0; collum < InvData.GetLength(0); collum++)
        {
            for (int row = 0; row < InvData.GetLength(1); row++)
            {
                if (InvData[collum, row] == item)
                {
                    InvData[collum, row] = null;
                    foundMatch = true;
                }
            }
        }

        return foundMatch;
    }

}
