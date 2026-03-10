using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable

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
    public SavefileSettings? SavefileSettings;
    public int CurrentSavefileNr = 1;

    public SaveFileManager()
    {
        GlobalSettings = ReadGlobalSettings();

        GlobalSettings ReadGlobalSettings()
        {
            if (File.Exists(Application.dataPath + SaveFileFolderName + GlobalSettingsName))
            {
                string json1 = File.ReadAllText(Application.dataPath + SaveFileFolderName + GlobalSettingsName);
                GlobalSettings? settings = JsonUtility.FromJson<GlobalSettings>(json1);

                if (settings == null)
                {
                    Debug.Log("global settings file compromized, creating a new one");
                    return CreateDefaultGlobalOptions();
                }
                else
                {
                    return settings;
                }
            }
            else if(Directory.Exists(Application.dataPath + SaveFileFolderName))
            {
                return CreateDefaultGlobalOptions();
            }
            else
            {
                CreateSaveFileDirectory();
                return CreateDefaultGlobalOptions();
            }

            GlobalSettings CreateDefaultGlobalOptions()
            {
                GlobalSettings defaultSettings = new GlobalSettings();
                string json = JsonUtility.ToJson(defaultSettings);
                File.WriteAllText(Application.dataPath + SaveFileFolderName + GlobalSettingsName, json);
                return defaultSettings;
            }
        }
    }

    public void LoadLastSaveFile()
    {
        PlaySavefile(GlobalSettings.LastSaveFileNr);
    }

    public void SaveSettings()
    {
        SaveGlobalOptions();
        SaveSavefileSettings();
    }

    public void PlaySavefile(int saveFileNr) //should also be a private one that takes savefile data
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
    #endregion

    #region Savefile

    /// <summary>
    /// makes a save file from world data and settings and then writes it to storage
    /// If there is a new scene then it also plays the save file after
    /// </summary>
    public void SaveInWorld(bool backup = false, string? newScene = null)
    {
        SavefileData data = ReadSavefile(CurrentSavefileNr); //we prob want to keep track of journals in real time aswell
        Debug.Log("reading save to get full data, need to split it up better");

        WorldDataCreator.CreateWorldData(out data.Dungeon, out data.PlayerSaveData);
        if (newScene != null)
        {
            data.SceneName = newScene;
            SaveSavefile(data, backup);
            PlaySavefile(CurrentSavefileNr);
        }
        else
        {
            data.SceneName = SceneManager.GetActiveScene().name;
            SaveSavefile(data, backup);
        }
    }

    /// <summary>
    /// makes a save file that keeps dungeon data and settings and then writes it to storage
    /// loading this save file will not move the player and reset thier health etc
    /// If there is a new scene then it also plays the save file after
    /// </summary>
    public void SaveFromTown(bool backup = false, string? newScene = null)
    {
        SavefileData data = ReadSavefile(CurrentSavefileNr); //we prob want to keep track of journals in real time aswell
        Debug.Log("reading save to get full data, need to split it up better");

#if DEBUG
        if(data.Dungeon == null)
        {
            Debug.LogError("World is null when saving from town");
            return;
        }
#endif
        TownDataCreator.TownData townData = TownDataCreator.GetTownData();

        if(data.PlayerSaveData == null)
        {
            data.PlayerSaveData = new(townData.Inventory, townData.Equipment, true, 1, 1, 1, 0);
            data.PlayerGold = townData.Cash;
        }
        else
        {
            data.PlayerSaveData.FromTown = true;
            data.PlayerGold = townData.Cash;
            data.PlayerSaveData.Inventory = townData.Inventory;
            data.PlayerSaveData.Equipment = townData.Equipment;
        }

        data.DonatedWeapons = townData.DonatedWeapons;

        if (newScene != null)
        {
            data.PlayerSaveData.RunCount++;
            data.SceneName = newScene;
            SaveSavefile(data, backup);
            PlaySavefile(CurrentSavefileNr);
        }
        else
        {
            data.SceneName = SceneManager.GetActiveScene().name;
            SaveSavefile(data, backup);
        }
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
        string json = JsonUtility.ToJson(data, true);

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
        if (SavefileSettings == null)
        {
            return;
        }

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
