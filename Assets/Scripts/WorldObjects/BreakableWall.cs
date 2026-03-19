using UnityEngine;

public class BreakableWall : MonoBehaviour, IEnabledHelper
{
    [SerializeField] private string phase2TriggerName;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject disableOnEnableFromSave;

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
