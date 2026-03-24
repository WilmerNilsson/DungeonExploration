using FMOD.Studio;
using UnityEngine;

public class CutsceneSoundLogic : MonoBehaviour
{
    [SerializeField] private string musicPath;
    [SerializeField] private string ambiancePath;
    [SerializeField] private string vaPath;
    [SerializeField] private string lineParameter;

    private void Start()
    {
        if (AudioManager.IsValid)
        {
            AudioManager.Instance.CreateInstance(musicPath);
            AudioManager.Instance.StartEvent(musicPath);
            if (ambiancePath != "" || ambiancePath != null)
            {
                AudioManager.Instance.CreateInstance(ambiancePath);
                AudioManager.Instance.StartEvent(ambiancePath);
            }
            if (vaPath != "" || vaPath != null)
            {
                AudioManager.Instance.InitializeDialogue(vaPath);
            }
        }
    }

    public void ProgressMusic(int progress)
    {
        if (AudioManager.IsValid)
        {
            AudioManager.Instance.SetParameter(musicPath, "Progress", progress);
        }
    }

    public void ProgressAmbiance(int progress)
    {
        if (AudioManager.IsValid)
        {
            AudioManager.Instance.SetParameter(ambiancePath, "Progress", progress);
        }
    }

    public void NextLine(int lineIndex)
    {
        if (AudioManager.IsValid)
        {
            AudioManager.Instance.SetParameter(vaPath, lineParameter, lineIndex);
            AudioManager.Instance.StartEvent(vaPath);
        }
    }

    public void StopLine()
    {
        if (AudioManager.IsValid)
        {
            AudioManager.Instance.StopEvent(vaPath, STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void OnDestroy()
    {
        if (AudioManager.IsValid)
        {
            AudioManager.Instance.StopEvent(musicPath, STOP_MODE.ALLOWFADEOUT);
            AudioManager.Instance.ReleaseInstance(musicPath);
            if (ambiancePath != "" || ambiancePath != null)
            {
                AudioManager.Instance.StopEvent(ambiancePath, STOP_MODE.ALLOWFADEOUT);
                AudioManager.Instance.ReleaseInstance(ambiancePath);
            }
            if (vaPath != "" || vaPath != null)
            {
                AudioManager.Instance.EndDialogue(vaPath);
            }
        }
    }
}
