using UnityEngine;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Debug = UnityEngine.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "EventList", menuName = "Scriptable Objects/EventList")]
public class EventList : ScriptableObject //TODO: Metoder, flytta cache till AudioManager? typ dictionary<path, eventData>?
{
    public string category;
    public EventData[] events;
    public bool debug;
    
    #region EventData
    
    private Dictionary<string, EventData> _eventCache = new Dictionary<string, EventData>();

    public void RefreshEventCache()
    {
        _eventCache = new Dictionary<string, EventData>();
        foreach (var eventData in events)
        {
            _eventCache.Add(eventData.eventName, eventData);

            if (debug)
            {
                Debug.Log("Added " + eventData.eventName + " to eventCache");
            }
        }
    }
    
    #if UNITY_EDITOR
    [ContextMenu("Fill eventData")]
    public void FillEventData()
    {
        foreach (var eventData in events)
        {
            eventData.PopulateData();
        }
        AssetDatabase.SaveAssetIfDirty(this);
    }
    #endif

    

    private bool TryGetEvent(string eventName, out EventData eventData)
    {
        if (_eventCache.TryGetValue(eventName, out eventData))
        {
            if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
            {
                Debug.Log("Successfully retrieved " + eventName);
            }
            
            return true;
        }

        if (AudioManager.Instance.debug)
        {
            Debug.LogWarning("Failed to get " + eventName);
        }
        
        return false;
    }
    
    #endregion

    #region Looping Events
    
    private Dictionary<GameObject, EventInstance> _instanceList = new Dictionary<GameObject, EventInstance>();

    public void ResetInstanceList() //kanske inte behövs men maybe
    {
        foreach (var instance in _instanceList)
        {
            instance.Value.stop(STOP_MODE.IMMEDIATE);
            instance.Value.release();
            _instanceList.Remove(instance.Key);
        }
        if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
        {
            Debug.Log("Resetting instance list");
        }
    }
    
    public void CreateInstance(string eventName, GameObject gameObject = null, bool attachToObject = false, bool followObject = true)
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        if (eventData.isOneShot) return;

