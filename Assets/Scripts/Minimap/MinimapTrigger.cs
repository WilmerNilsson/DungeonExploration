using System;
using System.Collections.Generic;
using UnityEngine;

public class MinimapTrigger : MonoBehaviour
{
    public List<MinimapArea> mapAreas = new List<MinimapArea>();

    public void DrawMap()
    {
        foreach (MinimapArea mapArea in mapAreas)
        {
            mapArea.DrawArea();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("enter");
        other.GetComponent<Collider>().enabled = false;
        foreach (Collider collider in other.GetComponents<Collider>())
        {
            collider.enabled = false;
        }
        mapAreas.Add(other.gameObject.GetComponent<MinimapArea>());
    }
}
