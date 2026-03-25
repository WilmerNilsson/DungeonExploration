using System;
using System.Collections.Generic;
using UnityEngine;

public class MinimapPart : MonoBehaviour
{
    [Tooltip("The minimap prefab corresponding to this object")]
    public GameObject prefab;
    
    private Renderer Renderer;

    private void Awake()
    {
        Renderer = GetComponent<Renderer>();
    }

    public Vector3 GetRotatedBounds()
    {
        if (!Renderer)
        {
            Debug.LogWarning("No renderer, using scale instead", this);
            return transform.localScale;
        }

        //gameObject.isStatic = false;
        Vector3 originalRotation = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0, 0, 0);
        Vector3 bounds = Renderer.bounds.extents * 2;
        transform.eulerAngles = originalRotation;
        //gameObject.isStatic = true;
        return bounds;
    }
}
