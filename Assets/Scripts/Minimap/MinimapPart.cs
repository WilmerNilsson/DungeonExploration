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

    public Vector3 GetBounds()
    {
        return Renderer.bounds.extents * 2;
    }
}
