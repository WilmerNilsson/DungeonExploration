using UnityEngine;
using UnityEngine.Events;

public class AnimationTrigger : MonoBehaviour
{
    public UnityEvent onTrigger;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Trigger()
    {
        onTrigger.Invoke();
    }
}
