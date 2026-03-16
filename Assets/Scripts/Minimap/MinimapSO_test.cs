using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MinimapSO_test", menuName = "Scriptable Objects/MinimapSO_test")]
public class MinimapSO_test : ScriptableObject
{
    [Header("The minimap component stuff")]
    public List<GameObject> minimapObjects;
    public List<string> prefabNames;
    public List<Vector3> minimapPositions;
    public List<Vector3> minimapScales;
    public List<Vector3> minimapRotations;

    public void AddToLists(GameObject objectToAdd, Transform transform, Vector3 scale)
    {
        if (!minimapPositions.Contains(transform.position))
        {
            if (!minimapObjects.Contains(objectToAdd))
            {
                minimapObjects.Add(objectToAdd);
            }
            prefabNames.Add(objectToAdd.name);
            minimapPositions.Add(transform.position);
            minimapScales.Add(scale);
            minimapRotations.Add(transform.eulerAngles);
        }
    }

    public GameObject GetPrefabByName(string prefabName)
    {
        return minimapObjects.Find(obj => obj.name == prefabName);
    }

    public void ClearAll()
    {
        minimapObjects.Clear();
        prefabNames.Clear();
        minimapPositions.Clear();
        minimapScales.Clear();
        minimapRotations.Clear();
    }
}
