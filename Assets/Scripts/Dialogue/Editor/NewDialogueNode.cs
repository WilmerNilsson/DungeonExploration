using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NewDialogueNode : Node
{
    public string GUID;
    
    public string DialogueText;
    
    public bool EntryPoint = false;
    
    public TextAsset DialogueAsset;
}
