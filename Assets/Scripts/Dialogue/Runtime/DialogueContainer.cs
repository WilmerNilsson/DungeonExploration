using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueContainer : ScriptableObject
{
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();
    public List<ExposedProperty>  ExposedProperties = new List<ExposedProperty>();

    public void Clear()
    {
        NodeLinks.Clear();
        DialogueNodeData.Clear();
        ExposedProperties.Clear();
    }

    public void MarkNodeAsRead(string nodeName)
    {
        Debug.Log(DialogueNodeData.Count);
        DialogueNodeData.Find(x => x.DialogueAsset.name == nodeName).HasBeenRead = true;
    }
    
    public void MarkNodeAsUnread(string nodeName)
    {
        DialogueNodeData.Find(x => x.DialogueAsset.name == nodeName).HasBeenRead = false;
    }
}
