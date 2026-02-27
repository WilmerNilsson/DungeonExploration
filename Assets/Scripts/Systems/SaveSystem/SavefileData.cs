using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

[Serializable]
public class SavefileData
{
    public string SceneName; //where player is

    public List<int> DialougesGotten;
    public List<string> BooksInJournal;
    public int PlayerGold;

    public SavefileSettings Settings;

    public WorldData? World;

    public SavefileData(SavefileSettings settings)
    {
        SceneName = "JSaveFileTest";
        DialougesGotten = new();
        BooksInJournal = new();
        Settings = settings;
        World = null;
    }

    public SavefileData(string sceneName, List<int> dialougesGotten, List<string> booksInJournal, SavefileSettings settings, WorldData? world)
    {
        SceneName = sceneName;
        DialougesGotten = dialougesGotten;
        BooksInJournal = booksInJournal;
        Settings = settings;
        World = world;
    }

    public SavefileData Clone()
    {
        return new SavefileData(SceneName, new(DialougesGotten), new(BooksInJournal), Settings.Clone(), World?.Clone());
    }

    [Serializable]
    public class WorldData
    {
        /// <summary>
        /// gets set to true in world data creator
        /// </summary>
        public bool Initialized = false;
        public PlayerSaveData PlayerSaveData;
        public DungeonSaveData DungeonSaveData;

        public WorldData(PlayerSaveData playerSaveData, DungeonSaveData dungeonSaveData)
        {
            PlayerSaveData = playerSaveData;
            DungeonSaveData = dungeonSaveData;
        }

        private WorldData(PlayerSaveData playerSaveData, DungeonSaveData dungeonSaveData, bool initialized)
        {
            PlayerSaveData = playerSaveData;
            DungeonSaveData = dungeonSaveData;
            Initialized = initialized;
        }

        public WorldData Clone()
        {
            return new WorldData(PlayerSaveData.Clone(), DungeonSaveData.Clone(), Initialized);
        }
    }
}
