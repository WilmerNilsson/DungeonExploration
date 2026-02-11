using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MinimapSO_test", menuName = "Scriptable Objects/MinimapSO_test")]
public class MinimapSO_test : ScriptableObject
{
    [Header("The minimap component stuff")]
    public List<GameObject> minimapObjects;
    public List<Vector3> minimapPositions;
    public List<Vector3> minimapScales;

    public void AddToLists(GameObject objectToAdd, Vector3 positionToAdd, Vector3 scaleToAdd)
    {
        if (!minimapPositions.Contains(positionToAdd))
        {
            minimapObjects.Add(objectToAdd);
            minimapPositions.Add(positionToAdd);
            minimapScales.Add(scaleToAdd);
        }
    }

    public void ClearAll()
    {
        minimapObjects.Clear();
        minimapPositions.Clear();
        minimapScales.Clear();
    }
}
