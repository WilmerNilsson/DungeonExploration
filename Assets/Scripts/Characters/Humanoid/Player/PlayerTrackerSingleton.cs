using System;
using UnityEngine;

public class PlayerTrackerSingleton : MonoBehaviour
{
    public static PlayerTrackerSingleton Instance;

    public GameObject player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        player = this.gameObject;
    }
}
