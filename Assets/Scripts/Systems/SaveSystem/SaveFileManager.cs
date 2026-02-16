using System;
using System.IO;
using UnityEngine;

public class SaveFileManager
{
    private const string SaveFileFolderName = "/Savefiles/";
    private const string SaveFileName = "SaveData.txt";
    //we need to make a crash detection system (good save system)
    private const string SaveFileBackupName = "SaveDataBackup.txt"; 
    private const string GlobalSettingsName = "GlobalSettings.txt";

    // global settings is for things like audio and languige
    public GlobalSettings GlobalSettings;
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

        if (!Directory.Exists(Application.dataPath + SaveFileFolderName + CurrentSavefileNr))
        {
            CreateSaveFileDirectory(CurrentSavefileNr);
        }
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
            if (!Directory.Exists(Application.dataPath + SaveFileFolderName))
            {
                CreateSaveFileDirectory(1);
            }
            CreateDefaultGlobalOptions();
        }
        string json = File.ReadAllText(Application.dataPath + SaveFileFolderName + GlobalSettingsName);

        GlobalSettings = JsonUtility.FromJson<GlobalSettings>(json);
    }

    private void CreateDefaultGlobalOptions()
    {
        string json = JsonUtility.ToJson(new GlobalSettings());

        File.WriteAllText(Application.dataPath + SaveFileFolderName + GlobalSettingsName, json);
    }
    #endregion

    #region Savefile

    public void HardSave()
    {
        throw new NotImplementedException();
    }

    public void BackupSave()
    {
        throw new NotImplementedException();
    }

    private void CreateSaveFileDirectory(int numberValue)
    {
        Directory.CreateDirectory(Application.dataPath + SaveFileFolderName + numberValue);
    }

    private void SaveSavefile(SavefileData data)
    {
        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.dataPath + SaveFileFolderName + CurrentSavefileNr + SaveFileName, json);
    }

    private void SaveSavefileBackup(SavefileData data)
    {
        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.dataPath + SaveFileFolderName + CurrentSavefileNr + SaveFileBackupName, json);
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

    private SavefileData ReadSavefile(int saveFileNr)
    {
        if (!File.Exists(Application.dataPath + SaveFileFolderName + saveFileNr + SaveFileName))
        {
            CreateDefaultSavefile();
        }
        string json = File.ReadAllText(Application.dataPath + SaveFileFolderName + saveFileNr + SaveFileName);

        SavefileData data = JsonUtility.FromJson<SavefileData>(json);
        SavefileSettings = data.Settings;

        return data;
    }

    private void CreateDefaultSavefile()
    {
        SavefileSettings = new();
        SaveSavefile(new SavefileData(SavefileSettings));
        //since world is empty this will be fucked, need to fix down the line
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
