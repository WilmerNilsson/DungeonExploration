using System.Collections.Generic;
using UnityEngine;

public class NewDialogueChoiceHandler : MonoBehaviour
{
    private void Awake()
    {
        HandleDialogue();
    }

    [SerializeField] private List<DialogueSelectButton> selectButtons = new List<DialogueSelectButton>();
    private void HandleDialogue()
    {
        if (!NewDialogueManager.GetInstance())
        {
            Debug.LogWarning("DialogueManager not found", this);
            return;
        }
        DialogueContainer dialogueTree = NewDialogueManager.GetInstance().dialogueTree;
        for (int i = 0; i < selectButtons.Count; i++)
        {
            selectButtons[i].gameObject.SetActive(false);
        }
        List<DialogueNodeData> currentNodes = new List<DialogueNodeData>();
        for (int i = 0; i < dialogueTree.DialogueNodeData.Count && currentNodes.Count < selectButtons.Count; i++)
        {
            FindDialogue(dialogueTree, currentNodes, dialogueTree.DialogueNodeData[i]);
        }

        for (int i = 0; i < currentNodes.Count; i++)
        {
            selectButtons[i].gameObject.SetActive(true);
            selectButtons[i].DialogueName = currentNodes[i].DialogueAsset.name;
            selectButtons[i].buttonText.text = currentNodes[i].ButtonText;
        }
    }

    private void FindDialogue(DialogueContainer dialogueContainer, List<DialogueNodeData> dialogueNodeDatas, DialogueNodeData dialogueNodeData)
    {
        //Find links
        if (dialogueNodeData.HasBeenRead || !dialogueNodeData.DialogueAsset)
        {
            return;
        }
        List<NodeLinkData> links = dialogueContainer.NodeLinks.FindAll(x => x.TargetNodeGuid == dialogueNodeData.Guid);
        bool parentsRead = true;
        for (int i = 0; i < links.Count && parentsRead; i++)
        {
            //Check if parent is read
            Debug.Log(links[i].BaseNodeGuid);
            for (int j = 0; j < dialogueContainer.DialogueNodeData.Count; j++)
            {
                Debug.Log(dialogueContainer.NodeLinks[j].BaseNodeGuid);
                Debug.Log(dialogueContainer.NodeLinks[j].TargetNodeGuid);
            }
            parentsRead = dialogueContainer.DialogueNodeData.Find(x => x.Guid == links[i].BaseNodeGuid).HasBeenRead;
        }

        if (parentsRead)
        {
            dialogueNodeDatas.Add(dialogueNodeData);
        }
    }
}
