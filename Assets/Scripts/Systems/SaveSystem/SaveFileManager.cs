using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveFileManager
{
    // global settings is for things like audio and languige
    private GlobalSettings globalSettings = new GlobalSettings();
    // this is current save file data, if the player crashes this will be used.
    private SavefileData currentSavefileData = new SavefileData();
    //last actual save, when the player enters or exits the dungeon (or whenever design says)
    private SavefileData lastSavedSavefileData = new SavefileData();
    private int currentSavefileNr = 1;
}
