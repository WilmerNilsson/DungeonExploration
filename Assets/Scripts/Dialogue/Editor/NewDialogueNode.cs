using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NewDialogueNode : Node
{
    public string GUID;

    public string Title;
    
    public string ButtonText;
    
    public bool EntryPoint = false;
    
    public TextAsset DialogueAsset;
    
    public bool HasBeenRead;

    public int ReadRun;

    public int RunWaitAmount;
}
