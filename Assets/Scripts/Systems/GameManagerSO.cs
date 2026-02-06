using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GlobalSettings
{
    //savefile
    public int lastSaveFileNr;

    //Volume
    public float masterVolume = 50f; public float effectsVolume = 100f; public float musicVolume = 100f;

    //Languige is taken care of automaticly apparently
    //Gameplay
    public bool conflictingControllsNeutralizes = false;
}

public class SavefileData
{
    public int sceneNr = 1;
    public Vector2 savePos = new Vector2(0, 0);

    //settings
    public float normalTimeScale = 1f;
    public float playerHealthCheatValue = 1f;
    public float enemyHealthCheatValue = 1f;

    //unlocks
    public List<int> abilities = new List<int>();
    public List<int> cutscenes = new List<int>();

    //stats
    public List<int> hpUps = new List<int>();
}

[CreateAssetMenu(fileName = "GameManagerSO", menuName = "Scriptable Objects/GameManagerSO")]
public class GameManagerSO : ScriptableObject
{
    [SerializeField] AudioMixer mixer;

    GlobalSettings globalSettings = new GlobalSettings();
    SavefileData currentSavefileData = new SavefileData();
    SavefileData lastSavedSavefileData = new SavefileData();
    int currentSavefileNr = 1;

    private static GameManagerSO instance;
    bool hasLoadedSettings = false;

    int _thingsFreezingGame = 0;

    public event Action<IDListName, int> OnIDAddedToListSelfReset;
    public event Action<float> OnPlayerHealthCheatValueChangeSelfReset;
    public event Action<float> OnEnemyHealthCheatValueChangeSelfReset;
    public event Action OnSavePointSaveSelfReset;
    public event Action<bool> OnFreezeTimeChangeSelfReset;
    public event Action<int> OnLoadScene;

    void FirstAcces()
    {
        if(!hasLoadedSettings)
        {
            //updating mixer volume is done in the music controller script since i can't make sure that the update mixer methods are called after the mixer loads
            //put things here instad of OnEnable etc

            instance.ReadGlobalSettings();
            hasLoadedSettings = true;
        }
    }

    public static GameManagerSO GetGameManagerSOInstance()
    {
        if(instance == null)
        {
            instance = Resources.LoadAll<GameManagerSO>("")[0];
        }
        instance.FirstAcces();
        return instance;
    }

    //called on SingletonSpawner awake
    public void ResetManagerVariables()
    {
        _thingsFreezingGame = 0;
        hasLoadedSettings = false;
    }

