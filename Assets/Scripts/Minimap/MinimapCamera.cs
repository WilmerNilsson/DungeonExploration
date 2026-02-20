using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [Tooltip("A list of cameras that view the different floors")]
    [SerializeField] private List<GameObject> cameras;

    [SerializeField] private TextMeshProUGUI floorText;

    private void Awake()
    {
        if (cameras.Count == 0)
        {
            Debug.LogWarning("No cameras found", this);
            return;
        }
        cameras[0].SetActive(true);
        for (int i = 1; i < cameras.Count; i++)
        {
            cameras[i].SetActive(false);
        }
    }

    public void SetFloor(int floor)
    {
        if (floor > cameras.Count)
        {
            Debug.LogWarning("floor is out of range", this);
            return;
        }

        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].SetActive(false);
        }
        cameras[floor - 1].SetActive(true);
        floorText.text = floor.ToString();
    }
}
