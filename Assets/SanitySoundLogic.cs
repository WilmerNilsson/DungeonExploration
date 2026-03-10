using FMOD.Studio;
using UnityEngine;

public class SanitySoundLogic : MonoBehaviour
{
    [SerializeField] private string sanityPath;

    private void Start()
    {
        if (!AudioManager.IsValid) return;
        AudioManager.Instance.CreateInstance(sanityPath, gameObject);
        AudioManager.Instance.StartEvent(sanityPath, gameObject);
    }
    
    public void OnLoseSanity(float sanity)
    {
        if (!AudioManager.IsValid) return;
        AudioManager.Instance.SetGlobalParameter("Sanity", sanity);
    }

    private void OnDestroy()
    {
        AudioManager.Instance.StopEvent(sanityPath, STOP_MODE.ALLOWFADEOUT, gameObject);
        AudioManager.Instance.ReleaseInstance(sanityPath, gameObject);
    }
}
