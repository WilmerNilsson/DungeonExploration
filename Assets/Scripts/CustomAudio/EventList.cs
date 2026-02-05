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
public class EventList : ScriptableObject
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

            PrintDebug("Added " + eventData.eventName + " to eventCache");
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
    
    private bool TryGetEvent(string eventName, out EventData eventData) //Om ett event finns i eventCache OCH banken eventet hör till är laddad returneras true samt EventData, annars false
    {
        if (_eventCache.TryGetValue(eventName, out eventData))
        {
            if (HasEventLoaded(eventData))
            {
                PrintDebug("Successfully retrieved " + eventName);
                return true;
            }
        }
        PrintDebug("Failed to get " + eventName, true);
        
        return false;
    }
    
    private const string BankExtension = ".bank";
    private bool HasEventLoaded(EventData eventData)
    {
        var hasLoaded = true;
        foreach (var bank in eventData.banks)
        {
            if (!RuntimeManager.HasBankLoaded(bank + BankExtension))
            {
                PrintDebug("The bank: " + bank + " for " + eventData.eventName + " isn't loaded", true);
                hasLoaded = false;
            }
        }
        if(hasLoaded) PrintDebug("All banks for " + eventData.eventName + " are loaded");
        return hasLoaded;
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
            PrintDebug("Stopped and removed eventInstance at " + instance.Key.name);
        }
    }
    
    public void CreateInstance(string eventName, GameObject gameObject = null, bool followObject = true)
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        
        if (eventData.isOneShot)
        {
            PrintDebug("Didn't create instance for " + eventName + " because it is not a looping event", true);
            return;
        } //Skapa inte instans utan att släppa den om eventet är oneShot;

        if (eventData.is3D && gameObject == null)
        {
            PrintDebug("Didn't create instance for " + eventName + " Since it is a 3D event and needs a gameObject to attach to", true);
            return;
        }
        
        if (gameObject != null) //Om gameObject skickas med så skapa ny instans och lägg till den i InstanceList tillsammans med gameObject
        {
            if (InstanceList.ContainsKey(gameObject))
            {
                PrintDebug("Didn't create an instance for " + eventName + " at " + gameObject.name + " because " + gameObject.name + " already has an event instance", true);
                return;
            }
            var instance = RuntimeManager.CreateInstance(eventData.eventReference);
            InstanceList.Add(gameObject, instance);
            PrintDebug("Created instance for " + eventName + " and added it to the instance list along with " + gameObject.name);

            if (!eventData.is3D) return; //Om event är 3D och attachToObject, fäser vi eventet på gameObject,
                                                            //om followObject är false följer eventet inte med gameObject utan
                                                            //stannar kvar på samma position där objektet var när instansen skapades
            if (followObject)
            {
                RuntimeManager.AttachInstanceToGameObject(instance, gameObject);
                PrintDebug("Attached " + eventName + " to " + gameObject.name);
            }
            else
            {
                instance.set3DAttributes(gameObject.transform.To3DAttributes());
                PrintDebug("Set 3D attributes of " + eventName + " to those of " + gameObject.name);
            }
        }
        else //Om inget gameObject finns lägger vi istället instance i eventData, t.ex för musikEvent som bara har en emitter.
        {
            if (eventData.eventInstance.isValid())
            {
                PrintDebug("Didn't create instance for " + eventName + " because it already has an instance", true);
                return;
            }

            if (eventData.is3D)
            {
                PrintDebug("Didn't create instance for "+ eventName + " because it's a 3D event and needs to be attached to a 3D object to work", true);
                return;
            }
            eventData.eventInstance = RuntimeManager.CreateInstance(eventData.eventReference);
            PrintDebug("Created instance for " + eventName);
        }
    }
    
    public void ReleaseInstance(string eventName, GameObject gameObject = null) //Som CreateInstance fast släpper instansen istället
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        
        if (gameObject != null)
        {
            if (InstanceList.TryGetValue(gameObject, out var instance))
            {
                if (!instance.isValid())
                {
                    PrintDebug("Didn't release instance for " + eventName + " at " + gameObject.name + " because it is not a valid instance", true);
                    return;
                }
                instance.getPlaybackState(out var state);
                if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING)
                {
                    instance.release();
                    InstanceList.Remove(gameObject);
                    PrintDebug("Releasing instance for " + eventName + " and removed from instance list");
                }
                else
                {
                    PrintDebug("Instance for " + eventData.eventName + " at " + gameObject.name + " needs to be stopped before release", true);
                }
            }
        }
        else
        {
            if (!eventData.eventInstance.isValid())
            {
                PrintDebug("Didn't release instance for " + eventName + " because it is not a valid instance", true);
                return;
            }
            
            eventData.eventInstance.getPlaybackState(out var state);
            if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING)
            {
                eventData.eventInstance.release();
                PrintDebug("Releasing instance for " + eventName);
            }
            else
            {
                PrintDebug("Instance for " + eventData.eventName + " needs to be stopped before release", true);
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
                if (!instance.isValid())
                {
                    PrintDebug("Didn't start " + eventName + " at " + gameObject.name + " because it's instance is not valid", true);
                    return;
                }
                instance.getPlaybackState(out var playbackState);
                if (playbackState != PLAYBACK_STATE.PLAYING)
                {
                    instance.start();
                    PrintDebug("Started event " + eventName + " on " + gameObject.name);
                }
            }
            else
            {
                if (!eventData.eventInstance.isValid())
                {
                    PrintDebug("Didn't start " + eventName + " because it's instance is not valid", true);
                    return;
                }
                
                eventData.eventInstance.getPlaybackState(out var playbackState);
                if (playbackState != PLAYBACK_STATE.PLAYING)
                {
                    eventData.eventInstance.start();
                    PrintDebug("Started event " + eventName);
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
                PrintDebug("Stopped event " + eventName + " on " + gameObject.name);
            }
            else
            {
                eventData.eventInstance.stop(stopMode);
                PrintDebug("Stopped event " + eventName);
            }
        }
    }
    
    public void SetParameter(string eventName, string paramName, float paramValue, GameObject gameObject = null) //Som CreateInstance men sätter parametrar. Om en parameter är global behöver man inte köra setParameter på instansen utan bara i studioSystem.
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        if (!eventData.ParameterCache.TryGetValue(paramName, out var parameterData))
        {
            PrintDebug("Couldn't find parameter: " + paramName, true);
            return;
        }
        
        if (parameterData.isGlobal)
        {
            RuntimeManager.StudioSystem.setParameterByID(parameterData.ID(), paramValue);
            PrintDebug("Set global parameter " + paramName + " to " + paramValue);
        }
        else
        {
            if (gameObject != null)
            {
                if (!InstanceList.TryGetValue(gameObject, out var instance)) return;
                instance.setParameterByID(parameterData.ID(), paramValue);
                PrintDebug("Set " + paramName + " in event " + eventName + " on object " + gameObject.name + " to " + paramValue);
            }
            else
            {
                eventData.eventInstance.setParameterByID(parameterData.ID(), paramValue);
                PrintDebug("Set " + paramName + " in event " + eventName + " to " + paramValue);
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
            
            PrintDebug("KeyOff in event " + eventName + " on object " + gameObject.name);
        }
        else
        { 
            eventData.eventInstance.keyOff();
            PrintDebug("KeyOff in event " + eventName);
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
                PrintDebug(eventName + " is not a OneShot event and should not be played through this method", true);
                return;
            }
            
            if (eventData.is3D && gameObject == null)
            {
                PrintDebug("Didn't play OneShot for " + eventName + " Since it is a 3D event and needs a gameObject to attach to", true);
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
            
            PrintDebug("Playing OneShot: " + eventName);
        }
    }
    
    #endregion
    
    #region VA

    public void InitializeDialogue(string eventName)
    {
        if (!TryGetEvent(eventName, out var eventData)) return; //Om event finns & det inte redan finns en instans skapar vi en
        if (eventData.eventInstance.isValid())
        {
            PrintDebug("Didn't create instance for dialogue event " + eventData.eventName + " since it already has a valid instance", true);
            return;
        }
        eventData.eventInstance = RuntimeManager.CreateInstance(eventData.eventReference);
        PrintDebug("Created instance for dialogue event " + eventData.eventName);
    }

    public void SayLine(string eventName, string lineParameter, int lineIndex)
    {
        if (!TryGetEvent(eventName, out var eventData)) return; 
        if (!eventData.eventInstance.isValid())
        {
            PrintDebug("You need to create an instance for " + eventName + " before trying to start a dialogue", true);
            return;
        } //Om event och instans finns ställer vi in parametrar(om de finns) och spelar instansen
        
        if (!eventData.ParameterCache.TryGetValue(lineParameter, out var parameterData))
        {
            PrintDebug("Couldn't find parameter: " + lineParameter, true);
            return;
        }
        eventData.eventInstance.setParameterByID(parameterData.ID(), lineIndex);
        
        eventData.eventInstance.start();
    }

    public void StopLine(string eventName) //Stoppa instansen om event finns
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        eventData.eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }

    public void EndDialogue(string eventName) //Stoppa och släpp instans
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        eventData.eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
        eventData.eventInstance.release();
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

    private void PrintDebug(string message, bool isWarning = false)
    {
        if (!AudioManager.Instance.debug) return;
        if (isWarning)
        {
            Debug.LogWarning(message);
            return;
        }
        if (!AudioManager.Instance.showOnlyWarnings)
        {
            Debug.Log(message);
        }
    }
}
