using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueContainer : ScriptableObject
{
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
}
