using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueTree", menuName = "Scriptable Objects/DialogueTree")]
public class DialogueTree : ScriptableObject
{
    public int friendshipLevel = 1;
    public List<DialogueNode> Dialogues = new List<DialogueNode>();
    public List<DialogueNode> Greetings  = new List<DialogueNode>();
    public List<DialogueNode> Buy = new List<DialogueNode>();
    public List<DialogueNode> Sell = new List<DialogueNode>();
    public List<DialogueNode> NeutralMoney = new List<DialogueNode>();

    public void resetRead()
    {
        foreach (DialogueNode node in Dialogues)
        {
            node.HasBeenRead = false;
        }
    }

    public void SetReadTrue(string name)
    {
        foreach (DialogueNode node in Dialogues)
        {
            if (node.Name == name)
            {
                node.HasBeenRead = true;
                return;
            }
        }
    }
}
