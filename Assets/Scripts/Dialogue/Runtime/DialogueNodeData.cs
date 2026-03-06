using System;
using UnityEngine;

[Serializable]
public class DialogueNodeData
{
    public string Guid;
    public string Title;
    public string ButtonText;
    public TextAsset DialogueAsset;
    public bool HasBeenRead;
    public Vector2Int FriendshipRange;
    public Vector2 Position;
    public int ReadRun;
    public int RunWaitAmount;
}
