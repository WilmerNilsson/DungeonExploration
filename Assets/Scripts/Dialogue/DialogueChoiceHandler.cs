using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueChoiceHandler : MonoBehaviour
{
    private void Awake()
    {
        HandleDialogue();
    }

    [SerializeField] private List<DialogueSelectButton> selectButtons = new List<DialogueSelectButton>();
    private void HandleDialogue()
    {
        if (!DialogueManager.GetInstance())
        {
            Debug.LogWarning("DialogueManager not found", this);
            return;
        }
        DialogueTree dialogueTree = DialogueManager.GetInstance().dialogueTree;
        for (int i = 0; i < selectButtons.Count; i++)
        {
            selectButtons[i].gameObject.SetActive(true);
        }
        int currentButtons = 0;
        for (int i = 0; i < dialogueTree.Dialogues.Count && currentButtons < selectButtons.Count; i++) //all dialogues in tree
        {
            if (!dialogueTree.Dialogues[i].HasBeenRead || !dialogueTree.Dialogues[i].IsUnreadable) //do not display dialogue that has already been read
            {
                bool prerequisitesRead = true;
                for (int j = 0; j < dialogueTree.Dialogues[i].PrerequisiteNames.Count && prerequisitesRead; j++) //all prerequisites to play this dialogue
                {
                    for (int k = 0; k < dialogueTree.Dialogues.Count; k++) //all dialogues in tree
                    {
                        if (dialogueTree.Dialogues[i].PrerequisiteNames[j] == dialogueTree.Dialogues[k].Name) //find prerequisites
                        {
                            if (!dialogueTree.Dialogues[k].HasBeenRead) //check if prerequisite is read
                            {
                                prerequisitesRead = false;
                                break;
                            }
                        }
                    }
                }

                if (prerequisitesRead)
                {
                    selectButtons[currentButtons].DialogueName =  dialogueTree.Dialogues[i].Name;
                    selectButtons[currentButtons].buttonText.text = dialogueTree.Dialogues[i].ButtonText;
                    currentButtons++;
                }
            }
        }

        if (currentButtons < selectButtons.Count)
        {
            for (int i = currentButtons; i < selectButtons.Count; i++)
            {
                selectButtons[i].gameObject.SetActive(false);
            }
        }
    }
}
