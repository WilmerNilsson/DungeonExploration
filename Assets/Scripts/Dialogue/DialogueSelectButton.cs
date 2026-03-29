using TMPro;
using UnityEngine;

public class DialogueSelectButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public string DialogueName;
    [SerializeField] private NewDialogueManager dialogueManager;

    public void OnClick()
    {
        dialogueManager.EnterDialogueMode(DialogueName);
    }
}
