using System;
using System.Collections.Generic;
using UnityEngine;

public class SavefileData
{
    public int sceneNr = 1; //where player is
    public Vector3 savePos = new Vector3();

    public List<int> DialougesGotten = new();
    public List<string> BooksInJournal = new();

    //settings
    public float normalTimeScale = 1f; //good accesability option to have

    public PlayerSaveData PlayerSaveData = new();
    public DungeonSaveData DungeonSaveData = new();

    public SavefileData Clone()
    {
        return new SavefileData()
        {
            sceneNr = sceneNr,
            savePos = savePos,
            normalTimeScale = normalTimeScale,

            DialougesGotten = new(DialougesGotten),
            BooksInJournal = new(BooksInJournal),
        };
    }
}
