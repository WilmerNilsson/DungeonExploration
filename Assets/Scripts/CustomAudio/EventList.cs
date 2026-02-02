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
    
    #region EventData
    
    #if UNITY_EDITOR
    [ContextMenu("Fill eventData")]
    public void FillEventDataAndRefreshCache()
    {
        _eventCache = new Dictionary<string, EventData>();
        foreach (var eventData in events)
        {
            eventData.PopulateData();
            _eventCache.Add(eventData.eventName, eventData);
        }
        AssetDatabase.SaveAssetIfDirty(this);
    }
    #endif

    private Dictionary<string, EventData> _eventCache = new Dictionary<string, EventData>();

    private bool TryGetEvent(string eventName, out EventData eventData)
    {
        if (_eventCache.TryGetValue(eventName, out eventData))
        {
            return true;
        }

        Debug.LogWarning(eventName + " not found, maybe it doesn't exist or the correct banks haven't been loaded.");
        
        return false;
    }
    
    #endregion

    #region Looping Events
    
    private Dictionary<GameObject, EventInstance> instanceList = new Dictionary<GameObject, EventInstance>();

    public void ResetInstanceList() //kanske inte behövs men maybe
    {
        foreach (var instance in instanceList)
        {
            instance.Value.stop(STOP_MODE.IMMEDIATE);
            instance.Value.release();
            instanceList.Remove(instance.Key);
        }
    }
    
    public void CreateInstance(string eventName, GameObject gameObject = null, bool attachToObject = false, bool followObject = true)
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        if (eventData.isOneShot) return;

        if (gameObject != null)
        {
            var instance = RuntimeManager.CreateInstance(eventData.eventReference);
            instanceList.Add(gameObject, instance);

            if (!attachToObject) return;
            if (followObject)
            {
                RuntimeManager.AttachInstanceToGameObject(instance, gameObject);
            }
            else
            {
                instance.set3DAttributes(gameObject.transform.To3DAttributes());
            }
        }
        else
        {
            eventData.eventInstance = RuntimeManager.CreateInstance(eventData.eventReference);
        }
    }
    
    public void ReleaseInstance(string eventName, GameObject gameObject = null)
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        if (eventData.isOneShot) return;

        if (gameObject != null)
        {
            if (instanceList.TryGetValue(gameObject, out var instance))
            {
                instance.release();
                instanceList.Remove(gameObject);
            }
        }
        else
        {
            eventData.eventInstance.release();
        }
    }
    
    public void StartEvent(string eventName, GameObject gameObject = null)
    {
        if (TryGetEvent(eventName, out var eventData))
        {
            if (gameObject != null)
            {
                if (!instanceList.TryGetValue(gameObject, out var instance)) return;
                instance.getPlaybackState(out var playbackState);
                if (playbackState != PLAYBACK_STATE.PLAYING)
                {
                    instance.start();
                }
            }
            else
            {
                eventData.eventInstance.getPlaybackState(out var playbackState);
                if (playbackState != PLAYBACK_STATE.PLAYING)
                {
                    eventData.eventInstance.start();
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
                if (!instanceList.TryGetValue(gameObject, out var instance)) return;
                instance.stop(stopMode);
            }
            else
            {
                eventData.eventInstance.stop(stopMode);
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
        }
        else
        {
            if (gameObject != null)
            {
                if (!instanceList.TryGetValue(gameObject, out var instance)) return;
                instance.setParameterByID(parameterData.ID(), paramValue);
            }
            else
            {
                eventData.eventInstance.setParameterByID(parameterData.ID(), paramValue);
            }
        }
    }

    public void KeyOff(string eventName, GameObject gameObject = null)
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        
        if (gameObject != null)
        { 
            if (!instanceList.TryGetValue(gameObject, out var instance)) return;
            instance.keyOff();
        }
        else
        { 
            eventData.eventInstance.keyOff();
        }
    }
    
    #endregion

    #region SFX

    public void PlayOneShot(string eventName, string[] paramNames = null, float[] paramValues = null ,GameObject gameObject = null, bool followSource = true)
    {
        if (TryGetEvent(eventName, out var eventData))
        {
            if (!eventData.isOneShot) return;
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
                if (followSource) RuntimeManager.AttachInstanceToGameObject(instance, gameObject);
                else instance.set3DAttributes(gameObject.transform.To3DAttributes());
            }

            instance.start();
            instance.release();
        }
    }
    
    #endregion
    
    #region VA
    
    //Logik för VA här???
    
    #endregion
}
