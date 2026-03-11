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

    public void SetDialogueData(DialogueContainer dialogueContainer)
    {
        FriendshipLevel = dialogueContainer.FriendshipLevel;
        
        NodeLinks.Clear();
        foreach (var nodeLink in dialogueContainer.NodeLinks)
        {
            NodeLinks.Add(nodeLink);
        }
        
        DialogueNodeDatas.Clear();
        foreach (var dialogueNodeData in dialogueContainer.DialogueNodeDatas)
        {
            DialogueNodeDatas.Add(dialogueNodeData);
        }
        
        ExposedProperties.Clear();
        foreach (var exposedProperty in dialogueContainer.ExposedProperties)
        {
            ExposedProperties.Add(exposedProperty);
        }
    }
}
