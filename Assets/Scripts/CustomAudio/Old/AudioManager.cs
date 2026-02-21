using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class AudioManager : MonoBehaviour
{
    #region Initialization

    public OcclusionChecker occlusionChecker = new();
    
    public WallChecker wallChecker = new();
    
    public static AudioManager Instance;
    
    private void Awake() //Singleton + BankLaddning + Caching
    {
        if (Instance != null && Instance != this)
        {
            AudioDebug.Print("AudioManager has an instance, destroying this one");
            Destroy(this.gameObject);
        } else 
        {
            Instance = this;
            IsValid = true;
        }
        
        DontDestroyOnLoad(this);
        
        AudioDebug.Print("AudioManager Initialized");
    }

    public static bool IsValid;

    #endregion

    #region Events

    public EventList[] eventLists;

    public Dictionary<string, EventList> EventListCache; //För snabbare lookup än foreach

    private void RefreshEventListCache() //Lägger till alla eventLists i eventListCache
    {
        EventListCache = new Dictionary<string, EventList>();
        if (eventLists == null)
        {
            AudioDebug.Print("No eventLists found, unable to add any to the eventList cache", true);
            return;
        }
        
        foreach (var list in eventLists)
        {
            EventListCache.Add(list.category, list);

            AudioDebug.Print("Added " + list.category + " to eventList cache");
        }
    }
    
    
    private void RefreshAllEventCaches() //Refreshar eventCache i alla eventLists
    {
        foreach (var eventList in eventLists)
        {
            eventList.RefreshEventCache();
        }
    }
    

    

    #endregion
    
    #region Looping Events 
    //Alla metoder här vidarebefodrar instruktioner in i rätt evenlist baserat på path.
    
    
    
    #endregion
    
    
    
    
    #region Debug

    public bool debug;

    public bool showOnlyWarnings;

    [ContextMenu("Toggle Debug")]
    public void ToggleDebug()
    {
        debug = !debug;
    }

    #endregion
    
}
