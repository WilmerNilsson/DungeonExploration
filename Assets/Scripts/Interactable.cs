using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject graphicModel;
    public bool canInteract = true;
    public UnityEvent OnInteract;
    public UnityEvent OnSeen;
    public UnityEvent OnUnSeen;
    
    private int defaultLayer;

    private void Awake()
    {
        defaultLayer = graphicModel.layer;
    }

    public void OnView(bool isSeen)
    {
        if (isSeen && canInteract)
        {
            graphicModel.layer = LayerMask.NameToLayer("Highlight");

            OnSeen?.Invoke();
        }
        else
        {
            graphicModel.layer = defaultLayer;

            OnUnSeen?.Invoke();
        }
    }

    public void Interact()
    {
        if (canInteract) OnInteract?.Invoke();
    }

    public void SetInteractability(bool value)
    {
        canInteract = value;
    }
}
