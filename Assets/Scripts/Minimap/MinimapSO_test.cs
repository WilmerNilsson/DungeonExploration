using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MinimapSO_test", menuName = "Scriptable Objects/MinimapSO_test")]
public class MinimapSO_test : ScriptableObject
{
    public List<GameObject> minimapObjects;
    public List<Vector3> minimapPositions;

    public void AddToLists(GameObject objectToAdd, Vector3 positionToAdd)
    {
        if (!minimapPositions.Contains(positionToAdd))
        {
            minimapObjects.Add(objectToAdd);
            minimapPositions.Add(positionToAdd);
        }
    }

    public void ClearAll()
    {
        minimapObjects.Clear();
        minimapPositions.Clear();
    }
}
