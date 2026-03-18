using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MinimapLibrarySO", menuName = "Scriptable Objects/MinimapLibrarySO")]
public class MinimapLibrarySO : ScriptableObject
{
    public List<GameObject> minimapObjects;
}
