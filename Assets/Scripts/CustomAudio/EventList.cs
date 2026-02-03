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

    public void RefreshEventCache() //Lägger till alla eventData till _eventCache för snabbare lookup än foreach loop i eventData
                                    //samt refreshar ParameterCache i alla eventData
    {
        _eventCache = new Dictionary<string, EventData>();
        foreach (var eventData in events)
        {
            eventData.RefreshParameterCache();
            _eventCache.Add(eventData.eventName, eventData);

            if (debug)
            {
                Debug.Log("Added " + eventData.eventName + " to eventCache");
            }
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
        AssetDatabase.SaveAssetIfDirty(this);
    }
    #endif
    
    private bool TryGetEvent(string eventName, out EventData eventData) //Om ett event finns i eventCache returneras true samt EventData, annars false
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
    
    public Dictionary<GameObject, EventInstance> InstanceList = new Dictionary<GameObject, EventInstance>(); //TODO: Bättre metod för att spara instanser, just nu kan ett gameObject bara ha en instans på sig.
    
    public void CleanupInstanceList() //Kallas av audioManager vid scenladdning, stoppar alla event på gameObjects som inte längre finns
        
    {
        foreach (var instance in InstanceList)
        {
            if(!instance.Key.activeInHierarchy) continue;
            instance.Value.stop(STOP_MODE.IMMEDIATE);
            instance.Value.release();
            InstanceList.Remove(instance.Key);
            if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
            {
                Debug.Log("Stopped and removed eventInstance at " + instance.Key.name);
            }
        }
    }
    
    public void CreateInstance(string eventName, GameObject gameObject = null, bool attachToObject = false, bool followObject = true)
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        if (eventData.isOneShot) return; //Skapa inte instans utan att släppa den om eventet är oneShot;

        if (gameObject != null) //Om gameObject skickas med så skapa ny instans och lägg till den i InstanceList tillsammans med gameObject
        {
            var instance = RuntimeManager.CreateInstance(eventData.eventReference);
            InstanceList.Add(gameObject, instance);
            if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
            {
                Debug.Log("Created instance for " + eventName + " and added it to the instance list along with " + gameObject.name);
            }

            if (!attachToObject && !eventData.is3D) return; //Om event är 3D och attachToObject, fäser vi eventet på gameObject,
                                                            //om followObject är false följer eventet inte med gameObject utan
                                                            //stannar kvar på samma position där objektet var när instansen skapades
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
        else //Om inget gameObject finns lägger vi istället instance i eventData, t.ex för musikEvent som bara har en emitter.
        {
            eventData.eventInstance = RuntimeManager.CreateInstance(eventData.eventReference);
            if (AudioManager.Instance.debug && !AudioManager.Instance.showOnlyWarnings)
            {
                Debug.Log("Created instance for " + eventName);
            }
        }
    }
    
    public void ReleaseInstance(string eventName, GameObject gameObject = null) //Som CreateInstance fast släpper instansen istället
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        
        if (gameObject != null)
        {
            if (InstanceList.TryGetValue(gameObject, out var instance))
            {
                instance.release();
                InstanceList.Remove(gameObject);
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
    
    public void StartEvent(string eventName, GameObject gameObject = null) //Som CreateInstance fast startar instansen istället OM instansen inte redan spelar
    {
        if (TryGetEvent(eventName, out var eventData))
        {
            if (gameObject != null)
            {
                if (!InstanceList.TryGetValue(gameObject, out var instance)) return;
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

    public void StopEvent(string eventName, STOP_MODE stopMode, GameObject gameObject = null) //Som CreateInstance fast stoppar med stopMode, denna bör kallas innan releaseInstance
    {
        if (TryGetEvent(eventName, out var eventData))
        {
            if (gameObject != null)
            {
                if (!InstanceList.TryGetValue(gameObject, out var instance)) return;
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
    
    public void SetParameter(string eventName, string paramName, float paramValue, GameObject gameObject = null) //Som CreateInstance men sätter parametrar. Om en parameter är global behöver man inte köra setParameter på instansen utan bara i studioSystem.
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        if (!eventData.ParameterCache.TryGetValue(paramName, out var parameterData))
        {
            Debug.LogWarning(paramName + " " + eventData.ParameterCache.TryGetValue(paramName, out var parameter));
            return;
        }
        
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
                if (!InstanceList.TryGetValue(gameObject, out var instance)) return;
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

    public void KeyOff(string eventName, GameObject gameObject = null) //Som CreateInstance men skickar KeyOff
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        
        if (gameObject != null)
        { 
            if (!InstanceList.TryGetValue(gameObject, out var instance)) return;
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

    #region SFX

    public void PlayOneShot(string eventName, string[] paramNames = null, float[] paramValues = null,GameObject gameObject = null, bool followObject = true)
    {
        //Om event finns och är oneshot skapar vi en instans, ställer in parametrar (om de finns) och fäster ljudet på ett gameObject (om de finns), om followObject är false följer ljudet inte efter objektet (crazy)
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
            
            if (gameObject != null && eventData.is3D)
            {
                if (followObject) RuntimeManager.AttachInstanceToGameObject(instance, gameObject);
                else instance.set3DAttributes(gameObject.transform.To3DAttributes());
            }
            
            if (paramNames != null && paramValues != null)
            {
                for (var i = 0; i < paramNames.Length; i++)
                {
                    if (eventData.TryGetParamData(paramNames[i], out var paramData))
                    {
                        if (paramData.isGlobal)
                        {
                            RuntimeManager.StudioSystem.setParameterByID(paramData.ID(), paramValues[i]);
                        }
                        else
                        {
                            instance.setParameterByID(paramData.ID(), paramValues[i]);
                        }
                    }
                }
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
