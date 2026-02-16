using System;
using System.Collections.Generic;
using UnityEngine;

public class SavefileData
{
    public int SceneNr; //where player is

    public List<int> DialougesGotten;
    public List<string> BooksInJournal;

    public SavefileSettings Settings;

    public WorldData World;

    public SavefileData(int sceneNr, List<int> dialougesGotten, List<string> booksInJournal, SavefileSettings settings, WorldData world)
    {
        SceneNr = sceneNr;
        DialougesGotten = dialougesGotten;
        BooksInJournal = booksInJournal;
        Settings = settings;
        World = world;
    }

    public SavefileData(SavefileSettings settings)
    {
        Settings = settings;

        SceneNr = 0;
        DialougesGotten = new();
        BooksInJournal = new();
        World = new();
    }

    public SavefileData Clone()
    {
        return new SavefileData(SceneNr, new(DialougesGotten), new(BooksInJournal), Settings.Clone(), World.Clone());
    }

    public class WorldData
    {
        public PlayerSaveData PlayerSaveData = new();
        public DungeonSaveData DungeonSaveData = new();

        public WorldData Clone()
        {
            return new WorldData()
            {
                PlayerSaveData = PlayerSaveData.Clone(),
                DungeonSaveData = DungeonSaveData.Clone()
            };
        }
    }
}
