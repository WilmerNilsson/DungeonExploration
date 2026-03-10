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
        GetAllVcas,
        GetLoadedBanks,
        SeePerformanceMetrics
    }

    public static void Print(string message, bool isWarning = false)
    {
        if (!AudioManager.IsValid) return;
        if (!AudioManager.Instance.debug) return;
        if (isWarning)
        {
            Debug.LogWarning(message);
        }
        else if (!AudioManager.Instance.showOnlyWarnings)
        {
            Debug.Log(message);
        }
    }

    public bool executeInUpdate;
    public string path;
    public Procedures procedure;
    public string text;
    public int lines;

    public void Execute()
    {
        if (!AudioManager.IsValid)
        {
            text = "";
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
                return;
            case Procedures.GetInstanceList:
                if (AudioManager.Instance.TryGetEventList(path, out var eventList, out var eventName))
                {
                    if (eventList.TryGetEvent(eventName, out var eventData))
                    {
                        if (eventData.isOneShot)
                        {
                            lines = 1;
                            text = "Cannot fetch instances since the event is not a Looping Event";
                            return;
                        }
                        var eventDesc = RuntimeManager.GetEventDescription(eventData.eventReference);
                        if (!eventDesc.isValid())
                        {
                            lines = 1;
                            text = "Event Description is not Valid";
                            return;
                        }
                        eventDesc.getInstanceList(out var instanceList);
                        text = eventName + " has " + instanceList.Length + " instance(s) \n";
                        lines = 1;
                        var objectList = new List<GameObject>();
                        foreach (var instance in instanceList)
                        {
                            foreach (var kvp in eventList.InstanceList)
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
                                if (obj == null)
                                {
                                    text += "NULL\n";
                                }
                                else
                                {
                                    text += obj.name + "\n";
                                }
                            }
                        }
                        return;
                    }
                }
                text = "Couldn't find Event";
                lines = 1;
                return;
            case Procedures.GetLocalParameterList:
                if (AudioManager.Instance.TryGetEventList(path, out var list, out var eName))
                {
                    if (list.TryGetEvent(eName, out var eventData))
                    {
                        if (eventData.isOneShot)
                        {
                            lines = 1;
                            text = "Cannot fetch instances since the event is not a Looping Event";
                            return;
                        }
                        var eventDesc = RuntimeManager.GetEventDescription(eventData.eventReference);
                        if (!eventDesc.isValid())
                        {
                            lines = 1;
                            text = "Event Description is not Valid";
                            return;
                        }
                        eventDesc.getInstanceList(out var instanceList);
                        if (eventData.EventInstance.isValid())
                        {
                            lines++;
                            text += "The instance in EventData has these parameters: \n";
                            foreach (var paramData in eventData.parameters)
                            {
                                lines++;
                                eventData.EventInstance.getParameterByID(paramData.ID(), out var value);
                                text += paramData.paramName + ": " + value + "\n";
                            }

                            text += "\n";

                            lines++;
                        }

                        foreach (var kvp in list.InstanceList)
                        {
                            lines++;
                            text += "The instance on " + kvp.Key.name + " has these parameters: \n";
                            foreach (var paramData in eventData.parameters)
                            {
                                lines++;
                                kvp.Value.getParameterByID(paramData.ID(), out var value);
                                text += paramData.paramName + ": " + value + "\n";
                            }

                            text += "\n";

                            lines++;
                        }
                        
                        return;
                    }
                }
                text = "Couldn't find Event";
                lines = 1;
                return;
            case Procedures.GetAllVcas:
                foreach (var vca in AudioManager.Instance.VcaCache)
                {
                    lines++;
                    vca.Value.getVolume(out var volume);
                    text += vca.Key + ": " + volume + "\n";
                }
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
                lines = 10;
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Update()
    {
        if (executeInUpdate)
        {
            Execute();
        }
    }
}
