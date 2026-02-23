using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveFileManager
{
    private const string SaveFileFolderName = "/Savefiles/";
    private const string SaveFileName = "SaveData.txt";
    //we need to make a crash detection system (good save system)
    private const string SaveFileBackupName = "SaveDataBackup.txt"; 
    private const string GlobalSettingsName = "GlobalSettings.txt";

    // global settings is for things like audio and languige
    public GlobalSettings GlobalSettings { get; private set; }
    // Save file settings are stuff like accesability options and cheats
    public SavefileSettings SavefileSettings;
    public int CurrentSavefileNr = 1;

    public void LoadLastSaveFile()
    {
        PlaySavefile(GlobalSettings.LastSaveFileNr);
    }

    public void SaveSettings()
    {
        SaveGlobalOptions();
        SaveSavefileSettings();
    }

    public void PlaySavefile(int saveFileNr)
    {
        CurrentSavefileNr = saveFileNr;

        GlobalSettings.LastSaveFileNr = CurrentSavefileNr;

        SavefileData data = ReadSavefile(CurrentSavefileNr);

        GameManagerSO.Instance.LoadSavefileScene(data);
    }

    #region GlobalSettings
    public void SaveGlobalOptions()
    {
        string json = JsonUtility.ToJson(GlobalSettings);

        File.WriteAllText(Application.dataPath + SaveFileFolderName + GlobalSettingsName, json);
    }

    public void ReadGlobalSettings()
    {
        if (!File.Exists(Application.dataPath + SaveFileFolderName + GlobalSettingsName))
        {
            CreateSaveFileDirectory();
            CreateDefaultGlobalOptions();
        }
        string json = File.ReadAllText(Application.dataPath + SaveFileFolderName + GlobalSettingsName);
        GlobalSettings = JsonUtility.FromJson<GlobalSettings>(json);

        if (GlobalSettings == null)
        {
            Debug.Log("global settings file compromized, creating a new one");
            CreateDefaultGlobalOptions();
            json = File.ReadAllText(Application.dataPath + SaveFileFolderName + GlobalSettingsName);
            Debug.Log(json);
            GlobalSettings = JsonUtility.FromJson<GlobalSettings>(json);
        }

        void CreateDefaultGlobalOptions()
        {
            string json = JsonUtility.ToJson(new GlobalSettings());

            File.WriteAllText(Application.dataPath + SaveFileFolderName + GlobalSettingsName, json);
        }
    }
    #endregion

    #region Savefile

    /// <summary>
    /// makes a save file from world data and settings and then writes it to storage
    /// </summary>
    public void Save(bool backup = false)
    {
        SavefileData data = ReadSavefile(CurrentSavefileNr); //we prob want to keep track of journals in real time aswell
        Debug.Log("reading save to get full data, need to split it up better");

        data.World = WorldDataCreator.CreateWorldData();
        data.SceneName = SceneManager.GetActiveScene().name;

        SaveSavefile(data, backup);
    }

    private void CreateSaveFileDirectory()
    {
        if (!Directory.Exists(Application.dataPath + SaveFileFolderName))
        {
            Directory.CreateDirectory(Application.dataPath + SaveFileFolderName);
        }
    }


    /// <summary>
    /// writes data to storage
    /// </summary>
    private void SaveSavefile(SavefileData data, bool backup = false)
    {
        string json = JsonUtility.ToJson(data);

        Debug.Log(json);

        if(backup)
        {
            File.WriteAllText(Application.dataPath + SaveFileFolderName + CurrentSavefileNr + SaveFileBackupName, json);
        }
        else
        {
            File.WriteAllText(Application.dataPath + SaveFileFolderName + CurrentSavefileNr + SaveFileName, json);
        }
    }

    private void SaveSavefileSettings()
    {
#if DEBUG
        if (SavefileSettings == null)
        {
            Debug.LogError("trying to Save save file settings, but it is null");
        }
#endif

        // instead of reading the full json we could try and find just the settings
        // but i feel like the most work is just reading and writing att all.
        // if it is a problem just have the settings seperate in thier own file
        SavefileData data = ReadSavefile(CurrentSavefileNr); 
        data.Settings = SavefileSettings;
        SaveSavefile(data);
    }

    /// <summary>
    /// If save file does not exists it creates a new one
    /// </summary>
    private SavefileData ReadSavefile(int saveFileNr)
    {
        CreateSaveFileDirectory();
        if (!File.Exists(Application.dataPath + SaveFileFolderName + saveFileNr + SaveFileName))
        {
            CreateNullWorldSavefile();
        }
        string json = File.ReadAllText(Application.dataPath + SaveFileFolderName + saveFileNr + SaveFileName);

        SavefileData data = JsonUtility.FromJson<SavefileData>(json);

        if(data == null)
        {
            Debug.Log("save file compromized, creating new one");
            CreateNullWorldSavefile();

            json = File.ReadAllText(Application.dataPath + SaveFileFolderName + saveFileNr + SaveFileName);
            data = JsonUtility.FromJson<SavefileData>(json);
        }

        SavefileSettings = data.Settings;

        return data;

        void CreateNullWorldSavefile()
        {
            SavefileSettings = new();
            SaveSavefile(new SavefileData(SavefileSettings));
        }
    }

    public void DeleteSavefile(int saveFileNr)
    {
        if (File.Exists(Application.dataPath + SaveFileFolderName + saveFileNr + SaveFileName))
        {
            File.Delete(Application.dataPath + SaveFileFolderName + saveFileNr + SaveFileName);
        }

        if (File.Exists(Application.dataPath + SaveFileFolderName + saveFileNr + SaveFileName))
        {
            File.Delete(Application.dataPath + SaveFileFolderName + saveFileNr + SaveFileName);
        }
    }

    #endregion

}
