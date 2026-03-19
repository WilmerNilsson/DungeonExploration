using UnityEngine;
using UnityEngine.Events;

public class BreakableWall : MonoBehaviour, IEnabledHelper
{
    [SerializeField] private string phase2TriggerName;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject disableOnEnableFromSave;

    public UnityEvent OnPhase2;

    private bool phase2triggerd;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(animator == null)
        {
            Debug.LogWarning("animator is null", this);
        }
    }
#endif

    public void Phase2Anim()
    {
        phase2triggerd = true;

        OnPhase2?.Invoke();

        animator.SetTrigger(phase2TriggerName);
    }

    public void EnableFromSave()
    {
        disableOnEnableFromSave.SetActive(false);
    }

    public bool IsEnabledForSave()
    {
        return phase2triggerd;
    }
}
