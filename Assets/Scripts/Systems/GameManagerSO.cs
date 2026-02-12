using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "GameManagerSO", menuName = "Scriptable Objects/GameManagerSO")]
public class GameManagerSO : ScriptableObject
{
    private const int mainMenuSceneNumber = 0;
    private const int mainSceneNumber = 1;
    private const string MasterSoundName = "Master";
    private const string SoundEffectsSoundName = "SFX";
    private const string MusicSoundName = "Music";

    private GlobalSettings globalSettings = new GlobalSettings();
    private SavefileData currentSavefileData = new SavefileData();
    private SavefileData lastSavedSavefileData = new SavefileData();
    private int currentSavefileNr = 1;

    private static GameManagerSO instance;
    private bool hasLoadedSettings = false;

    private int thingsFreezingGame = 0;
    public bool IsGameFrozen
    {
        get { return thingsFreezingGame > 0; }
    }
    private int thingsLockingMouse = 0;

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public event Action? OnSavePointSaveSelfReset;
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
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

    private void FirstAcces()
    {
        if(!hasLoadedSettings)
        {
            //updating mixer volume is done in the music controller script since i can't make sure that the update mixer methods are called after the mixer loads
            //put things here instad of OnEnable etc

            instance.ReadGlobalSettings();
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
        hasLoadedSettings = false;
    }

    #region move to scene stuff

    public void StartDemo()
    {
        MoveToScene(Vector2.zero, mainSceneNumber);
    }

    public void MoveToScene(Vector2 newLocation, int newSceneNr)
    {
        //currentSavefileData.sceneNr = newSceneNr;
        //currentSavefileData.savePos = newLocation;

        if(newSceneNr != SceneManager.GetActiveScene().buildIndex)
        {
            ResetActions();

            if(newSceneNr == mainMenuSceneNumber) // main menu
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
        OnSavePointSaveSelfReset = null;
        OnFreezeGameChangeSelfReset = null;
    }
    #endregion

    #region SaveFile ID stuff
    public enum IDListName
    {
        DialougeFound,
        BooksInJournal,
    }

    public void AddIDToList(IDListName type, string id)
    {
        switch (type)
        {
            case IDListName.DialougeFound:

                break;
            case IDListName.BooksInJournal:

                break;
#if DEBUG
            default:
                Debug.LogError("trying to add a ID of unimplimented type: " + type, this);
                throw new NotImplementedException("trying to add a ID of unimplimented type: " + type);
#endif
        }
    }

    public bool CheckIfIDExists(IDListName type, string id)
    {
        switch (type)
        {
            case IDListName.DialougeFound:

                break;
            case IDListName.BooksInJournal:

                break;
#if DEBUG
            default:
                Debug.LogError("trying to check a ID of unimplimented type: " + type, this);
                throw new NotImplementedException("trying to check a ID of unimplimented type: " + type);
#endif
        }
        return false;
    }
    #endregion

    #region  GamePlayCheats Unimplimented

    #endregion

    #region Timescale and mouselock
    public float GetTimeScale()
    {
        return currentSavefileData.normalTimeScale;
    }

    public void SetTimeScale(float newValue)
    {
        currentSavefileData.normalTimeScale = newValue;
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
            Time.timeScale = currentSavefileData.normalTimeScale;

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
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined; //we need to controll curson with a pause menu once implimented
            OnLockMouse?.Invoke(true);
        }
        else if (wasLocked && thingsLockingMouse == 0)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            OnLockMouse?.Invoke(false);
        }
    }

    #endregion

    #region Sound
    public float GetMasterVolume()
    {
        return globalSettings.masterVolume;
    }

    public float GetEffectsVolume()
    {
        return globalSettings.effectsVolume;
    }

    public float GetMusicVolume()
    {
        return globalSettings.musicVolume;
    }

    public void UpdateVolumes()
    {
        UpdateMixerEffectsVolume();
        UpdateMixerMasterVolume();
        UpdateMixerMusicVolume();
    }

    public void SetMasterVolume(float newValue)
    {
        globalSettings.masterVolume = newValue;
        UpdateMixerMasterVolume();
    }

    private void UpdateMixerMasterVolume()
    {
        if(AudioManager.IsValid)
        {
            AudioManager.Instance.SetVolume(MasterSoundName, globalSettings.masterVolume / 100f);
        }
    }

    public void SetEffectsVolume(float newValue)
    {
        globalSettings.effectsVolume = newValue;
        UpdateMixerEffectsVolume();
    }

    private void UpdateMixerEffectsVolume()
    {
        if (AudioManager.IsValid)
        {
            AudioManager.Instance.SetVolume(SoundEffectsSoundName, globalSettings.effectsVolume / 100f);
        }
    }

    public void SetMusicVolume(float newValue)
    {
        globalSettings.musicVolume = newValue;
        UpdateMixerMusicVolume();
    }

    public void UpdateMixerMusicVolume()
    {
        if (AudioManager.IsValid)
        {
            AudioManager.Instance.SetVolume(MusicSoundName, globalSettings.musicVolume / 100f);
        }
    }
    #endregion

