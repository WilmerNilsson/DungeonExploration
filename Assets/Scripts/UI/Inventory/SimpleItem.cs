using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class SimpleItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, Tooltip("what slot the center is, 0,0 is bottom left")]
    private Vector2Int piviot;
    [SerializeField] private sizes size;

    public RectTransform RectTransform { get { return (transform as RectTransform); } }
    private bool isDragging;
    Vector2 returnPos;

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

    private void Update()
    {
        GetSizeMatrix();

        if (isDragging)
        { 
            RectTransform.position = Input.mousePosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        returnPos = RectTransform.position;
        isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        if(!InvMaster.InvMasterInstance.TryPlaceItem(this))
        {
            RectTransform.position = returnPos;
        }
    }
}

