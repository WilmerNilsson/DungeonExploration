using System;
using FMOD.Studio;
using UnityEngine;

[Serializable]
public class Instruction //Används av audioTrigger för att skicka instruktioner till AudioManager
{
    public enum Command
    {
        CreateInstance, //0
        ReleaseInstance,
        StartEvent,
        StopEvent,
        SetParameter,
        SetGlobalParameter, //5
        KeyOff,
        PlayOneShot,
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
}

[Serializable]
public struct ParameterToSet
{
    public string name;
    public float value;
}