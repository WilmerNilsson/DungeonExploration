using System.Collections.Generic;
using UnityEngine;

public class DialogueSaveData
{
    public string TreeName;
    public int FriendshipLevel;
    public List<string> Guid = new List<string>();
    public List<bool> HasBeenRead = new List<bool>();
    public List<int> ReadRun = new List<int>();

    public DialogueSaveData(DialogueContainer dialogueContainer)
    {
        TreeName  = dialogueContainer.name;
        FriendshipLevel = dialogueContainer.FriendshipLevel;
        for (int i = 0; i < dialogueContainer.DialogueNodeDatas.Count; i++)
        {
            Guid.Add(dialogueContainer.DialogueNodeDatas[i].Guid);
            HasBeenRead.Add(dialogueContainer.DialogueNodeDatas[i].HasBeenRead);
            ReadRun.Add(dialogueContainer.DialogueNodeDatas[i].ReadRun);
        }
    }

    public DialogueSaveData(string name, int friendshipLevel, List<string> guid, List<bool> hasBeenRead, List<int> readRun)
    {
        TreeName  = name;
        FriendshipLevel  = friendshipLevel;
        Guid = guid;
        HasBeenRead = hasBeenRead;
        ReadRun = readRun;
    }

    public DialogueSaveData Clone()
    {
        return new (TreeName, FriendshipLevel, Guid, HasBeenRead, ReadRun);
    }
}
