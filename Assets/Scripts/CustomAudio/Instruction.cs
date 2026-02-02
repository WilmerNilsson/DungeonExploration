using System;
using FMOD.Studio;
using UnityEngine;

[Serializable]
public class Instruction
{
    public enum Command
    {
        CreateInstance,
        ReleaseInstance,
        StartEvent,
        StopEvent,
        SetParameter,
        SetGlobalParameter,
        KeyOff,
        PlayOneShot,
        LoadBank,
        UnloadBank,
    }

    [Serializable]
    public struct ParameterToSet
    {
        public string paramName;
        public float paramValue;
    }
    
    public Command command;

    public string path;
    public GameObject gameObj;
    public bool attatchToObject;
    public bool followObject = true;
    public STOP_MODE stopMode;
    public ParameterToSet[] parametersToSet;
    public string bankName;
}
