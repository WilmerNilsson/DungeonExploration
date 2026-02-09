using System;
using FMOD.Studio;
using UnityEngine;

[Serializable]
public class Instruction //Används av audioTrigger för att skicka instruktioner till AudioManager
{
    public enum Command
    {
        CreateInstance, //0
        LoadSampleData,
        StartEvent,
        StopEvent, 
        ReleaseInstance, //4
        UnloadSampleData,
        SetParameter,
        KeyOff,
        PlayOneShot,
        SetGlobalParameter, //9
        LoadBank,
        UnloadBank,
        
    }
    
    public Command command; //Variabler för alla metoder som kan skickas till audioManager, i inspektorn visas bara relevanta variabler beroende på command
    public string path;
    public GameObject gameObj;
    public bool followObject = true;
    public STOP_MODE stopMode;
    public ParameterToSet[] parametersToSet;
    public string bankName;
    public bool loadSampleData;
}

[Serializable]
public struct ParameterToSet
{
    public string name;
    public float value;
}