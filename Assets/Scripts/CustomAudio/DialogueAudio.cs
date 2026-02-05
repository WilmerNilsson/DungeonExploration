using UnityEngine;

public class DialogueAudio : MonoBehaviour
{
    //WIP - Bara skelett just nu för att försöka lista ut hur saker kanske kommer funka
    //Tänker att denna sitter på samma objekt som dialogueManager och får sina instruktioner av eventsen som sitter på dialogueManager
    //Om det på något sätt går att definera olika karaktärer i Ink kanske man borde försöka koppla det i detta skriptet så att sånt löser sig själv automatiskt???
    //Vi behöver lista ut ett system mellan programmerare, ljud&musik och writers för att skapa ett bra system.
    
    public void InitializeDialogue(string path)
    {
        if (!AudioManager.IsValid)
        {
            Debug.LogWarning("There is no AudioManager in the scene, please add one");
            return;
        }
        AudioManager.Instance.InitializeDialogue(path);
    }

    public void SayLine(string path, string lineParameter, int lineIndex)
    {
        if (!AudioManager.IsValid)
        {
            Debug.LogWarning("There is no AudioManager in the scene, please add one");
            return;
        }
        AudioManager.Instance.SayLine(path, lineParameter, lineIndex);
    }

    public void StopLine(string path)
    {
        if (!AudioManager.IsValid)
        {
            Debug.LogWarning("There is no AudioManager in the scene, please add one");
            return;
        }
        AudioManager.Instance.StopLine(path);
    }

    public void EndDialogue(string path)
    {
        if (!AudioManager.IsValid)
        {
            Debug.LogWarning("There is no AudioManager in the scene, please add one");
            return;
        }
        AudioManager.Instance.EndDialogue(path);
    }
}
