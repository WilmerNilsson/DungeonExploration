using System;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public List<MinimapArea> mapAreas = new List<MinimapArea>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
