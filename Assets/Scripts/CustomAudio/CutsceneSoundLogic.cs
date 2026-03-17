using System;
using FMOD.Studio;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class CutsceneSoundLogic : MonoBehaviour
{
    [SerializeField] private string musicPath;
    [SerializeField] private string vaPath;

    private void Start()
    {
        if (AudioManager.IsValid)
        {
            AudioManager.Instance.CreateInstance(musicPath);
            AudioManager.Instance.StartEvent(musicPath);
            AudioManager.Instance.InitializeDialogue(vaPath);
        }
    }

    public void ProgressMusic(int progress)
    {
        if (AudioManager.IsValid)
        {
            AudioManager.Instance.SetParameter(musicPath, "Progress", progress);
        }
    }

    public void NextLine(int lineIndex)
    {
        if (AudioManager.IsValid)
        {
            AudioManager.Instance.SetParameter(vaPath, "LineIndex", lineIndex);
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
            AudioManager.Instance.EndDialogue(vaPath);
        }
    }
}
