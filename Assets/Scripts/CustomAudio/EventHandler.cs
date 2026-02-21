using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace CustomAudio
{
    public class EventHandler
    {
        
        #region Lookup
        public Dictionary<string, EventList> EventListLookup = new Dictionary<string, EventList>();
        
        private bool TryGetEventData(string path, out EventData eventData, out EventList eventList)
        {
            if (EventListLookup.TryGetValue(path, out eventList))
            {
                var split = path.Split('/');
                if (eventList.EventCache.TryGetValue(split[^1], out eventData))
                {
                    if (HasEventLoaded(eventData))
                    {
                        return true;
                    }
                }
            }
            eventData = null;
            return false;
            }
        
            private const string BankExtension = ".bank";
            private bool HasEventLoaded(EventData eventData)
            {
                var hasLoaded = true;
                foreach (var bank in eventData.banks)
                {
                    if (RuntimeManager.HasBankLoaded(bank + BankExtension)) continue;
                    AudioDebug.Print("The bank: " + bank + " for " + eventData.eventName + " isn't loaded", true);
                    hasLoaded = false;
                }
                if(hasLoaded) AudioDebug.Print("All banks for " + eventData.eventName + " are loaded");
                return hasLoaded;
            }
            
            #endregion
            
            #region Looping Events
        
            public void CreateInstance(string eventName, GameObject gameObject = null, bool followObject = true)
            {
                if (!TryGetEventData(eventName, out var eventData, out var list)) return;
                
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
                
                if (gameObject != null) //Om gameObject skickas med så skapa ny instans och lägg till den i list.InstanceList tillsammans med gameObject
                {
                    if (list.InstanceList.ContainsKey(gameObject))
                    {
                        AudioDebug.Print("Didn't create an instance for " + eventName + " at " + gameObject.name + " because " + gameObject.name + " already has an event instance", true);
                        return;
                    }
                    var instance = RuntimeManager.CreateInstance(eventData.eventReference);
                    list.InstanceList.Add(gameObject, instance);
                    list.InstanceToEventData.Add(instance, eventData);
                    AudioDebug.Print("Created instance for " + eventName + " and added it to the instance list along with " + gameObject.name);

                    if (!eventData.is3D) return; //Om event är 3D och attachToObject, fäser vi eventet på gameObject,
                                                                    //om followObject är false följer eventet inte med gameObject utan
                                                                    //stannar kvar på samma position där objektet var när instansen skapades
                    if (followObject)
                    {
                        if (gameObject.TryGetComponent<Rigidbody>(out _))
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
                if (!TryGetEventData(eventName, out var eventData, out _)) return;
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
               if (!TryGetEventData(eventName, out var eventData, out var list)) return;
                
                if (gameObject != null)
                {
                    if (list.InstanceList.TryGetValue(gameObject, out var instance))
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
                            list.InstanceToEventData.Remove(instance);
                            list.InstanceList.Remove(gameObject);
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
               if (!TryGetEventData(eventName, out var eventData, out _)) return;
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
                if (TryGetEventData(eventName, out var eventData, out var list))
                {
                    if (gameObject != null)
                    {
                        if (!list.InstanceList.TryGetValue(gameObject, out var instance)) return;
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
                if (TryGetEventData(eventName, out var eventData, out var list))
                {
                    if (gameObject != null)
                    {
                        if (!list.InstanceList.TryGetValue(gameObject, out var instance)) return;
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
               if (!TryGetEventData(eventName, out var eventData, out var list)) return;
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
                        if (!list.InstanceList.TryGetValue(gameObject, out var instance)) return;
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
               if (!TryGetEventData(eventName, out var eventData, out var list)) return;
                
                if (gameObject != null)
                { 
                    if (!list.InstanceList.TryGetValue(gameObject, out var instance)) return;
                    instance.keyOff();
                    
                    AudioDebug.Print("KeyOff in event " + eventName + " on object " + gameObject.name);
                }
                else
                { 
                    eventData.EventInstance.keyOff();
                    AudioDebug.Print("KeyOff in event " + eventName);
                }
            }
            #endregion
            
            #region SFX

            public void PlayOneShot(string eventName, string[] paramNames = null, float[] paramValues = null,GameObject gameObject = null, bool followObject = true)
            {
                //Om event finns och är oneshot skapar vi en instans, ställer in parametrar (om de finns) och fäster ljudet på ett gameObject (om de finns), om followObject är false följer ljudet inte efter objektet (crazy)
                if (TryGetEventData(eventName, out var eventData, out _)) return;
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
                            if (gameObject.TryGetComponent<Rigidbody>(out _))
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
                            if (Vector3.Distance(gameObject.transform.position, AudioSystem.Listener.transform.position) <
                                eventData.maxDistance + 1)
                            {
                                AudioManager.Instance.occlusionChecker.CheckOcclusion(gameObject,AudioSystem.Listener, out var occlusion);
                                AudioManager.Instance.wallChecker.CheckWalls(gameObject,AudioSystem.Listener, out var walls);
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

            public void InitializeDialogue(string path)
            {
                if (!TryGetEventData(path, out var eventData, out _)) return; //Om event finns & det inte redan finns en instans skapar vi en
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
               if (!TryGetEventData(eventName, out var eventData, out _)) return; 
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
               if (!TryGetEventData(eventName, out var eventData, out _)) return;
                eventData.EventInstance.stop(STOP_MODE.ALLOWFADEOUT);
            }

            public void EndDialogue(string eventName) //Stoppa och släpp instans
            {
               if (!TryGetEventData(eventName, out var eventData, out _)) return;
                eventData.EventInstance.stop(STOP_MODE.ALLOWFADEOUT);
                eventData.EventInstance.release();
            }
            
            #endregion
                
            public void PauseAllSfx(bool paused)
            {
                if (!AudioSystem.instance.BankHandler.BankLookup.TryGetValue("Master", out var bank)) return;
            
                foreach (var bus in bank.Buses)
                {
                    bus.getPath(out var path);
                    if (path == "bus:/Sound")
                    {
                        bus.setPaused(paused);
                    }
                }

                AudioDebug.Print("Set pause to " + paused);
            }
        }
    }
