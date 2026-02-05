using UnityEngine;

public class DialogueTester : MonoBehaviour
{
    public string path;

    public void InitializeDialogue()
    {
        AudioManager.Instance.InitializeDialogue(path);
    }

    public string lineParameter;
    [Range(0, 10)]public int lineIndex;

    public void SayLine()
    {
        AudioManager.Instance.SayLine(path, lineParameter, lineIndex);
    }

    public void StopLine()
    {
        AudioManager.Instance.StopLine(path);
    }

    public void EndDialogue()
    {
        AudioManager.Instance.EndDialogue(path);
    }
}
