using System;
using System.Collections.Generic;
using UnityEngine;

public class MinimapArea : MonoBehaviour
{
    public List<GameObject> children = new List<GameObject>();


    public void ReturnDecendantOfParent(GameObject parent, List<GameObject> children)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.gameObject.layer == LayerMask.NameToLayer("Minimap"))
            {
                children.Add(child.gameObject);
            }
            else
            {
                ReturnDecendantOfParent(child.gameObject, children);
            }
        }
    }
    private void Awake()
    {
        ReturnDecendantOfParent(this.gameObject, children);
    }

    public void DrawArea()
    {
        foreach (GameObject child in children)
        {
            child.SetActive(true);
        }
    }
}
