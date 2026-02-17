using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable

[CreateAssetMenu(fileName = "GameManagerSO", menuName = "Scriptable Objects/GameManagerSO")]
public class GameManagerSO : ScriptableObject
{
    private const int mainMenuSceneNumber = 0;
    private const int mainSceneNumber = 1;
    private const string MasterSoundName = "Master";
    private const string SoundEffectsSoundName = "SFX";
    private const string MusicSoundName = "Music";

    public SaveFileManager SavefileManager = new();
    private SavefileData? tempSavefile;

    private static GameManagerSO? instance;
    private bool hasLoadedSettings = false;

    private int thingsFreezingGame = 0;
    public bool IsGameFrozen
    {
        get { return thingsFreezingGame > 0; }
    }
    private int thingsLockingMouse = 0;
    private int thingsLockingCamera = 0;

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public event Action<bool>? OnFreezeGameChangeSelfReset;
    /// <summary>
    /// will not reset on scene change, care for memory leaks
    /// </summary>
    public event Action<bool>? OnFreezeGameChange;
    /// <summary>
    /// will not reset on scene change, care for memory leaks
    /// </summary>
    public event Action<int>? OnLoadScene;
    /// <summary>
    /// will not reset on scene change, care for memory leaks
    /// </summary>
    public event Action<bool>? OnLockMouse;
    public event Action<bool>? OnLockCamera;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

    private void FirstAcces()
    {
        if(!hasLoadedSettings)
        {
            //updating volume is sound peoples thing, ask them if needed
            //put things here instad of OnEnable etc

            SavefileManager.ReadGlobalSettings();
            hasLoadedSettings = true;
        }
    }

    public static GameManagerSO Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.LoadAll<GameManagerSO>("")[0];
            }
            instance.FirstAcces();
            return instance;
        }
    }

    //called on awake in GameManagerReseter;
    public void ResetManagerVariables()
    {
        thingsFreezingGame = 0;
        thingsLockingMouse = 0;
        thingsLockingCamera = 0;
        hasLoadedSettings = false;
    }

    #region move to scene stuff

    public void StartDemo()
    {
        MoveToScene(Vector2.zero, mainSceneNumber);
    }

    /// <summary>
    /// If we move to a new scene trough a door or passage.
    /// </summary>
    /// <param name="newLocation">Where te exit of said passage is.</param>
    public void MoveToScene(Vector2 newLocation, int newSceneNr)
    {
        //currentSavefileData.sceneNr = newSceneNr;
        //currentSavefileData.savePos = newLocation;

        if(newSceneNr != SceneManager.GetActiveScene().buildIndex)
        {
            ResetActions();

            if(newSceneNr == mainMenuSceneNumber)
            {
                Time.timeScale = 1;
            }

            if(OnLoadScene != null)
            {
                OnLoadScene(newSceneNr);
            }

            SceneManager.LoadScene(newSceneNr);
        }
    }

    private void ResetActions()
    {
        OnFreezeGameChangeSelfReset = null;
    }
    #endregion

    #region  GamePlayCheats Unimplimented

    #endregion

    #region Timescale and mouselock
    public float GetTimeScale()
    {
        return SavefileManager.SavefileSettings.NormalTimescale;
    }

    public void SetTimeScale(float newValue)
    {
        SavefileManager.SavefileSettings.NormalTimescale = newValue;
    }

    public void FreezeTime(bool value)
    {
        bool wasFrozen = thingsFreezingGame != 0;

        if(value)
        {
            thingsFreezingGame++;
        }
        else
        {
            thingsFreezingGame--;
        }

        if(!wasFrozen && thingsFreezingGame != 0)
        {
            Time.timeScale = 0;

            OnFreezeGameChangeSelfReset?.Invoke(true);
            OnFreezeGameChange?.Invoke(true);
        }
        else if(wasFrozen && thingsFreezingGame == 0)
        {
            Time.timeScale = SavefileManager.SavefileSettings.NormalTimescale;

            OnFreezeGameChangeSelfReset?.Invoke(false);
            OnFreezeGameChange?.Invoke(false);
        }
    }
    
    public void LockMouse(bool value)
    {
        bool wasLocked = thingsLockingMouse != 0;

        if (value)
        {
            thingsLockingMouse++;
        }
        else
        {
            thingsLockingMouse--;
        }

        if(!wasLocked && thingsLockingMouse != 0)
        {
            LockCamera(true);
            OnLockMouse?.Invoke(true);
        }
        else if (wasLocked && thingsLockingMouse == 0)
        {
            LockCamera(false);
            OnLockMouse?.Invoke(false);
        }
    }

    public void LockCamera(bool value)
    {
        bool wasLocked = thingsLockingCamera != 0;
        
        if (value)
        {
            thingsLockingCamera++;
        }
        else
        {
            thingsLockingCamera--;
        }
        
        if(!wasLocked && thingsLockingCamera != 0)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined; //we need to controll curson with a pause menu once implimented
            OnLockCamera?.Invoke(true);
        }
        else if (wasLocked && thingsLockingCamera == 0)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            OnLockCamera?.Invoke(false);
        }
    }

    #endregion

    #region Sound
    public float GetMasterVolume()
    {
        return SavefileManager.GlobalSettings.MasterVolume;
    }

    public float GetEffectsVolume()
    {
        return SavefileManager.GlobalSettings.EffectsVolume;
    }

    public float GetMusicVolume()
    {
        return SavefileManager.GlobalSettings.MusicVolume;
    }

    public void UpdateVolumes()
    {
        UpdateMixerEffectsVolume();
        UpdateMixerMasterVolume();
        UpdateMixerMusicVolume();
    }

    public void SetMasterVolume(float newValue)
    {
        SavefileManager.GlobalSettings.MasterVolume = newValue;
        UpdateMixerMasterVolume();
    }

    private void UpdateMixerMasterVolume()
    {
        if(AudioManager.IsValid)
        {
            AudioManager.Instance.SetVolume(MasterSoundName, SavefileManager.GlobalSettings.MasterVolume / 100f);
        }
    }

    public void SetEffectsVolume(float newValue)
    {
        SavefileManager.GlobalSettings.EffectsVolume = newValue;
        UpdateMixerEffectsVolume();
    }

    private void UpdateMixerEffectsVolume()
    {
        if (AudioManager.IsValid)
        {
            AudioManager.Instance.SetVolume(SoundEffectsSoundName, SavefileManager.GlobalSettings.EffectsVolume / 100f);
        }
    }

    public void SetMusicVolume(float newValue)
    {
        SavefileManager.GlobalSettings.MusicVolume = newValue;
        UpdateMixerMusicVolume();
    }

    public void UpdateMixerMusicVolume()
    {
        if (AudioManager.IsValid)
        {
            AudioManager.Instance.SetVolume(MusicSoundName, SavefileManager.GlobalSettings.MusicVolume / 100f);
        }
    }
    #endregion

    #region  SavefilesStuff

    public bool TryConsumeSavefileData([NotNullWhen(true)] out SavefileData? data)
    {
        data = tempSavefile;
        tempSavefile = null;
        return data != null;
    }

    /// <summary>
    /// should only be called from SaveFileManager
    /// </summary>
    public void LoadSavefileScene(SavefileData data)
    {
        Time.timeScale = data.Settings.NormalTimescale;
        OnLoadScene?.Invoke(data.SceneNr);
        SceneManager.LoadScene(data.SceneNr);
        Debug.Log("todo initialize scene");
    }

    public bool GetConflictingControllsNeutralizes()
    {
        return SavefileManager.GlobalSettings.ConflictingControllsNeutralizes;
    }
    #endregion
}