using System;
using UnityEngine;

public class DialogueTester : MonoBehaviour
{
    public string path;

    public void InitializeDialogue()
    {
        if (!AudioManager.IsValid)
        {
            Debug.LogWarning("There is no AudioManager in the scene, please add one");
            return;
        }
        AudioManager.Instance.InitializeDialogue(path);
    }

    public string lineParameter;
    [Range(0, 10)]public int lineIndex;

    public void SayLine()
    {
        if (!AudioManager.IsValid)
        {
            Debug.LogWarning("There is no AudioManager in the scene, please add one");
            return;
        }
        AudioManager.Instance.SayLine(path, lineParameter, lineIndex);
    }

    public void StopLine()
    {
        if (!AudioManager.IsValid)
        {
            Debug.LogWarning("There is no AudioManager in the scene, please add one");
            return;
        }
        AudioManager.Instance.StopLine(path);
    }

    public void EndDialogue()
    {
        if (!AudioManager.IsValid)
        {
            Debug.LogWarning("There is no AudioManager in the scene, please add one");
            return;
        }
        AudioManager.Instance.EndDialogue(path);
    }
}
