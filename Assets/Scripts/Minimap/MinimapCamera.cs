using System;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [Tooltip("A list of transforms that represent the possible positions of the camera")]
    [SerializeField] private List<Transform> cameraPositions;

    private void Awake()
    {
        if (cameraPositions.Count > 0)
        {
            transform.position = cameraPositions[0].position;
        }
    }

    public void SetFloor(int floor)
    {
        if (floor > cameraPositions.Count - 1)
        {
            Debug.LogWarning("floor is out of range", this);
            return;
        }
        transform.position = cameraPositions[floor - 1].position;
    }
}
