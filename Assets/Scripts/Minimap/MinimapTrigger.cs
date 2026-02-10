using System;
using System.Collections.Generic;
using UnityEngine;

public class MinimapTrigger : MonoBehaviour
{
    public List<MinimapArea> mapAreas = new List<MinimapArea>();
    [SerializeField] private MinimapSO_test minimapSO;

    public void DrawMap()
    {
        foreach (MinimapArea mapArea in mapAreas)
        {
            mapArea.DrawArea();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out MinimapArea mapArea))
        {
            other.GetComponent<Collider>().enabled = false;
            foreach (Collider collider in other.GetComponents<Collider>())
            {
                collider.enabled = false;
            }
            mapAreas.Add(mapArea);
        }
    }
}
