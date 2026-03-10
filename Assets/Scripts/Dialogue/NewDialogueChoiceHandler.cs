using System;
using System.Collections.Generic;
using UnityEngine;

public class NewDialogueChoiceHandler : MonoBehaviour
{
    [SerializeField] private List<DialogueSelectButton> selectButtons = new List<DialogueSelectButton>();
    [SerializeField] private NewDialogueManager dialogueManager;
    private void Awake()
    {
        HandleDialogue();
    }
    public void HandleDialogue()
    {
        DialogueContainer dialogueTree = dialogueManager.dialogueTree;
        for (int i = 0; i < selectButtons.Count; i++)
        {
            selectButtons[i].gameObject.SetActive(false);
        }
        List<DialogueNodeData> currentNodes = new List<DialogueNodeData>();
        for (int i = 0; i < dialogueTree.DialogueNodeDatas.Count && currentNodes.Count < selectButtons.Count; i++)
        {
            FindDialogue(dialogueTree, currentNodes, dialogueTree.DialogueNodeDatas[i]);
        }

        for (int i = 0; i < currentNodes.Count; i++)
        {
            selectButtons[i].gameObject.SetActive(true);
            selectButtons[i].DialogueName = currentNodes[i].Title;
            selectButtons[i].buttonText.text = currentNodes[i].ButtonText;
        }
    }

    private void FindDialogue(DialogueContainer dialogueContainer, List<DialogueNodeData> dialogueNodeDatas, DialogueNodeData dialogueNodeData)
    {
        int runCount = dialogueManager.RunCount;
        if (dialogueNodeData.HasBeenRead || !dialogueNodeData.DialogueAsset || dialogueNodeData.IsGreeting || 
            dialogueNodeData.FriendshipRange.x > dialogueContainer.FriendshipLevel || dialogueNodeData.FriendshipRange.y < dialogueContainer.FriendshipLevel)
        {
            return;
        }
        //Find links
        List<NodeLinkData> links = dialogueContainer.NodeLinks.FindAll(x => x.TargetNodeGuid == dialogueNodeData.Guid);
        bool parentsRead = true;
        for (int i = 0; i < links.Count && parentsRead; i++)
        {
            DialogueNodeData parentData = dialogueContainer.DialogueNodeDatas.Find(x => x.Guid == links[i].BaseNodeGuid);
            //Check if parent is read
            parentsRead = parentData.HasBeenRead && parentData.ReadRun + dialogueNodeData.RunWaitAmount <= runCount;
            //parentsRead = dialogueContainer.DialogueNodeDatas.Find(x => x.Guid == links[i].BaseNodeGuid).HasBeenRead;
        }

        if (parentsRead)
        {
            dialogueNodeDatas.Add(dialogueNodeData);
        }
    }
}
