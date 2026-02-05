using UnityEngine;
using UnityEngine.Events;

public class UnityEventTest : MonoBehaviour
{
    public UnityEvent OnPlay;

    [ContextMenu("Play")]
    public void Play()
    {
        OnPlay.Invoke();
    }
}
