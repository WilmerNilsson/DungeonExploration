using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MinimapSO_test", menuName = "Scriptable Objects/MinimapSO_test")]
public class MinimapSO_test : ScriptableObject
{
    [Header("The minimap component stuff")]
    public List<MinimapComponentData> minimapComponentData;

    public void AddToLists(string nameToAdd, Transform transform, Vector3 scale)
    {
        MinimapComponentData newData = new MinimapComponentData(nameToAdd, transform.position, scale, transform.eulerAngles);
        if (minimapComponentData.Find(x => x.position  == newData.position) == null)
        {
            Debug.Log("adding minimap component " + nameToAdd);
            minimapComponentData.Add(newData);
        }
    }

    public void ClearAll()
    {
        minimapComponentData.Clear();
    }

    public void SetToData(List<MinimapComponentData> newData)
    {
        minimapComponentData.Clear();
        minimapComponentData.AddRange(newData);
    }
}
