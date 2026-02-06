
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class SimpleItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, Tooltip("what slot the center is, 0,0 is bottom left")]
    private Vector2Int piviot;
    [HideInInspector] public bool[] itemGridSize = new bool[16];
    //may just have simple bool for importing the default discard use
    [SerializeField] private ItemUse[] uses;

    public RectTransform RectTransform { get { return (transform as RectTransform); } }
    private bool isDragging;
    Vector2 returnPos;
    Transform returnParent;
    public Vector2Int Piviot {get{ return piviot; } }

    public bool[,] GetSizeMatrix()
    {
        bool[,] tempSize = new bool[4, 4];

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

            InvMaster.Instance.ParentTransformOntop(transform);

            isDragging = true;
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            InvMaster.Instance.GetContextMenu().SelectItem(this, uses);
        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = false;
            if (!InvMaster.Instance.TryPlaceItem(this))
            {
                RectTransform.position = returnPos;
                transform.SetParent(returnParent);
            }
        }
    }
}

