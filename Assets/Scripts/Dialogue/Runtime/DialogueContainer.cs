using System;
using System.Collections.Generic;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEngine;

[Serializable]
public class DialogueContainer : ScriptableObject
{
    public int FriendshipLevel = 1;
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    public List<DialogueNodeData> DialogueNodeDatas = new List<DialogueNodeData>();
    public List<ExposedProperty>  ExposedProperties = new List<ExposedProperty>();

    public void Clear()
    {
        NodeLinks.Clear();
        DialogueNodeDatas.Clear();
        ExposedProperties.Clear();
    }

    public void MarkNodeAsRead(string nodeName)
    {
        DialogueNodeData dialogueNodeData = DialogueNodeDatas.Find(x => x.Title == nodeName);
        if (dialogueNodeData != null)
        {
            dialogueNodeData.HasBeenRead = true;
        }
        else
        {
            Debug.LogWarning("No dialogue node with the name " + nodeName, this);
        }
    }
    
    public void MarkNodeAsUnread(string nodeName)
    {
        DialogueNodeData dialogueNodeData = DialogueNodeDatas.Find(x => x.Title == nodeName);
        if (dialogueNodeData != null)
        {
            dialogueNodeData.HasBeenRead = false;
        }
        else
        {
            Debug.LogWarning("No dialogue node with the name " + nodeName, this);
        }
    }

    public void SetFriendshipLevel(int level)
    {
        FriendshipLevel = level;
    }

    public void IncreaseFriendshipLevel(int amount)
    {
        FriendshipLevel += amount;
    }

    public void SetDialogueData(DialogueSaveData saveData)
    {
        FriendshipLevel = saveData.FriendshipLevel;
        
        for (int i = 0; i < saveData.Guid.Count; i++)
        {
            DialogueNodeData currentNode = DialogueNodeDatas.Find(x => x.Guid  == saveData.Guid[i]);
            currentNode.HasBeenRead = saveData.HasBeenRead[i];
            currentNode.ReadRun = saveData.ReadRun [i];
        }
    }
}
