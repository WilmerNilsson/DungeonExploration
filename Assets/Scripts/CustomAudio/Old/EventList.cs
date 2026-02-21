using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "EventList", menuName = "Scriptable Objects/EventList")]
public class EventList : ScriptableObject
{
    public string category;
    public EventData[] events;
    public bool debug;
    
    #region EventData
    
    public Dictionary<string, EventData> EventCache = new Dictionary<string, EventData>();

    public void RefreshEventCache() //Lägger till alla eventData till _eventCache för snabbare lookup än foreach loop i eventData
                                    //samt refreshar ParameterCache i alla eventData
    {
        EventCache = new Dictionary<string, EventData>();
        foreach (var eventData in events)
        {
            eventData.RefreshParameterCache();
            EventCache.Add(eventData.eventName, eventData);

            AudioDebug.Print("Added " + eventData.eventName + " to eventCache");
        }
    }
    
    #if UNITY_EDITOR
    [ContextMenu("Fill eventData")]
    public void FillEventData() //Kallar populateData i alla eventData samt sparar objektet
    {
        foreach (var eventData in events)
        {
            eventData.PopulateData();
        }
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssetIfDirty(this);
    }
    #endif
    
    public Dictionary<GameObject, EventInstance> InstanceList = new Dictionary<GameObject, EventInstance>();
    public Dictionary<EventInstance, EventData> InstanceToEventData = new Dictionary<EventInstance, EventData>();
    
    public void CleanupInstanceList() //Kallas av audioManager vid scenladdning, stoppar alla event på gameObjects som inte längre finns
    {
        AudioDebug.Print(category + " has " + InstanceList.Count + " instance(s) in list before cleanup");
        var objList = InstanceList.Select(kvp => kvp.Key).ToList(); 
        foreach (var obj in objList)
        {
            if (obj) continue;
            InstanceList[obj].stop(STOP_MODE.IMMEDIATE);
            InstanceList[obj].release();
            InstanceToEventData.Remove(InstanceList.Single(kvp => kvp.Key == obj).Value);
            InstanceList.Remove(obj);
            AudioDebug.Print("Stopped and released eventInstance at " + obj);
        }
        AudioDebug.Print(category + " has " + InstanceList.Count + " instance(s) in list after cleanup");
    }
    
    public void StopAndReleaseAllInstances()
    {
        foreach (var eventData in events)
        {
            var eventDesc = RuntimeManager.GetEventDescription(eventData.eventReference);
            eventDesc.getInstanceList(out var instances);
            foreach (var instance in instances)
            {
                instance.stop(STOP_MODE.IMMEDIATE);
                instance.release();
            }
        }
    }
    
    #endregion

    
    [ContextMenu("Toggle Debug")]
    public void ToggleDebug()
    {
        debug = !debug;
        foreach (var eventData in events)
        {
            eventData.SetDebug(debug);
        }
    }
}
