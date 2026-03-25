using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [Tooltip("A list of cameras that view the different floors")]
    [SerializeField] private List<GameObject> cameras;
    [SerializeField] private GameObject UpButton;
    [SerializeField] private GameObject DownButton;
    [SerializeField] private RenderTexture renderTexture;

    [SerializeField] private TextMeshProUGUI floorText;
    [SerializeField] private int currentFloor;

    private void Awake()
    {
        if (cameras.Count == 0)
        {
            Debug.LogWarning("No minimap cameras found", this);
            return;
        }
        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].SetActive(false);
        }
        cameras[currentFloor - 1].SetActive(true);
        floorText.text = (currentFloor).ToString();
    }

    public void ChangeFloor(int value)
    {
        currentFloor = Mathf.Clamp(currentFloor + value, 1, cameras.Count);
        SetFloor(currentFloor);
    }

    public void SetFloor(int floor)
    {
        renderTexture.Release();
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
