using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InkContainers", menuName = "Scriptable Objects/InkContainers")]
public class InkContainers : ScriptableObject
{
    public List<TextAsset> inkJsons = new List<TextAsset>();
}
