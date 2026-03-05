using System;
using UnityEngine;

[Serializable]
public class DialogueNodeData
{
    public string Guid;
    public string DialogueText;
    public TextAsset DialogueAsset;
    public bool HasBeenRead;
    public Vector2 Position;
}
