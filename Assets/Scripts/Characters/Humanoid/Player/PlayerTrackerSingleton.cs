using System;
using UnityEngine;

public class PlayerTrackerSingleton : MonoBehaviour
{
    public static PlayerTrackerSingleton Instance;

    [Tooltip("Sets by itself")] public GameObject player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        player = this.gameObject;
    }
}
