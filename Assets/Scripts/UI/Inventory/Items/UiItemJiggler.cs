using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class UiItemJiggler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private string animationNameClick = "UIItemJiggleAnimation";
    [SerializeField] private string animationNameRelease = "UIItemJiggleAnimation";

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
        animator.Play(animationNameRelease);
    }
}
