using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent OnInteract;
    public UnityEvent OnSeen;
    public UnityEvent OnUnSeen;

    public void OnView(bool isSeen)
    {
        if (isSeen)
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
        OnInteract?.Invoke();
    }
}
