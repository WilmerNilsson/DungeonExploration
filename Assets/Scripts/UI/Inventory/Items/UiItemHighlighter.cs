using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiItemHighlighter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Graphic targetGraphic;
    [SerializeField] private Color highlightCollor = Color.cyan;


    private bool _colorFetched;
    private Color _defaultCollor;
    private Color defaultCollor
    {
        get
        {
            if(_colorFetched == false)
            {
                _defaultCollor = targetGraphic.color;
                _colorFetched = true;
            }

            return _defaultCollor;
        }
    }

#if UNITY_EDITOR
    [SerializeField] private bool supressFind = false;

    private void OnValidate()
    {
        if (targetGraphic == null && !supressFind)
        {
            Debug.Log("target graphic is null, trying to find one", this);

            if (TryGetComponent(out Graphic graphic))
            {
                targetGraphic = graphic;
            }
            else
            {
                supressFind = true;
                Debug.LogWarning("found no graphic on object", this);
            }
        }
        else if (targetGraphic == null) Debug.LogWarning("target graphic is null", this);
    }
#endif

    private void OnDisable()
    {
        targetGraphic.color = defaultCollor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetGraphic.color = highlightCollor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetGraphic.color = defaultCollor;
    }
}