        if (gameObject != null)
        {
            var instance = RuntimeManager.CreateInstance(eventData.eventReference);
            _instanceList.Add(gameObject, instance);
            if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
            {
                Debug.Log("Created instance for " + eventName + " and added it to the instance list along with " + gameObject.name);
            }

            if (!attachToObject) return;
            if (followObject)
            {
                RuntimeManager.AttachInstanceToGameObject(instance, gameObject);
                if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
                {
                    Debug.Log("Attached " + eventName + " to " + gameObject.name);
                }
            }
            else
            {
                instance.set3DAttributes(gameObject.transform.To3DAttributes());
                if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
                {
                    Debug.Log("Set 3D attributes of " + eventName + " to those of " + gameObject.name);
                }
            }
        }
        else
        {
            eventData.eventInstance = RuntimeManager.CreateInstance(eventData.eventReference);
            if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
            {
                Debug.Log("Created instance for " + eventName);
            }
        }
    }
    
    public void ReleaseInstance(string eventName, GameObject gameObject = null)
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        if (eventData.isOneShot) return;

        if (gameObject != null)
        {
            if (_instanceList.TryGetValue(gameObject, out var instance))
            {
                instance.release();
                _instanceList.Remove(gameObject);
                if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
                {
                    Debug.Log("Releasing instance for " + eventName + " and removed from instance list");
                }
            }
        }
        else
        {
            eventData.eventInstance.release();
            if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
            {
                Debug.Log("Releasing instance for " + eventName);
            }
        }
    }
    
    public void StartEvent(string eventName, GameObject gameObject = null)
    {
        if (TryGetEvent(eventName, out var eventData))
        {
            if (gameObject != null)
            {
                if (!_instanceList.TryGetValue(gameObject, out var instance)) return;
                instance.getPlaybackState(out var playbackState);
                if (playbackState != PLAYBACK_STATE.PLAYING)
                {
                    instance.start();
                    if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
                    {
                        Debug.Log("Started event " + eventName + " on " + gameObject.name);
                    }
                }
            }
            else
            {
                eventData.eventInstance.getPlaybackState(out var playbackState);
                if (playbackState != PLAYBACK_STATE.PLAYING)
                {
                    eventData.eventInstance.start();
                    if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
                    {
                        Debug.Log("Started event " + eventName);
                    }
                }
            }
        }
    }

    public void StopEvent(string eventName, STOP_MODE stopMode, GameObject gameObject = null)
    {
        if (TryGetEvent(eventName, out var eventData))
        {
            if (gameObject != null)
            {
                if (!_instanceList.TryGetValue(gameObject, out var instance)) return;
                instance.stop(stopMode);
                if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
                {
                    Debug.Log("Stopped event " + eventName + " on " + gameObject.name);
                }
            }
            else
            {
                eventData.eventInstance.stop(stopMode);
                if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
                {
                    Debug.Log("Stopped event " + eventName);
                }
            }
        }
    }
    
    public void SetParameter(string eventName, string paramName, float paramValue, GameObject gameObject = null)
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        if (!eventData.ParameterCache.TryGetValue(paramName, out var parameterData)) return;
        
        if (parameterData.isGlobal)
        {
            RuntimeManager.StudioSystem.setParameterByID(parameterData.ID(), paramValue);
            if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
            {
                Debug.Log("Set global parameter " + paramName + " to " + paramValue);
            }
        }
        else
        {
            if (gameObject != null)
            {
                if (!_instanceList.TryGetValue(gameObject, out var instance)) return;
                instance.setParameterByID(parameterData.ID(), paramValue);
                if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
                {
                    Debug.Log("Set " + paramName + " in event " + eventName + " on object " + gameObject.name + " to " + paramValue);
                }
            }
            else
            {
                eventData.eventInstance.setParameterByID(parameterData.ID(), paramValue);
                if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
                {
                    Debug.Log("Set " + paramName + " in event " + eventName + " to " + paramValue);
                }
            }
        }
    }

    public void KeyOff(string eventName, GameObject gameObject = null)
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        
        if (gameObject != null)
        { 
            if (!_instanceList.TryGetValue(gameObject, out var instance)) return;
            instance.keyOff();
            
            if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
            {
                Debug.Log("KeyOff in event " + eventName + " on object " + gameObject.name);
            }
        }
        else
        { 
            eventData.eventInstance.keyOff();
            if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
            {
                Debug.Log("KeyOff in event " + eventName);
            }
        }
    }
    
    #endregion

    #region SFX

    public void PlayOneShot(string eventName, string[] paramNames = null, float[] paramValues = null ,GameObject gameObject = null, bool followObject = true)
    {
        if (TryGetEvent(eventName, out var eventData))
        {
            if (!eventData.isOneShot)
            {
                if (AudioManager.Instance.debug)
                {
                    Debug.LogWarning(eventName + " is not a OneShot event and should not be played through this method");
                }
                return;
            }
            
            var instance = RuntimeManager.CreateInstance(eventData.eventReference);
            
            if (paramNames != null && paramValues != null)
            {
                for (var i = 0; i < paramNames.Length; i++)
                {
                    if (eventData.ParameterCache.TryGetValue(paramNames[i], out var parameterData))
                    {
                        instance.setParameterByID(parameterData.ID(), paramValues[i]);
                    }
                }
            }

            if (gameObject != null && eventData.is3D)
            {
                if (followObject) RuntimeManager.AttachInstanceToGameObject(instance, gameObject);
                else instance.set3DAttributes(gameObject.transform.To3DAttributes());
            }

            instance.start();
            instance.release();
            
            if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
            {
                Debug.Log("Playing OneShot: " + eventName);
            }
        }
    }
    
    #endregion
    
    #region VA
    
    //Logik för VA här???
    
    #endregion

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
