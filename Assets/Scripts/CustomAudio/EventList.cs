using UnityEngine;
using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Debug = UnityEngine.Debug;
using GUID = FMOD.GUID;
using Object = System.Object;
using STOP_MODE = FMOD.Studio.STOP_MODE;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "EventList", menuName = "Scriptable Objects/EventList")]
public class EventList : ScriptableObject //TODO: Metoder, flytta cache till AudioManager? typ dictionary<path, eventData>?
{
    #region EventData
    
    [Serializable]
    public class EventData
    {
        public string eventName;
        public EventReference eventReference;
        public GUID guid;
        public string[] banks;
        public bool isOneShot;
        public bool is3D;
        public bool isDoppler;
        public float minDistance;
        public float maxDistance;
        
        public EventInstance eventInstance;
        
        [Serializable]
        public class ParameterData
        {
            public string paramName;
            public bool isGlobal;
            public uint data1;
            public uint data2;

            public PARAMETER_ID ID()
            {
                return new PARAMETER_ID() { data1 = data1, data2 = data2 };
            }
        }

        public ParameterData[] parameters;
        
        public Dictionary<string, ParameterData> ParameterCache;

        #if UNITY_EDITOR
        public void PopulateData()
        {
            EditorUtils.LoadPreviewBanks();
            guid = eventReference.Guid;

            if (eventName == null | eventName == "")
            {
                var split = eventReference.Path.Split('/');
                eventName = split[^1];
            }
            
            var editorEventRef = EventManager.EventFromGUID(guid);

            var tempBankList = new List<string>();
            foreach (var bank in editorEventRef.Banks)
            {
                tempBankList.Add(bank.Name);
            }
            banks = tempBankList.ToArray();
            
            EditorUtils.System.getEventByID(guid, out var eventDescription);
            
            eventDescription.isOneshot(out isOneShot);
            eventDescription.is3D(out is3D);
            eventDescription.isDopplerEnabled(out isDoppler);
            eventDescription.getMinMaxDistance(out minDistance, out maxDistance);

            ParameterCache = new Dictionary<string, ParameterData>();
            var tempParamList = new List<ParameterData>();
            foreach (var paramRef in editorEventRef.Parameters)
            {
                var tempParam = new ParameterData
                {
                    paramName = paramRef.Name,
                    isGlobal = paramRef.IsGlobal,
                    data1 = paramRef.ID.data1,
                    data2 = paramRef.ID.data2
                };
                tempParamList.Add(tempParam);
                ParameterCache.Add(paramRef.Name, tempParam);
            }
            parameters = tempParamList.ToArray();
            
            
            EditorUtils.UnloadPreviewBanks();
        }
        #endif
    }
    
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

    #region General
    
    public void ReleaseInstance(string eventName)
    {
        if (TryGetEvent(eventName, out var eventData))
        {
            eventData.eventInstance.release();
        }
    }

    public new void CreateInstance(string eventName)
    {
        if (TryGetEvent(eventName, out var eventData))
        {
            eventData.eventInstance = RuntimeManager.CreateInstance(eventData.eventReference);
        }
    }
    
    public void SetParameter(string eventName, string paramName, float paramValue)
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        if (!eventData.ParameterCache.TryGetValue(paramName, out var parameterData)) return;
        
        if (parameterData.isGlobal)
        {
            RuntimeManager.StudioSystem.setParameterByID(parameterData.ID(), paramValue);
        }
        else
        {
            eventData.eventInstance.setParameterByID(parameterData.ID(), paramValue);
        }
    }
    
    #endregion
    
    #region Music
    
    public void StartMusic(string eventName)
    {
        if (TryGetEvent(eventName, out var eventData))
        {
            if (!eventData.eventInstance.isValid())
                CreateInstance(eventName);
            eventData.eventInstance.getPlaybackState(out var playbackState);
            if (playbackState == PLAYBACK_STATE.PLAYING) return;
            else
            {
                eventData.eventInstance.start();
            }
        }
    }

    public void StopMusic(string eventName, STOP_MODE stopMode)
    {
        if (TryGetEvent(eventName, out var eventData))
        {
            eventData.eventInstance.stop(stopMode);
        }
    }

    public void KeyOff(string eventName)
    {
        if (TryGetEvent(eventName, out var eventData))
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
                //Change parameter
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

    #region Ambience

    private Dictionary<GameObject, EventInstance> ambienceInstances = new Dictionary<GameObject, EventInstance>();

    public void CreateAmbienceInstance(string eventName, GameObject gameObject)
    {
        if (TryGetEvent(eventName, out var eventData))
        {
            if (eventData.isOneShot) return;
            var instance = RuntimeManager.CreateInstance(eventData.eventReference);
            ambienceInstances.Add(gameObject, instance);
        }
    }
    
    public void StartAmbience(string eventName, GameObject gameObject, bool attachToObject = false, bool followSource = true)
    {
        if (TryGetEvent(eventName, out var eventData))
        {
            if (eventData.isOneShot) return;
            var instance = RuntimeManager.CreateInstance(eventData.eventReference);
            ambienceInstances.Add(gameObject, instance);
            
            if (gameObject != null && eventData.is3D && attachToObject)
            {
                if (followSource) RuntimeManager.AttachInstanceToGameObject(instance, gameObject);
                else instance.set3DAttributes(gameObject.transform.To3DAttributes());
            }

            instance.start();
        }
    }

    //RuntimeUtils.to3dAttributes
    #endregion
    
    public string category;
    public EventData[] events;
}
