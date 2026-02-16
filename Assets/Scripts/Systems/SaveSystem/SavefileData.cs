using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class SavefileData
{
    public int SceneNr; //where player is

    public List<int> DialougesGotten;
    public List<string> BooksInJournal;

    public SavefileSettings Settings;

    public WorldData? World;

    public SavefileData(SavefileSettings settings)
    {
        SceneNr = 0;
        DialougesGotten = new();
        BooksInJournal = new();
        Settings = settings;
        World = null;
    }

    public SavefileData(int sceneNr, List<int> dialougesGotten, List<string> booksInJournal, SavefileSettings settings, WorldData world)
    {
        SceneNr = sceneNr;
        DialougesGotten = dialougesGotten;
        BooksInJournal = booksInJournal;
        Settings = settings;
        World = world;
    }

    public SavefileData Clone()
    {
        return new SavefileData(SceneNr, new(DialougesGotten), new(BooksInJournal), Settings.Clone(), World.Clone());
    }

    public class WorldData
    {
        public PlayerSaveData PlayerSaveData;
        public DungeonSaveData DungeonSaveData;

        public WorldData(PlayerSaveData playerSaveData, DungeonSaveData dungeonSaveData)
        {
            PlayerSaveData = playerSaveData;
            DungeonSaveData = dungeonSaveData;
        }

        public WorldData Clone()
        {
            return new WorldData(PlayerSaveData.Clone(), DungeonSaveData.Clone());
        }
    }
}
