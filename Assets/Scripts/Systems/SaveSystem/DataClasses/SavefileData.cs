using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

[Serializable]
public class SavefileData
{
    public const string UnInitializedSceneName = "UNINITIALIZED";

    public string SceneName; //where player is
    public SavefileSettings Settings;

    public int PlayerGold;
    public List<string> DonatedWeapons;

    public DungeonSaveData? Dungeon;
    
    public List<DialogueSaveData> DialogueSaves;
    /// <summary>
    /// gets set to true in world data creator
    /// </summary>
    public bool DungeonIsInitialized = false;
    public PlayerSaveData? PlayerSaveData;

    public SavefileData(SavefileSettings settings)
    {
        SceneName = UnInitializedSceneName;
        DonatedWeapons = new();
        Settings = settings;
        Dungeon = null;
        PlayerGold = 30;
        DialogueSaves = new();
    }

    public SavefileData(string sceneName, int playerGold, List<string> donatedWeapons, SavefileSettings settings, DungeonSaveData? dungeon, PlayerSaveData? playerSaveData, List<DialogueSaveData> dialogueSaves)
    {
        SceneName = sceneName;
        DonatedWeapons = donatedWeapons;
        PlayerGold = playerGold;
        Settings = settings;
        Dungeon = dungeon;
        PlayerSaveData = playerSaveData;
        DialogueSaves = dialogueSaves;
    }

    public SavefileData Clone()
    {
        return new SavefileData(SceneName, PlayerGold, DonatedWeapons, Settings.Clone(), Dungeon?.Clone(), PlayerSaveData?.Clone(), DialogueSaves);
    }
}
