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
        return Quaternion.AngleAxis(-transform.rotation.eulerAngles.y, Vector3.up) * Renderer.bounds.extents * 2;
    }
}
