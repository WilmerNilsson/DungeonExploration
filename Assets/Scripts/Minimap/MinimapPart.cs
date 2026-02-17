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
        Debug.Log("rotate");
        float Y = transform.eulerAngles.y;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
        Vector3 bounds = Renderer.bounds.extents * 2;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Y, transform.eulerAngles.z);
        return bounds;
    }
}
