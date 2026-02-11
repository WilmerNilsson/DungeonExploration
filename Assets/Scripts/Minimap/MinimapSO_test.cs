using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MinimapSO_test", menuName = "Scriptable Objects/MinimapSO_test")]
public class MinimapSO_test : ScriptableObject
{
    [Header("The minimap component stuff")]
    public List<GameObject> minimapObjects;
    public List<Vector3> minimapPositions;
    public List<Vector3> minimapScales;
    public List<Vector3> minimapRotations;

    public void AddToLists(GameObject objectToAdd, Transform transform, Vector3 scale)
    {
        if (!minimapPositions.Contains(transform.position))
        {
            minimapObjects.Add(objectToAdd);
            minimapPositions.Add(transform.position);
            minimapScales.Add(scale);
            minimapRotations.Add(transform.eulerAngles);
        }
    }

    public void ClearAll()
    {
        minimapObjects.Clear();
        minimapPositions.Clear();
        minimapScales.Clear();
        minimapRotations.Clear();
    }
}
