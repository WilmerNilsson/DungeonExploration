using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Debug = UnityEngine.Debug;

[Serializable]
public class EventData
{
    //Viktiga variabler (metadata) för event. eventName och eventReference bestäms i inspektorn. Resten av PopulateData().
    public string eventName;
    public EventReference eventReference;
    public string[] banks;
    public bool isOneShot;
    public bool is3D;
    public bool isOcclusion;
    public float minDistance;
    public float maxDistance;
    public bool debug;

    public EventInstance EventInstance;
    
    public ParameterData[] parameters;
    
    public Dictionary<string, ParameterData> ParameterCache; //För snabbare lookup i ParameterData istället för foreach loop

    public bool TryGetParamData(string paramName, out ParameterData paramData)
    {
        if (ParameterCache.TryGetValue(paramName, out paramData))
        {
            return true;
        }
        paramData = null;
        return false;
    }

    public void RefreshParameterCache()
    {
        ParameterCache = new Dictionary<string, ParameterData>();
        foreach (var parameterData in parameters)
        {
            ParameterCache.Add(parameterData.paramName, parameterData);
        }
    }

    #if UNITY_EDITOR
    public void PopulateData()
    {
        if (eventReference.IsNull)
        {
            Debug.LogWarning("Could not find event reference");
            return;
        }
        EditorUtils.LoadPreviewBanks(); //Behövs för att EventManager och EditorUtils ska fungera
        
        
        if (eventName == null | eventName == "") //Fyll eventName om den inte redan har ett namn
        {
            var split = eventReference.Path.Split('/');
            eventName = split[^1];
        }
        
        //EditorEventRefs är väldigt användbara eftersom de håller väldigt mycket metadata om ett event.
        //Eftersom de bara kan nås i Editorn behöver vi själva kopiera vissa variabler,
        //som vilka banker ett event tillhör samt vilka parametrar som används av eventet.
        
        var editorEventRef = EventManager.EventFromGUID(eventReference.Guid);

        if (editorEventRef == null)
        {
            Debug.LogWarning("Could not find editorEventRef");
            return;
        }

        banks = new string[editorEventRef.Banks.Count];
        for (int i = 0; i < banks.Length; i++)
        {
            banks[i] = editorEventRef.Banks[i].Name;
        }
        
        isOneShot = editorEventRef.IsOneShot;
        is3D = editorEventRef.Is3D;
        minDistance = editorEventRef.MinDistance;
        maxDistance = editorEventRef.MaxDistance;
        isOcclusion = false;
        //Fyll i parameters och checka om eventet behöver occlusionChecks
        parameters = new ParameterData[editorEventRef.Parameters.Count];
        for (var i = 0; i < editorEventRef.Parameters.Count; i++)
        {
            var tempParam = new ParameterData
            {
                paramName = editorEventRef.Parameters[i].Name,
                isGlobal = editorEventRef.Parameters[i].IsGlobal,
                data1 = editorEventRef.Parameters[i].ID.data1,
                data2 = editorEventRef.Parameters[i].ID.data2,
            };
            parameters[i] = tempParam;
            if (tempParam.paramName == "Occluded")
            {
                isOcclusion = true;
            }
        }
        EditorUtils.UnloadPreviewBanks();
    }
    #endif
    public void SetDebug(bool newValue) //Kallas av eventList, används för att gömma eller visa variabler i inspektorn
    {
        debug = newValue;
    }
}