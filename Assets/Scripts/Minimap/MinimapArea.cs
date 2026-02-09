using System;
using System.Collections.Generic;
using UnityEngine;

public class MinimapArea : MonoBehaviour
{
    private List<GameObject> children = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i).gameObject);
            children[i].SetActive(false);
        }
    }

    public void DrawArea()
    {
        foreach (GameObject child in children)
        {
            child.SetActive(true);
        }
    }
}
