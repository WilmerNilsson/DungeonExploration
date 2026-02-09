using UnityEngine;
using UnityEngine.Events;

public class UnityEventTest : MonoBehaviour
{
    public UnityEvent onPlay;

    [ContextMenu("Play")]
    public void Play()
    {
        onPlay.Invoke();
    }

    public void Add()
    {
        CombatChecker.AddToChaseList(gameObject);
    }

    public void Remove()
    {
        CombatChecker.RemoveFromChaseList(gameObject);
    }
}
