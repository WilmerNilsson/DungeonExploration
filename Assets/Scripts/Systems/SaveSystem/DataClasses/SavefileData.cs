using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

[Serializable]
public class SavefileData
{
    public string SceneName; //where player is
    public SavefileSettings Settings;

    public int PlayerGold;
    public List<string> DonatedWeapons;

    public DungeonSaveData? Dungeon;
    /// <summary>
    /// gets set to true in world data creator
    /// </summary>
    public bool DungeonIsInitialized = false;
    public PlayerSaveData? PlayerSaveData;

    public SavefileData(SavefileSettings settings)
    {
        SceneName = "JSaveFileTest";
        DonatedWeapons = new();
        Settings = settings;
        Dungeon = null;
    }

    public SavefileData(string sceneName, int playerGold, List<string> donatedWeapons, SavefileSettings settings, DungeonSaveData? dungeon, PlayerSaveData? playerSaveData)
    {
        SceneName = sceneName;
        DonatedWeapons = donatedWeapons;
        PlayerGold = playerGold;
        Settings = settings;
        Dungeon = dungeon;
        PlayerSaveData = playerSaveData;
    }

    public SavefileData Clone()
    {
        return new SavefileData(SceneName, PlayerGold, DonatedWeapons, Settings.Clone(), Dungeon?.Clone(), PlayerSaveData?.Clone());
    }
}
