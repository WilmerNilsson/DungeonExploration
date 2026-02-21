using UnityEngine;
using EventHandler = CustomAudio.EventHandler;

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
        EventHandler.InitializeDialogue(path);
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
        EventHandler.SayLine(path, lineParameter, lineIndex);
    }

    public void StopLine()
    {
        if (!AudioManager.IsValid)
        {
            Debug.LogWarning("There is no AudioManager in the scene, please add one");
            return;
        }
        EventHandler.StopLine(path);
    }

    public void EndDialogue()
    {
        if (!AudioManager.IsValid)
        {
            Debug.LogWarning("There is no AudioManager in the scene, please add one");
            return;
        }
        EventHandler.EndDialogue(path);
    }
}
