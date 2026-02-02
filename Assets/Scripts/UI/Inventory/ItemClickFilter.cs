using Codice.CM.SEIDInfo;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform), typeof(SimpleItem))]
public class ItemClickFilter : MonoBehaviour, ICanvasRaycastFilter
{
    //assumes that item sprite is sized correctly and that bottom left is bottom left
    //system does not use piviot so should work if we move away from that

    private SimpleItem item;


    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        bool[,] invSlotMatrix = item.GetSizeMatrix();

        Vector3[] corners = new Vector3[4];
        (transform as RectTransform).GetWorldCorners(corners);

        Vector2 bl = corners[0];

        Vector2 slotSize = InvMaster.Instance.GetSlotSize();

        for (int x = 0; x < invSlotMatrix.GetLength(0); x++)
        {
            for (int y = 0; y < invSlotMatrix.GetLength(1); y++)
            {
                Vector2 offset = new(x*slotSize.x, y*slotSize.y);

                Rect workingRect = new Rect(bl + offset, slotSize);

                if(workingRect.Contains(sp))
                {
                    return invSlotMatrix[x, y];
                }
            }
        }

        return false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        item = GetComponent<SimpleItem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
