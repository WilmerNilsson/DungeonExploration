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
    [Tooltip("The scriptable object named MinimapLibrarySO")]
    [SerializeField] private MinimapLibrarySO minimapLibrary;
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

    public MinimapSO_test GetSO()
    {
        return minimapSoTest;
    }

    public void AddToSO(List<MinimapPart> children)
    {
        foreach (MinimapPart child in children)
        {
            if (child.prefab == null)
            {
                Debug.Log("Minimap Prefab ID is null", child.gameObject);
                continue;
            }
            minimapSoTest.AddToLists(child.prefab.name, child.transform, child.GetRotatedBounds());
        }
    }

    public void SpawnMinimap()
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            Destroy(spawnedObjects[i]);
        }
        spawnedObjects.Clear();
        for (int i = 0; i < minimapSoTest.minimapComponentData.Count; i++)
        {
            GameObject currentMinimap = Instantiate(minimapLibrary.minimapObjects.Find(x => x.name == minimapSoTest.minimapComponentData[i].name), transform);
            currentMinimap.transform.position = minimapSoTest.minimapComponentData[i].position;
            currentMinimap.transform.localScale = minimapSoTest.minimapComponentData[i].scale;
            currentMinimap.transform.eulerAngles = minimapSoTest.minimapComponentData[i].rotation;
            spawnedObjects.Add(currentMinimap);
        }
    }

    public void ToggleMinimap()
    {
        if (GameManagerSO.Instance.IsGameFrozen) return;
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
        if (minimap.activeSelf)
        {
            return;
        }
        if (InvMasterBase.Instance is InvMaster invMaster)
        {
            invMaster.ClosePlayerInventory();
        }
        minimap.SetActive(true);
        Cursor.visible = true;
        GameManagerSO.Instance.LockMouse(true);
    }

    public void CloseMinimap()
    {
        if (!minimap.activeSelf)
        {
            return;
        }
        minimap.SetActive(false);
        Cursor.visible = false;
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

    public bool GetIsActive()
    {
        return minimap.activeSelf;
    }
}
