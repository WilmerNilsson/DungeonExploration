using System;
using UnityEngine;

[System.Serializable]
public class GlobalSettings
{
    //savefile
    public int LastSaveFileNr;

    //Volume
    public float MasterVolume = 50f; public float EffectsVolume = 100f; public float MusicVolume = 100f;

    //Languige is taken care of automaticly apparently
    //Gameplay
    public bool ConflictingControllsNeutralizes = false;

    public int Fov = 80;

#nullable enable

    /// <summary>
    /// sends out the new value
    /// </summary>
    public event Action<int>? OnFovChange;

    public void ChangeFovWithNotify(int newFov)
    {
        Fov = newFov;

        OnFovChange?.Invoke(Fov);
    }
}