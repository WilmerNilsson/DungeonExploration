using System;
using System.Collections.Generic;
using UnityEngine;

public class MinimapMaster : MonoBehaviour
{
    public static MinimapMaster Instance
    {
        get; private set; 
    }
    private List<GameObject> spawnedObjects = new List<GameObject>();
    [Tooltip("Most likely the UI where the map will be shown")]
    [SerializeField] private GameObject minimap;

    [Tooltip("The scriptable object belonging to this level")]
    [SerializeField] private MinimapSO_test minimapSoTest;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnValidate()
    {
        if (minimap == null)
        {
            Debug.LogWarning("No minimap object found", this);
        }

        if (minimapSoTest == null)
        {
            Debug.LogWarning("No minimap scriptable object found", this);
        }
    }

    private void Start()
    {
        minimap.SetActive(false);
        SpawnMinimap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        Instance = this;
    }

    public void AddToSO(List<MinimapPart> children)
    {
        foreach (MinimapPart child in children)
        {
            if (child.prefab == null)
            {
                continue;
            }
            minimapSoTest.AddToLists(child.prefab, child.transform, child.GetRotatedBounds());
        }
    }

    public void SpawnMinimap()
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            Destroy(spawnedObjects[i]);
        }
        spawnedObjects.Clear();
        for (int i = 0; i < minimapSoTest.prefabNames.Count; i++)
        {
            GameObject currentMinimap = Instantiate(minimapSoTest.GetPrefabByName(minimapSoTest.prefabNames[i]), transform);
            currentMinimap.transform.position = minimapSoTest.minimapPositions[i];
            currentMinimap.transform.eulerAngles = minimapSoTest.minimapRotations[i];
            currentMinimap.transform.localScale = minimapSoTest.minimapScales[i];
            spawnedObjects.Add(currentMinimap);
        }
    }

    public void ToggleMinimap()
    {
        if(minimap.activeSelf)
        {
            CloseMinimap();
        }
        else
        {
            OpenMinimap();
        }
    }

    public void OpenMinimap()
    {
        minimap.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined; //we need to controll curson with a pause menu once implimented
        GameManagerSO.Instance.LockMouse(true);
    }

    public void CloseMinimap()
    {
        minimap.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameManagerSO.Instance.LockMouse(false);
    }

    public void UnlockFullMinimap()
    {
        Debug.Log("Unlocking full minimap");
        foreach (var area in FindObjectsByType<MinimapArea>(FindObjectsSortMode.None))
        {
            area.DrawArea();
        }
        SpawnMinimap();
    }
}
