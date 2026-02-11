using System;
using System.Collections.Generic;
using UnityEngine;

public class MinimapTrigger : MonoBehaviour
{
    //private List<MinimapArea> mapAreas = new List<MinimapArea>();

    /*public void DrawMap()
    {
        foreach (MinimapArea mapArea in mapAreas)
        {
            mapArea.DrawArea();
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out MinimapArea mapArea))
        {
            foreach (Collider collider in other.GetComponents<Collider>())
            {
                collider.enabled = false;
            }
            //mapAreas.Add(mapArea);
            mapArea.DrawArea();
        }
    }
}