    #region  SavefilesStuff
    private SavefileData CopySaveFileData(SavefileData dataToBeCopied)
    {
        SavefileData newData = new SavefileData();

        newData.sceneNr = dataToBeCopied.sceneNr;
        newData.savePos = dataToBeCopied.savePos;

        newData.normalTimeScale = dataToBeCopied.normalTimeScale;

        return newData;
    }

    public void LoadLastSaveFile()
    {
        PlaySaveFile(globalSettings.lastSaveFileNr);
    }

    public void PlaySaveFile(int saveFileNr)
    {
        currentSavefileNr = saveFileNr;
        globalSettings.lastSaveFileNr = currentSavefileNr;

        if (!Directory.Exists(Application.dataPath + "/Savefiles/" + currentSavefileNr))
        {
            CreateSaveFileDirectory(currentSavefileNr);
        }
        ReadSavefileSettings();

        Time.timeScale = currentSavefileData.normalTimeScale;
        if(OnLoadScene != null)
        {
            OnLoadScene(currentSavefileData.sceneNr);
        }
        SceneManager.LoadScene(currentSavefileData.sceneNr);
    }

    public void DeleteSaveFile(int saveFileNr)
    {
        if (File.Exists(Application.dataPath + "/Savefiles/" + saveFileNr + "/SavefileSettings.txt"))
        {
            File.Delete(Application.dataPath + "/Savefiles/" + saveFileNr + "/SavefileSettings.txt");
        }

        if (File.Exists(Application.dataPath + "/Savefiles/" + saveFileNr + "/SavefileSettings.txt.meta"))
        {
            File.Delete(Application.dataPath + "/Savefiles/" + saveFileNr + "/SavefileSettings.txt.meta");
        }
    }

    public void SavePointSave(Vector2 saveLocation)
    {
        currentSavefileData.savePos = saveLocation;
        lastSavedSavefileData = CopySaveFileData(currentSavefileData);
        SaveSavefile();
        if(OnSavePointSaveSelfReset != null)
        {
            OnSavePointSaveSelfReset();
        }
    }

    public void ResetSave()
    {
        currentSavefileData = CopySaveFileData(lastSavedSavefileData);

        if(OnLoadScene != null)
        {
            OnLoadScene(currentSavefileData.sceneNr);
        }
        SceneManager.LoadScene(currentSavefileData.sceneNr);
    }

    public void SaveSettings()
    {
        //since this doesent touch currentSavefileData it only saves lastSavedSavefileData, meaning the only thing it saves is the stuff directly editing it and global
        //things directly edeting last save file should be save file specific settings, such as difficulty and cheats.
        //due to this it can be used very liberally
        SaveGlobalOptions();
        SaveSavefile();
    }

    void CreateSaveFileDirectory(int numberValue)
    {
        Directory.CreateDirectory(Application.dataPath + "/Savefiles/" + numberValue);
    }

    void SaveGlobalOptions()
    {
        string json = JsonUtility.ToJson(globalSettings);

        File.WriteAllText(Application.dataPath + "/Savefiles/" + "/GlobalSettings.txt", json);
    }

    void CreateDefaultGlobalOptions()
    {
        string json = JsonUtility.ToJson(new GlobalSettings());

        File.WriteAllText(Application.dataPath + "/Savefiles/" + "/GlobalSettings.txt", json);
    }

    void SaveSavefile()
    {
        string json = JsonUtility.ToJson(lastSavedSavefileData);

        File.WriteAllText(Application.dataPath + "/Savefiles/" + currentSavefileNr + "/SavefileSettings.txt", json);
    }

    void CreateDefaultSaveFileSettings()
    {
        string json = JsonUtility.ToJson(new SavefileData());

        File.WriteAllText(Application.dataPath + "/Savefiles/" + currentSavefileNr + "/SavefileSettings.txt", json);
    }

    void ReadGlobalSettings()
    {
        if (!File.Exists(Application.dataPath + "/Savefiles/" + "/GlobalSettings.txt"))
        {
            if (!Directory.Exists(Application.dataPath + "/Savefiles/"))
            {
                CreateSaveFileDirectory(1);
            }
            CreateDefaultGlobalOptions();
        }
        string json = File.ReadAllText(Application.dataPath + "/Savefiles/" + "/GlobalSettings.txt");

        globalSettings = JsonUtility.FromJson<GlobalSettings>(json);
        hasLoadedSettings = true;
    }

    void ReadSavefileSettings()
    {
        if (!File.Exists(Application.dataPath + "/Savefiles/" + currentSavefileNr + "/SavefileSettings.txt"))
        {
            CreateDefaultSaveFileSettings();
        }
        string json = File.ReadAllText(Application.dataPath + "/Savefiles/" + currentSavefileNr + "/SavefileSettings.txt");

        currentSavefileData = JsonUtility.FromJson<SavefileData>(json);
    }
    #endregion

    #region GetOtherSavefileStuff
    public Vector2 GetCurrentPlayerSavePos()
    {
        return currentSavefileData.savePos;
    }

    public bool GetConflictingControllsNeutralizes()
    {
        return globalSettings.conflictingControllsNeutralizes;
    }
    #endregion
}