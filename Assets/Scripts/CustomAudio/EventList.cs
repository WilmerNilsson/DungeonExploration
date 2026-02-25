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

    //Lägger till alla eventData till _eventCache för snabbare lookup än foreach loop i eventData
    //samt refreshar ParameterCache i alla eventData
    public void RefreshEventCache() 
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
        var hasChanged = false;
        foreach (var eventData in events)
        {
            var previous = CloneEventData(eventData);
            eventData.PopulateData();
            if (HasDataChanged(previous, eventData))
            {
                hasChanged = true;
            }
        }
        if (!hasChanged) return;
        //Debug.Log("EventData changed");
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssetIfDirty(this);
    }

    private static EventData CloneEventData(EventData eventData)
    {
        return new EventData()
        {
            eventName = eventData.eventName,
            eventReference = eventData.eventReference,
            banks = eventData.banks,
            isOneShot = eventData.isOneShot,
            is3D = eventData.is3D,
            isOcclusion = eventData.isOcclusion,
            minDistance = eventData.minDistance,
            maxDistance = eventData.maxDistance,
            parameters = eventData.parameters,
        };
    }
    
    private static bool HasDataChanged(EventData prev, EventData current)
    {
        var hasChanged = false;
        
        if (prev.eventName != current.eventName)
        {
            //Debug.Log("name changed");
            hasChanged = true;
        }
        
        if (hasChanged) return true;
        
        if (prev.eventReference.Guid != current.eventReference.Guid)
        {
            //Debug.Log("reference changed");
            hasChanged = true;
        }

        if (hasChanged) return true;
        
        if (prev.banks.Length == current.banks.Length)
        {
            hasChanged = HasStringArrayChanged(prev.banks, current.banks);
        }
        else
        {
            //Debug.Log("bankLength changed");
            hasChanged = true;
        }
        
        if (hasChanged) return true;
        
        if (prev.isOneShot != current.isOneShot)
        {
            //Debug.Log("isOneShot changed");
            hasChanged = true;
        }
        
        if (hasChanged) return true;
        
        if (prev.is3D != current.is3D)
        {
            //Debug.Log("is3D changed");
            hasChanged = true;
        }
        
        if (hasChanged) return true;
        
        if (prev.isOcclusion != current.isOcclusion)
        {
            //Debug.Log("isOcclusion changed");
            hasChanged = true;
        }
        
        if (hasChanged) return true;
        
        if (!Mathf.Approximately(prev.minDistance, current.minDistance))
        {
            //Debug.Log("minDistance changed");
            hasChanged = true;
        }
        
        if (hasChanged) return true;
        
        if (!Mathf.Approximately(prev.maxDistance, current.maxDistance))
        {
            //Debug.Log("maxDistance changed");
            hasChanged = true;
        }
        
        if (hasChanged) return true;
        
        if (prev.parameters.Length == current.parameters.Length)
        {
            var prevParamStrings = new string[prev.parameters.Length];
            for (int i = 0; i < prev.parameters.Length; i++)
            {
                prevParamStrings[i] = prev.parameters[i].paramName + prev.parameters[i].isGlobal + prev.parameters[i].data1 + prev.parameters[i].data2;
            }
            var currParamStrings = new string[current.parameters.Length];
            for (int i = 0; i < current.parameters.Length; i++)
            {
                currParamStrings[i] = current.parameters[i].paramName + current.parameters[i].isGlobal + current.parameters[i].data1 + current.parameters[i].data2;
            }
            hasChanged = HasStringArrayChanged(prevParamStrings, currParamStrings);
        }
        else
        {
            //Debug.Log("paramLength changed");
            hasChanged = true;
        }

        if (hasChanged) return true;
        return false;
    }

    private static bool HasStringArrayChanged(string[] prev, string[] current)
    {
        var hasChanged = false;
        
        foreach (var s in prev)
        {
            if (!current.Contains(s))
            {
                //Debug.Log("string array changed");
                hasChanged = true;
            }
        }

        return hasChanged;
    }
    
