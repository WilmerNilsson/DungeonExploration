using TMPro;
using UnityEngine;

public class DialogueSelectButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public string DialogueName;

    public void OnClick()
    {
        DialogueManager.GetInstance().EnterDialogueMode(DialogueName);
    }
}
