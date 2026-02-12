using UnityEngine;

public class GlobalSettings
{
    //savefile
    public int lastSaveFileNr;

    //Volume
    public float masterVolume = 50f; public float effectsVolume = 100f; public float musicVolume = 100f;

    //Languige is taken care of automaticly apparently
    //Gameplay
    public bool conflictingControllsNeutralizes = false;
}