    /* #region  moveToSceneStuff */
    public void MoveToScene(Vector2 newLocation, int newSceneNr)
    {
        currentSavefileData.sceneNr = newSceneNr;
        currentSavefileData.savePos = newLocation;

        if(newSceneNr != SceneManager.GetActiveScene().buildIndex)
        {
            ResetActions();

            if(newSceneNr == 0)
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

    void ResetActions()
    {
        OnIDAddedToListSelfReset = null;
        OnPlayerHealthCheatValueChangeSelfReset = null;
        OnEnemyHealthCheatValueChangeSelfReset = null;
        OnSavePointSaveSelfReset = null;
        OnFreezeTimeChangeSelfReset = null;
    }
    /* #endregion */

    /* #region  SaveFile ID stuff */
    public enum IDListName
    {
        abilities,
        cutscenes,
        hpUps
    }

    public void AddIDToList(IDListName type, int id)
    {
        if(type == IDListName.abilities)
        {
            if(!currentSavefileData.abilities.Contains(id))
            {
                currentSavefileData.abilities.Add(id);
            }
        }
        else if(type == IDListName.cutscenes)
        {
            if(!currentSavefileData.cutscenes.Contains(id))
            {
                currentSavefileData.cutscenes.Add(id);
            }
        }
        else if(type == IDListName.hpUps)
        {
            if(!currentSavefileData.cutscenes.Contains(id))
            {
                currentSavefileData.hpUps.Add(id);
            }
        }

        if(OnIDAddedToListSelfReset != null)
        {
            OnIDAddedToListSelfReset(type, id);
        }
    }

    public bool CheckIfIDExists(IDListName type, int id)
    {
        if(type == IDListName.abilities)
        {
            return currentSavefileData.abilities.Contains(id);
        }
        else if(type == IDListName.cutscenes)
        {
            return currentSavefileData.cutscenes.Contains(id);
        }
        else if(type == IDListName.hpUps)
        {
            return currentSavefileData.hpUps.Contains(id);
        }
        else
        {
            return false;
        }
    }

    public int GetAmountOfIDsInSaveFile(IDListName type)
    {
        if(type == IDListName.abilities)
        {
            return currentSavefileData.abilities.Count;
        }
        else if(type == IDListName.cutscenes)
        {
            return currentSavefileData.cutscenes.Count;
        }
        else if(type == IDListName.hpUps)
        {
            return currentSavefileData.hpUps.Count;
        }
        else
        {
            return -1;
        }
    }
    /* #endregion */

    /* #region  GamePlayCheats */
    public void SetPlayerHealthCheatValue(float newValue)
    {
        currentSavefileData.playerHealthCheatValue = newValue;
        lastSavedSavefileData.playerHealthCheatValue = newValue;

        if (OnPlayerHealthCheatValueChangeSelfReset != null)
        {
            OnPlayerHealthCheatValueChangeSelfReset(newValue);
        }
    }

    public float GetPlayerHealthCheatValue()
    {
        return currentSavefileData.playerHealthCheatValue;
    }

    public void SetEnemyHealthCheatValue(float newValue)
    {
        currentSavefileData.enemyHealthCheatValue = newValue;
        lastSavedSavefileData.enemyHealthCheatValue = newValue;

        if (OnEnemyHealthCheatValueChangeSelfReset != null)
        {
            OnEnemyHealthCheatValueChangeSelfReset(newValue);
        }
    }

    public float GetEnemyHealthCheatValue()
    {
        return currentSavefileData.enemyHealthCheatValue;
    }
    /* #endregion */

    /* #region  TimeScale */
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
        bool wasFrozen = _thingsFreezingGame == 0 ? false : true;

        if(value)
        {
            _thingsFreezingGame++;
        }
        else
        {
            _thingsFreezingGame--;
        }

        if(!wasFrozen && _thingsFreezingGame != 0)
        {
            Time.timeScale = 0;

            if(OnFreezeTimeChangeSelfReset != null)
            {
                OnFreezeTimeChangeSelfReset(true);
            }
        }
        else if(wasFrozen && _thingsFreezingGame == 0)
        {
            Time.timeScale = currentSavefileData.normalTimeScale;

            if(OnFreezeTimeChangeSelfReset != null)
            {
                OnFreezeTimeChangeSelfReset(false);
            }
        }
    }
    /* #endregion */

    /* #region  Sound */
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

    void UpdateMixerMasterVolume()
    {
        if (globalSettings.masterVolume > 0)
        {
            mixer.SetFloat("MasterVol", Mathf.Log10(globalSettings.masterVolume / 100f) * 20f);
        }
        else
        {
            mixer.SetFloat("MasterVol", -80f);
        }
    }

    public void SetEffectsVolume(float newValue)
    {
        globalSettings.effectsVolume = newValue;
        UpdateMixerEffectsVolume();
    }

    void UpdateMixerEffectsVolume()
    {
        if (globalSettings.effectsVolume > 0)
        {
            mixer.SetFloat("EffectsVol", Mathf.Log10(globalSettings.effectsVolume / 100f) * 20f);
        }
        else
        {
            mixer.SetFloat("EffectsVol", -80f);
        }
    }

    public void SetMusicVolume(float newValue)
    {
        globalSettings.musicVolume = newValue;
        UpdateMixerMusicVolume();
    }

    void UpdateMixerMusicVolume()
    {
        if (globalSettings.musicVolume > 0)
        {
            mixer.SetFloat("MusicVol", Mathf.Log10(globalSettings.musicVolume / 100f) * 20f);
        }
        else
        {
            mixer.SetFloat("MusicVol", -80f);
        }
    }
    /* #endregion */

    /* #region  SavefilesStuff */
    private SavefileData CopySaveFileData(SavefileData dataToBeCopied)
    {
        SavefileData newData = new SavefileData();

        newData.sceneNr = dataToBeCopied.sceneNr;
        newData.savePos = dataToBeCopied.savePos;

        newData.normalTimeScale = dataToBeCopied.normalTimeScale;
        newData.playerHealthCheatValue = dataToBeCopied.playerHealthCheatValue;
        newData.enemyHealthCheatValue = dataToBeCopied.enemyHealthCheatValue;

        newData.abilities = new List<int>(dataToBeCopied.abilities);
        newData.cutscenes = new List<int>(dataToBeCopied.cutscenes);
        newData.hpUps = new List<int>(dataToBeCopied.hpUps);

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
    /* #endregion */

    /* #region  GetOtherSavefileStuff */
    public Vector2 GetCurrentPlayerSavePos()
    {
        return currentSavefileData.savePos;
    }

    public bool GetConflictingControllsNeutralizes()
    {
        return globalSettings.conflictingControllsNeutralizes;
    }
    /* #endregion */
}