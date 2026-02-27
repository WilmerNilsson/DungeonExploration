using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueNode
{
    public string Name;
    public string ButtonText;
    public TextAsset InkJson;
    public bool HasBeenRead;
    public bool IsUnreadable;
    public List<string> PrerequisiteNames;
}
