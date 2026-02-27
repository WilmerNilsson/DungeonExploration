using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueTree", menuName = "Scriptable Objects/DialogueTree")]
public class DialogueTree : ScriptableObject
{
    public List<DialogueNode> Dialogues = new List<DialogueNode>();

    public void resetRead()
    {
        foreach (DialogueNode node in Dialogues)
        {
            node.HasBeenRead = false;
        }
    }
}
