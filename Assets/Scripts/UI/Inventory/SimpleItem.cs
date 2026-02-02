
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class SimpleItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, Tooltip("what slot the center is, 0,0 is bottom left")]
    private Vector2Int piviot;
    [SerializeField] private sizes size;
    private IItemEffect[] effects;

    public RectTransform RectTransform { get { return (transform as RectTransform); } }
    private bool isDragging;
    Vector2 returnPos;
    public Vector2Int Piviot {get{ return piviot; } }

    private enum sizes
    {
        x = default, 
        xx,
        xxx,
        xxNxo,
        xxNxx
    }

    public bool[,] GetSizeMatrix()
    {
        switch (size)
        {
            case sizes.x:
            default:
                bool[,] x = { { true } };

                return x;
            case sizes.xx:
                bool[,] xx = { { true, true } };

                return xx;
            case sizes.xxx:
                bool[,] xxx = { { true, true, true } };

                return xxx;
            case sizes.xxNxo:
                bool[,] xxNxo = { { true, true },
                                  { false, true} };

                return xxNxo;
            case sizes.xxNxx:

                bool[,] xxNxx = { { true, true },
                                  { true, true } };

                return xxNxx;
            
        }
    }

    private void Start()
    {
        effects = GetComponents<IItemEffect>();
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
            isDragging = true;
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            InvMaster.Instance.GetContextMenu().SelectItem(this, effects);
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
            }
        }
    }
}

