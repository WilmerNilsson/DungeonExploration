using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class SimpleItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform RectTransform { get { return (transform as RectTransform); } }
    private bool isDragging;
    Vector2 returnPos;

    private void Update()
    {
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
