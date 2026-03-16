using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public bool canInteract = true;
    public UnityEvent OnInteract;
    public UnityEvent OnSeen;
    public UnityEvent OnUnSeen;

    public void OnView(bool isSeen)
    {
        if (isSeen && canInteract)
        {
            OnSeen?.Invoke();
        }
        else
        {
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
