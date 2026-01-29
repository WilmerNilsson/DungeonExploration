using UnityEditor.Graphs;
using UnityEngine;


public class InvMaster : MonoBehaviour
{
    public static InvMaster InvMasterInstance
    {
        get; private set; 
    }
    [SerializeField, Min(1)] private int collumns = 1;
    [SerializeField, Min(1)] private int rows = 1;
    [SerializeField] private RectTransform inventoryGrid;

    /// <summary>
    /// collum, row
    /// </summary>
    private SimpleItem[,] invData;

    private void Start()
    {
        InvMasterInstance = this;
        invData = new SimpleItem[collumns, rows];
    }

    //we prob want to save the resutls instead of recalculating every time.
    //But i will look into it when we start working or rezising the windows
    private Rect GetBigRect()
    {
        Rect bigRect = new();

        Vector3[] corners = new Vector3[4];
        inventoryGrid.GetWorldCorners(corners);
        //bl, tl, tr, br 
        bigRect.xMin = corners[0].x;
        bigRect.yMin = corners[0].y;
        bigRect.xMax = corners[2].x;
        bigRect.yMax = corners[2].y;

        return bigRect;
    }

    //should prob make a simpler variant that only calculates the center.
    /// <summary>
    /// gets the rect for the slot, start counting from 0
    /// </summary>
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

    public bool TryPlaceItem(SimpleItem item)
    {
        Vector2 pos = item.RectTransform.position;
        Rect bigRect = GetBigRect();

        if (!bigRect.Contains(pos)) return false;

        //big O can also decrease if we do a halving of the possible spaces
        //instead of itterating trough all possible till we get a match
        //not really needed unless the inventory is like 100000 spaces or something.
        for (int collum = 0; collum < collumns; collum++)
        {
            for (int row = 0; row < rows; row++)
            {
                Rect slot = GetSlotRect(row, collum);

                if(slot.Contains(pos))
                {
                    if (invData[collum, row] == null)
                    {
                        TryRemoveSlottedItem(item);

                        invData[collum, row] = item;
                        item.RectTransform.position = slot.center;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
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

    private bool TryRemoveSlottedItem(SimpleItem item)
    {
        for (int collum = 0; collum < invData.GetLength(0); collum++)
        {
            for(int row = 0; row < invData.GetLength(1);row++)
            {
                if (invData[collum, row] == item)
                {
                    invData[collum, row] = null;
                    return true;
                }
            }
        }
        return false;
    }

}
