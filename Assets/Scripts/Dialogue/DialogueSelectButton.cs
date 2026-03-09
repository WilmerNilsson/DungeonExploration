using TMPro;
using UnityEngine;

public class DialogueSelectButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public string DialogueName;

    public void OnClick()
    {
        NewDialogueManager.GetInstance().EnterDialogueMode(DialogueName);
    }
}
