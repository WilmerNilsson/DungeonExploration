using System;
using UnityEngine;

public class PlayerTrackerSingleton : MonoBehaviour
{
    public static PlayerTrackerSingleton Instance;

    public GameObject playerGameObject { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        playerGameObject = this.gameObject;
    }
}