#endif
    
    public bool TryGetEvent(string eventName, out EventData eventData) //Om ett event finns i eventCache OCH banken eventet hör till är laddad returneras true samt EventData, annars false
    {
        if (EventCache.TryGetValue(eventName, out eventData))
        {
            if (HasEventLoaded(eventData))
            {
                AudioDebug.Print("Successfully retrieved " + eventName);
                return true;
            }
        }
        AudioDebug.Print("Failed to get " + eventName, true);
        
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
                AudioDebug.Print("The bank: " + bank + " for " + eventData.eventName + " isn't loaded", true);
                hasLoaded = false;
            }
        }
        if(hasLoaded) AudioDebug.Print("All banks for " + eventData.eventName + " are loaded");
        return hasLoaded;
    }
    
    #endregion

    #region Looping Events
    
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
    
    public void CreateInstance(string eventName, GameObject gameObject = null, bool followObject = true)
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        
        if (eventData.isOneShot)
        {
            AudioDebug.Print("Didn't create instance for " + eventName + " because it is not a looping event", true);
            return;
        } //Skapa inte instans utan att släppa den om eventet är oneShot;

        if (eventData.is3D && gameObject == null)
        {
            AudioDebug.Print("Didn't create instance for " + eventName + " Since it is a 3D event and needs a gameObject to attach to", true);
            return;
        }
        
        if (gameObject != null) //Om gameObject skickas med så skapa ny instans och lägg till den i InstanceList tillsammans med gameObject
        {
            if (InstanceList.ContainsKey(gameObject))
            {
                AudioDebug.Print("Didn't create an instance for " + eventName + " at " + gameObject.name + " because " + gameObject.name + " already has an event instance", true);
                return;
            }
            var instance = RuntimeManager.CreateInstance(eventData.eventReference);
            InstanceList.Add(gameObject, instance);
            InstanceToEventData.Add(instance, eventData);
            AudioDebug.Print("Created instance for " + eventName + " and added it to the instance list along with " + gameObject.name);

            if (!eventData.is3D) return; //Om event är 3D och attachToObject, fäser vi eventet på gameObject,
                                                            //om followObject är false följer eventet inte med gameObject utan
                                                            //stannar kvar på samma position där objektet var när instansen skapades
            if (followObject)
            {
                if (gameObject.TryGetComponent<Rigidbody>(out var rb))
                {
                    RuntimeManager.AttachInstanceToGameObject(instance, gameObject);
                }
                else
                {
                    RuntimeManager.AttachInstanceToGameObject(instance, gameObject, true);
                }
                
                AudioDebug.Print("Attached " + eventName + " to " + gameObject.name);
            }
            else
            {
                instance.set3DAttributes(gameObject.transform.To3DAttributes());
                AudioDebug.Print("Set 3D attributes of " + eventName + " to those of " + gameObject.name);
            }
        }
        else //Om inget gameObject finns lägger vi istället instance i eventData, t.ex för musikEvent som bara har en emitter.
        {
            if (eventData.EventInstance.isValid())
            {
                AudioDebug.Print("Didn't create instance for " + eventName + " because it already has an instance", true);
                return;
            }

            if (eventData.is3D)
            {
                AudioDebug.Print("Didn't create instance for "+ eventName + " because it's a 3D event and needs to be attached to a 3D object to work", true);
                return;
            }
            eventData.EventInstance = RuntimeManager.CreateInstance(eventData.eventReference);
            AudioDebug.Print("Created instance for " + eventName);
        }
    }

    public void LoadSampleData(string eventName)
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        if (eventData.isOneShot)
        {
            AudioDebug.Print("Didn't load samples for " + eventName + " because it is not a looping event", true);
            return;
        }
        
        var eventDesc = RuntimeManager.GetEventDescription(eventData.eventReference);
        eventDesc.loadSampleData();
        AudioDebug.Print("Loading samples for" + eventName);
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
                    AudioDebug.Print("Didn't release instance for " + eventName + " at " + gameObject.name + " because it is not a valid instance", true);
                    return;
                }
                instance.getPlaybackState(out var state);
                if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING)
                {
                    instance.release();
                    InstanceToEventData.Remove(instance);
                    InstanceList.Remove(gameObject);
                    AudioDebug.Print("Releasing instance for " + eventName + " and removed from instance list");
                }
                else
                {
                    AudioDebug.Print("Instance for " + eventData.eventName + " at " + gameObject.name + " needs to be stopped before release", true);
                }
            }
        }
        else
        {
            if (!eventData.EventInstance.isValid())
            {
                AudioDebug.Print("Didn't release instance for " + eventName + " because it is not a valid instance", true);
                return;
            }
            
            eventData.EventInstance.getPlaybackState(out var state);
            if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING)
            {
                eventData.EventInstance.release();
                AudioDebug.Print("Releasing instance for " + eventName);
            }
            else
            {
                AudioDebug.Print("Instance for " + eventData.eventName + " needs to be stopped before release", true);
            }
        }
    }

    public void UnloadSampleData(string eventName)
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        if (eventData.isOneShot)
        {
            AudioDebug.Print("Didn't unload samples for " + eventName + " because it is not a looping event", true);
            return;
        }
        var eventDesc = RuntimeManager.GetEventDescription(eventData.eventReference);
        eventDesc.unloadSampleData();
        AudioDebug.Print("Unloading samples for" + eventName);
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
                    AudioDebug.Print("Didn't start " + eventName + " at " + gameObject.name + " because it's instance is not valid", true);
                    return;
                }
                instance.getPlaybackState(out var playbackState);
                if (playbackState != PLAYBACK_STATE.PLAYING)
                {
                    instance.start();
                    AudioDebug.Print("Started event " + eventName + " on " + gameObject.name);
                }
            }
            else
            {
                if (!eventData.EventInstance.isValid())
                {
                    AudioDebug.Print("Didn't start " + eventName + " because it's instance is not valid", true);
                    return;
                }
                
                eventData.EventInstance.getPlaybackState(out var playbackState);
                if (playbackState != PLAYBACK_STATE.PLAYING)
                {
                    eventData.EventInstance.start();
                    AudioDebug.Print("Started event " + eventName);
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
                AudioDebug.Print("Stopped event " + eventName + " on " + gameObject.name);
            }
            else
            {
                eventData.EventInstance.stop(stopMode);
                AudioDebug.Print("Stopped event " + eventName);
            }
        }
    }
    
    public void SetParameter(string eventName, string paramName, float paramValue, GameObject gameObject = null) //Som CreateInstance men sätter parametrar. Om en parameter är global behöver man inte köra setParameter på instansen utan bara i studioSystem.
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        if (!eventData.ParameterCache.TryGetValue(paramName, out var parameterData))
        {
            AudioDebug.Print("Couldn't find parameter: " + paramName, true);
            return;
        }
        
        if (parameterData.isGlobal)
        {
            RuntimeManager.StudioSystem.setParameterByID(parameterData.ID(), paramValue);
            AudioDebug.Print("Set global parameter " + paramName + " to " + paramValue);
        }
        else
        {
            if (gameObject != null)
            {
                if (!InstanceList.TryGetValue(gameObject, out var instance)) return;
                instance.setParameterByID(parameterData.ID(), paramValue);
                AudioDebug.Print("Set " + paramName + " in event " + eventName + " on object " + gameObject.name + " to " + paramValue);
            }
            else
            {
                eventData.EventInstance.setParameterByID(parameterData.ID(), paramValue);
                AudioDebug.Print("Set " + paramName + " in event " + eventName + " to " + paramValue);
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
            
            AudioDebug.Print("KeyOff in event " + eventName + " on object " + gameObject.name);
        }
        else
        { 
            eventData.EventInstance.keyOff();
            AudioDebug.Print("KeyOff in event " + eventName);
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

    private float _occlusion;
    private int _walls;
    
    public void CheckOcclusions()
    {
        if (!AudioManager.Listener) return;
        foreach (var kvp in InstanceList)
        {
            if (!InstanceToEventData.TryGetValue(kvp.Value, out var eventData)) continue;
            if (!eventData.isOcclusion) continue;
            if (Vector3.Distance(kvp.Key.transform.position, AudioManager.Listener.transform.position) <
                eventData.maxDistance + 1) //gör inte raycast om distance är för långt, vi lägger till +1 på maxDistance så värden hinner sättas innan ljudet börjar bli audible
            {
                AudioManager.Instance.occlusionChecker.CheckOcclusion(kvp.Key, AudioManager.Listener,out _occlusion, eventData.maxDistance + 1);
                AudioManager.Instance.wallChecker.CheckWalls(kvp.Key, AudioManager.Listener, out _walls);
                if (eventData.ParameterCache.TryGetValue("Occluded", out var parameterData))
                {
                    kvp.Value.setParameterByID(parameterData.ID(), _occlusion);
                }
                if (eventData.ParameterCache.TryGetValue("Walls", out parameterData))
                {
                    kvp.Value.setParameterByID(parameterData.ID(), _walls);
                }
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
                AudioDebug.Print(eventName + " is not a OneShot event and should not be played through this method", true);
                return;
            }
            
            if (eventData.is3D && gameObject == null)
            {
                AudioDebug.Print("Didn't play OneShot for " + eventName + " Since it is a 3D event and needs a gameObject to attach to", true);
                return;
            }
            
            var instance = RuntimeManager.CreateInstance(eventData.eventReference);
            
            if (gameObject && eventData.is3D)
            {
                if (followObject)
                {
                    if (gameObject.TryGetComponent<Rigidbody>(out var rb))
                    {
                        RuntimeManager.AttachInstanceToGameObject(instance, gameObject);
                    }
                    else
                    {
                        RuntimeManager.AttachInstanceToGameObject(instance, gameObject, true);
                    }
                }
                else instance.set3DAttributes(gameObject.transform.To3DAttributes());

                if (eventData.isOcclusion)
                {
                    if (Vector3.Distance(gameObject.transform.position, AudioManager.Listener.transform.position) <
                        eventData.maxDistance + 1)
                    {
                        AudioManager.Instance.occlusionChecker.CheckOcclusion(gameObject,AudioManager.Listener, out var occlusion);
                        AudioManager.Instance.wallChecker.CheckWalls(gameObject,AudioManager.Listener, out var walls);
                        if (eventData.ParameterCache.TryGetValue("Occlusion", out var parameterData))
                        {
                            instance.setParameterByID(parameterData.ID(), occlusion);
                            AudioDebug.Print("Successfully set occlusion for " + eventName);
                        }
                        if (eventData.ParameterCache.TryGetValue("Walls", out parameterData))
                        {
                            instance.setParameterByID(parameterData.ID(), walls);
                            AudioDebug.Print("Successfully set walls for " + eventName);
                        }
                    }
                }
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
            
            AudioDebug.Print("Playing OneShot: " + eventName);
        }
    }
    
    #endregion
    
    #region VA

    public void InitializeDialogue(string eventName)
    {
        if (!TryGetEvent(eventName, out var eventData)) return; //Om event finns & det inte redan finns en instans skapar vi en
        if (eventData.EventInstance.isValid())
        {
            AudioDebug.Print("Didn't create instance for dialogue event " + eventData.eventName + " since it already has a valid instance", true);
            return;
        }
        eventData.EventInstance = RuntimeManager.CreateInstance(eventData.eventReference);
        AudioDebug.Print("Created instance for dialogue event " + eventData.eventName);
    }

    public void SayLine(string eventName, string lineParameter, int lineIndex)
    {
        if (!TryGetEvent(eventName, out var eventData)) return; 
        if (!eventData.EventInstance.isValid())
        {
            AudioDebug.Print("You need to create an instance for " + eventName + " before trying to start a dialogue", true);
            return;
        } //Om event och instans finns ställer vi in parametrar(om de finns) och spelar instansen
        
        if (!eventData.ParameterCache.TryGetValue(lineParameter, out var parameterData))
        {
            AudioDebug.Print("Couldn't find parameter: " + lineParameter, true);
            return;
        }
        eventData.EventInstance.setParameterByID(parameterData.ID(), lineIndex);
        
        eventData.EventInstance.start();
    }

    public void StopLine(string eventName) //Stoppa instansen om event finns
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        eventData.EventInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }

    public void EndDialogue(string eventName) //Stoppa och släpp instans
    {
        if (!TryGetEvent(eventName, out var eventData)) return;
        eventData.EventInstance.stop(STOP_MODE.ALLOWFADEOUT);
        eventData.EventInstance.release();
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
