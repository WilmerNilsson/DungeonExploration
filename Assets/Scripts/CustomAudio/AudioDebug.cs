using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class AudioDebug : MonoBehaviour
{
    public enum Procedures
    {
        GetGlobalParameterList,
        GetInstanceList,
        GetLocalParameterList,
        GetAllInstances,
        GetAllVcas,
        GetLoadedBanks,
        SeePerformanceMetrics
    }

    public string path;
    public Procedures procedure;
    public string text;

    public void Execute(out string result, out int lines)
    {
        if (!AudioManager.IsValid)
        {
            result = "";
            lines = 0;
            return;
        }

        text = "";
        lines = 0;
        switch (procedure)
        {
            case Procedures.GetGlobalParameterList:
                foreach (var param in AudioManager.Instance.GlobalParameterCache)
                {
                    lines++;
                    RuntimeManager.StudioSystem.getParameterByID(param.Value, out var value);
                    text += param.Key + ": " + value + "\n";
                }
                result = text;
                return;
            case Procedures.GetInstanceList:
                if (AudioManager.Instance.TryGetEventList(path, out var list, out var eventName))
                {
                    if (list.TryGetEvent(eventName, out var eventData))
                    {
                        if (eventData.isOneShot)
                        {
                            lines = 0;
                            result = "";
                            return;
                        }
                        var eventDesc = RuntimeManager.GetEventDescription(eventData.eventReference);
                        if (!eventDesc.isValid())
                        {
                            Debug.Log("Description IS NOT VALID");
                            lines = 0;
                            result = "";
                            return;
                        }
                        eventDesc.getInstanceList(out var instanceList);
                        text = eventName + " has " + instanceList.Length + " instance(s) \n";
                        lines = 1;
                        var objectList = new List<GameObject>();
                        foreach (var instance in instanceList)
                        {
                            foreach (var kvp in list.InstanceList)
                            {
                                if (instance.Equals(kvp.Value))
                                {
                                    objectList.Add(kvp.Key);
                                }
                            }
                        }

                        if (objectList.Count > 0)
                        {
                            lines++;
                            text += objectList.Count + " of which are on these objects:\n";
                            foreach (var obj in objectList)
                            {
                                lines++;
                                text += obj.name + "\n";
                            }
                        }
                       
                        result = text;
                        return;
                    }
                }
                break;
            case Procedures.GetLocalParameterList:
                result = "This has not been implemented yet";
                lines = 1;
                return;
            case Procedures.GetAllInstances:
                result = "This has not been implemented yet";
                lines = 1;
                return;
            case Procedures.GetAllVcas:
                foreach (var VCA in AudioManager.Instance.VcaCache)
                {
                    lines++;
                    VCA.Value.getVolume(out var volume);
                    text += VCA.Key + ": " + volume + "\n";
                }
                result = text;
                return;
            case Procedures.GetLoadedBanks:
                RuntimeManager.StudioSystem.getBankList(out var banks);
                foreach (var bank in banks)
                {
                    lines += 3;
                    bank.getPath(out var bankPath);
                    bank.getLoadingState(out var loadingState);
                    bank.getSampleLoadingState(out var sampleLoadingState);
                    var split = bankPath.Split('/');
                    text += split[^1] + ": \n Loading state: " + loadingState  + "\n Sample loading state: " + sampleLoadingState + "\n \n";
                }
                result = text;
                return;
            case Procedures.SeePerformanceMetrics:
                RuntimeManager.StudioSystem.getMemoryUsage(out var memory);
                
                text += "Memory Metrics: (Doesn't include non-streaming sample data)" +
                        "\n Exclusive: " + memory.exclusive/1000000f + " MB" + 
                        "\n Inclusive: " + memory.inclusive/1000000f + " MB" + 
                        "\n Sample Data: " + memory.sampledata/1000000f + " MB" + "\n \n";
                
                RuntimeManager.StudioSystem.getCPUUsage(out var cpu, out var core);
                text += "CPU Metrics: " +
                        "\n DSP: " + core.dsp + 
                        "\n Stream: " + core.stream + 
                        "\n Geometry: " + core.geometry + 
                        "\n Update: " + core.update;
                result = text;
                lines = 10;
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }

        result = "";
        lines = 0;
    }
    
    /*
     public string[] GetGlobalParameterList(out float[] valueList)
    {
        var tempList = new List<string>();
        var tempValueList = new List<float>();
        foreach (var parameter in _globalParameterCache)
        {
            tempList.Add(parameter.Key);
            tempValueList.Add(GetGlobalParameterValue(parameter.Key));
        }
        valueList = tempValueList.ToArray();
        return tempList.ToArray();
    }

    public float GetGlobalParameterValue(string paramName)
    {
        if (_globalParameterCache.TryGetValue(paramName, out var id))
        {
            RuntimeManager.StudioSystem.getParameterByID(id, out var paramValue);
            return paramValue;
        }

        return 0f;
    }

    public bool TryGetEventDescription(string path, out EventDescription eventDescription)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.TryGetEvent(eventName, out var eventData);
            eventDescription = RuntimeManager.GetEventDescription(eventData.eventReference);
            return true;
        }

        eventDescription = new EventDescription();
        return false;
    }

    public bool TryGetLocalParameterList(string path, out ParameterData[] parameterList)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.TryGetEvent(eventName, out var eventData);
            parameterList = eventData.parameters;
            return true;
        }
        parameterList = null;
        return false;
    }

    public string[] GetEventInstanceList(string category)
    {
        var tempList = new List<string>();
        var path = category + "/x";
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            foreach (var instance in eventList.InstanceList)
            {
                instance.Value.getDescription(out var description);
                description.getPath(out var eventPath);
                var split = eventPath.Split('/');
                
                tempList.Add(split[^1] + ": " + instance.Key.name);
            }
        }
        return tempList.ToArray();
    }

    public bool TryEventData (string path, out EventData eventData)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.TryGetEvent(eventName, out eventData);
            return true;
        }
        eventData = null;
        return false;
    }
     *text = "";
                lines = 0;
                switch (proceduresProperty.enumValueIndex)
                {
                    case 0:
                        var strings = AudioManager.Instance.GetGlobalParameterList(out var values);
                        for (int i = 0; i < strings.Length; i++)
                        {
                            lines++;
                            text += strings[i] + ": " + values[i] + "\n";
                        }
                        break;
                    case 1:
                        var instances = AudioManager.Instance.GetEventInstanceList(pathProperty.stringValue);
                        lines = 1;
                        text = pathProperty.stringValue + " has Instances on these objects:" + "\n";
                        foreach (var instance in instances)
                        {
                            lines++;
                            text += instance + "\n";
                        }
                        break;
                    case 2:
                        if (AudioManager.Instance.TryEventData(pathProperty.stringValue, out var eventData))
                        {
                            text = eventData.eventName + " has these local parameters:";
                            
                        }
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                }
     * 
     */
}
