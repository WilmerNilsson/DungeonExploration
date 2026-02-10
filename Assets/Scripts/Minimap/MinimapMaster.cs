using System;
using UnityEngine;

public class MinimapMaster : MonoBehaviour
{
    public static MinimapMaster Instance
    {
        get; private set; 
    }
    [SerializeField] private GameObject minimap;

    [SerializeField] private MinimapSO_test minimapSoTest;
    //[SerializeField] private PlayerController playerController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        for (int i = 0; i < minimapSoTest.minimapObjects.Count; i++)
        {
            GameObject currentMinimap = Instantiate(minimapSoTest.minimapObjects[i]);
            currentMinimap.transform.position = minimapSoTest.minimapPositions[i];
            currentMinimap.layer = LayerMask.NameToLayer("Minimap");
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
        //playerController.LockMovement(true);
    }

    public void CloseMinimap()
    {
        minimap.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //playerController.LockMovement(false);
    }
}
