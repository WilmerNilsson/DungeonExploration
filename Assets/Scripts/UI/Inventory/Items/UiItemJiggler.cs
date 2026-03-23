using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class UiItemJiggler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private string animationNameClick = "UIItemJiggleAnimation";
    [SerializeField] private string animationNameRelease = "UIItemJiggleAnimation";
    [SerializeField] private SimpleItem mySimpleItem;

#if UNITY_EDITOR
    [SerializeField] private bool blockAutoConnect = false;

    private void OnValidate()
    {
        if(mySimpleItem == null && !blockAutoConnect)
        {
            Debug.LogWarning("simple item is null, trying to connect automatically", this);

            if(TryGetComponent(out SimpleItem item))
            {
                mySimpleItem = item;
                Debug.Log("could autoconnet simple item", this);
            }
        }
        else if(mySimpleItem == null)
        {
            Debug.LogWarning("my simple item is null and blocking autoconnect", this);
        }
    }
#endif

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        animator.Play(animationNameClick);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(!mySimpleItem.TryConsumeBlockReleaseAnimation())
        {
            animator.Play(animationNameRelease);
        }
    }
}
