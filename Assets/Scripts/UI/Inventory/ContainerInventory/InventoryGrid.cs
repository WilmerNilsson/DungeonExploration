using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class InventoryGrid : MonoBehaviour
{
    [SerializeField, Min(1)] private int collumns = 1;
    [SerializeField, Min(1)] private int rows = 1;

    /// <summary>
    /// collum, row
    /// </summary>
    private SimpleItem[,] invData;

#if DEBUG
    private void OnValidate()
    {
        Vector2 slotSize = GetSlotSize();

        //note that this is without a check for float rounding.
        //if it becomes a problem then we can change, but inventory should not have insande tranform values
        if (slotSize.x != slotSize.y) 
            Debug.LogWarning($"inventory slot width and height does not match. width is {slotSize.x}, height is {slotSize.y}", this);
    }
#endif

    void Start()
    {
        invData = new SimpleItem[collumns, rows];
    }

    private Rect GetBigRect()
    {
        Rect bigRect = new();

        Vector3[] corners = new Vector3[4];
        Debug.Log("a");

        (transform as RectTransform).GetWorldCorners(corners);

        Debug.Log("b");
        //bl, tl, tr, br 
        bigRect.xMin = corners[0].x;
        bigRect.yMin = corners[0].y;
        bigRect.xMax = corners[2].x;
        bigRect.yMax = corners[2].y;

        return bigRect;
    }

    private Rect GetSlotRect(int collum, int row)
    {
        Rect bigRect = GetBigRect();

        Rect slot = new();

        slot.xMin = bigRect.xMin + collum * (bigRect.width / collumns);
        slot.xMax = bigRect.xMin + (collum + 1) * (bigRect.width / collumns);
        slot.yMin = bigRect.yMin + row * (bigRect.height / rows);
        slot.yMax = bigRect.yMin + (row + 1) * (bigRect.height / rows);

        return slot;
    }

    public Vector2 GetSlotSize()
    {
        Rect bigRect = GetBigRect();

        return new(bigRect.width / collumns, bigRect.height / rows);
    }

    public bool TryPlaceItem(SimpleItem item)
    {
        Vector2 pos = item.RectTransform.position;

        Debug.Log(1);

        if(!GetBigRect().Contains(pos)) return false;

        Debug.Log(2);
        bool[,] itemSlots = item.GetSizeMatrix();

        Debug.Log(3);
#if DEBUG
        for (int x = 0; x < itemSlots.GetLength(0); x++)
        {
            for (int y = 0; y < itemSlots.GetLength(1); y++)
            {
                //Debug.Log($"item takes up: {x},{y} - {itemSlots[x,y]}");
            }
        }
#endif
        Debug.Log(4);

        //TODO work in piviot
        if (TryGetSlotOfPos(pos, out int collum, out int row, out Rect slot))
        {
            for (int x = 0; x < itemSlots.GetLength(0); x++)
            {
                for (int y = 0; y < itemSlots.GetLength(1); y++)
                {
                    if ((InvSlotExists(collum + x - item.Piviot.x, row + y - item.Piviot.y) &&
                        itemSlots[x, y] == true &&
                        (invData[collum + x - item.Piviot.x, row + y - item.Piviot.y] == null ||
                        invData[collum + x - item.Piviot.x, row + y - item.Piviot.y] == item)

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

            RemoveSlottedItem(item);

            for (int x = 0; x < itemSlots.GetLength(0); x++)
            {
                for (int y = 0; y < itemSlots.GetLength(1); y++)
                {
                    if (itemSlots[x, y] == true)
                    {
                        invData[collum + x - item.Piviot.x, row + y - item.Piviot.y] = item;
                    }
                }
            }
            item.RectTransform.position = slot.center;
            return true;
        }

        return false;
    }

    private bool InvSlotExists(int collumn, int row)
    {
        if (collumn < 0 || row < 0 || collumn >= invData.GetLength(0) || row >= invData.GetLength(1))
        { return false; }
        return true;
    }

    private bool TryGetSlotOfPos(Vector2 pos, out int collum, out int row, out Rect slot)
    {
        //slot should be nullable, but i could not get the nullable forgiving to work for some reason.

        //big O can also decrease if we do a halving of the possible spaces
        //instead of itterating trough all possible till we get a match
        //not really needed unless the inventory is like 100000 spaces or something.

        if (!GetBigRect().Contains(pos)) goto fail;

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

    public bool TryInsertItem(SimpleItem item)
    {
        for (int collum = 0; collum < invData.GetLength(0); collum++)
        {
            for (int row = 0; row < invData.GetLength(1); row++)
            {
                Debug.Log("trying c,r" + collum + ", " + row);

                if (invData[collum, row] == null)
                {
                    Rect slot = GetSlotRect(collum, row);

                    item.RectTransform.position = slot.center;

                    invData[collum, row] = item;
                    return true;
                }
            }
        }
        return false;
    }

    private void RemoveSlottedItem(SimpleItem item)
    {
        //we have to itterate trough all cause items can be bigger
        //alternativly we can count the size and stopp itterating once we have [size] amount of hits
        for (int collum = 0; collum < invData.GetLength(0); collum++)
        {
            for (int row = 0; row < invData.GetLength(1); row++)
            {
                if (invData[collum, row] == item)
                {
                    invData[collum, row] = null;
                }
            }
        }
    }

}
