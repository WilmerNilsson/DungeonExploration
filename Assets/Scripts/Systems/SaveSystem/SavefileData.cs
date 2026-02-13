using System;
using System.Collections.Generic;
using UnityEngine;

public class SavefileData
{
    public int sceneNr = 1; //where player is

    public List<int> DialougesGotten = new();
    public List<string> BooksInJournal = new();

    public SavefileSettings settings = new();
    public PlayerSaveData PlayerSaveData = new();
    public DungeonSaveData DungeonSaveData = new();

    public SavefileData Clone()
    {
        return new SavefileData()
        {
            sceneNr = sceneNr,

            DialougesGotten = new(DialougesGotten),
            BooksInJournal = new(BooksInJournal),

            settings = settings.Clone(),
            PlayerSaveData = PlayerSaveData.Clone(),
            DungeonSaveData = DungeonSaveData.Clone(),
        };
    }
}
