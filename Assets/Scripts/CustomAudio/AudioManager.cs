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

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        
        if (GameManagerSO.Instance != null)
        {
            GameManagerSO.Instance.OnFreezeGameChange += OnPauseEvent;
        }
        else
        {
            AudioDebug.Print("Couldn't find GameManagerSO", true);
        }
        
        DontDestroyOnLoad(this);
        
        LoadStartBanks();
        RefreshVcaCache();
        RefreshEventListCache();
        RefreshAllEventCaches();
        RefreshGlobalParameterCache();
        GetListener();
        CombatChecker.ResetCombatList();

        AudioDebug.Print("AudioManager Initialized");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CheckIfExists()
    {
        var go = GameObject.FindObjectsByType<AudioManager>(FindObjectsSortMode.None);
        if (go.Length < 1 || go == null)
        {
            Debug.LogWarning("AudioManager not found in scene, creating one");
            Instantiate(Resources.Load<GameObject>("AudioManager"));
        }
    }

    public static bool IsValid;

    #endregion

    #region Events

    [HideInInspector] public EventList[] eventLists;

    public Dictionary<string, EventList> EventListCache; //För snabbare lookup än foreach

    private void RefreshEventListCache() //Lägger till alla eventLists i eventListCache
    {
        eventLists = Resources.LoadAll<EventList>("EventLists/");
        
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
    #if UNITY_EDITOR
    public void FillAllEventData()
    {
        EventDataRefresher.RefreshEventData();
    }
    #endif
    
    private void RefreshAllEventCaches() //Refreshar eventCache i alla eventLists
    {
        foreach (var eventList in eventLists)
        {
            eventList.RefreshEventCache();
        }
    }
    

    public bool TryGetEventList(string path, out EventList eventList, out string eventName) //Om eventlist finns returneras true, eventListan, samt eventNamnet
    {
        if (!path.Contains("/"))
        {
            AudioDebug.Print(path + " is not a valid path", true);
            eventList = null;
            eventName = "";
            return false;
        }
        var split = path.Split('/');
        if (split.Length != 2 || split[1] == "")
        {
            AudioDebug.Print(path + " is not a valid path", true);
            eventList = null;
            eventName = "";
            return false;
        }
        if (EventListCache.TryGetValue(split[0], out eventList))
        {
            eventName = split[1];
            AudioDebug.Print("Successfully retrieved " + path);
            
            return true;
        }

        AudioDebug.Print("Failed to get " + path + ". Does the event or list exist?", true);
        
        eventName = null;
        return false;
    }

    #endregion

    #region Global Parameters

    public Dictionary<string, PARAMETER_ID> GlobalParameterCache; //För snabb lookup

    private void RefreshGlobalParameterCache() //Lägger alla globala parameterIDs i _globalParameterCache;
    {
        GlobalParameterCache = new Dictionary<string, PARAMETER_ID>();
        RuntimeManager.StudioSystem.getParameterDescriptionList(out var descriptionList);
        foreach (var paramDesc in descriptionList)
        {
            GlobalParameterCache.Add(paramDesc.name, paramDesc.id);
            AudioDebug.Print("Added " + paramDesc.name + " to global parameter cache");
        }
    }

    public void SetGlobalParameter(string paramName, float paramValue, bool printDebug = true) //Om Global parameter finns sätts vi den till paramValue;
    {
        if (GlobalParameterCache.TryGetValue(paramName, out var id))
        {
            RuntimeManager.StudioSystem.setParameterByID(id, paramValue);
            if (printDebug)
            {
                AudioDebug.Print("Successfully set " + paramName + " to " + paramValue);
            }
        }
        else
        {
            if (printDebug)
            {
                AudioDebug.Print("Failed to set " + paramName + " to " + paramValue, true);
            }
        }
    }

   
    
    #endregion
    
    #region Looping Events 
    //Alla metoder här vidarebefodrar instruktioner in i rätt evenlist baserat på path.
    
    public void CreateInstance(string path, GameObject gameObj = null, bool followObject = true)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.CreateInstance(eventName, gameObj, followObject);
        }
    }

    public void LoadSampleData(string path)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.LoadSampleData(eventName);
        }
    }
    
    public void ReleaseInstance(string path, GameObject gameObj = null)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.ReleaseInstance(eventName, gameObj);
        }
    }
    
    public void UnloadSampleData(string path)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.UnloadSampleData(eventName);
        }
    }
    
    public void StartEvent(string path, GameObject gameObj = null)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.StartEvent(eventName, gameObj);
        }
    }

    public void StopEvent(string path, STOP_MODE stopMode, GameObject gameObj = null)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.StopEvent(eventName, stopMode, gameObj);
        }
    }
    
    public void SetParameter(string path, string paramName, float paramValue, GameObject gameObj = null)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.SetParameter(eventName, paramName, paramValue, gameObj);
        }
    }

    public void KeyOff(string path, GameObject gameObj = null)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.KeyOff(eventName, gameObj);
        }
    }
    
    #endregion
    
    #region SFX

    //Samma som övre region, vidarebefodrar instruktion till eventList
    public void PlayOneShot(string path, string[] paramNames = null, float[] paramValues = null,
        GameObject gameObj = null, bool followObject = true)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.PlayOneShot(eventName, paramNames, paramValues, gameObj, followObject);
        }
    }
    
    #endregion
    
    #region VA

    public void InitializeDialogue(string path)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.InitializeDialogue(eventName);
        }
    }

    public void SayLine(string path, string lineParameter, int lineIndex)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.SayLine(eventName, lineParameter, lineIndex);
        }
    }

    public void StopLine(string path)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.StopLine(eventName);
        }
    }

    public void EndDialogue(string path)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.EndDialogue(eventName);
        }
    }
    
    #endregion
    
    #region VCA
    
    private const string MasterBankPath = "bank:/Master";

    public Dictionary<string, VCA> VcaCache; //cache med namn på vca samt VCA

    private void RefreshVcaCache() //Lägger till alla vcas till VcaCache
    { 
        VcaCache = new Dictionary<string, VCA>();
        RuntimeManager.StudioSystem.getBank(MasterBankPath, out var masterBank);
        masterBank.getVCAList(out var vcaList);
        foreach (var vca in vcaList)
        {
            vca.getPath(out var path);
            var split = path.Split('/');
            VcaCache.Add(split[^1], vca);
            
            AudioDebug.Print("Added " + split[^1] + " to vcaCache");
        }
    }

    public void SetVolume(string vcaName, float volume) //Sätter volym på en vca
    {
        if (VcaCache.TryGetValue(vcaName, out var vca))
        {
            vca.setVolume(volume);
            AudioDebug.Print("Set " + vcaName + " volume to " + volume);
        }
        else
        {
            AudioDebug.Print("Failed to set " + vcaName + " to " + volume, true);
        }
    }

    public float GetVolume(string vcaName) //Hämtar volym från vca i vcaCache
    {
        if (VcaCache.TryGetValue(vcaName, out var vca))
        {
            vca.getVolume(out var volume);
            
            AudioDebug.Print("Successfully retrieved volume for vca: " + vcaName);
            
            return volume;
        }
        
        AudioDebug.Print("Failed to get volume for vca: " + vcaName, true);
        
        return 0;
    }

    #endregion

    #region Banks

    private const string BankExtension = ".bank";
    private const string StringBankExtension = ".strings.bank";

    public void LoadBank(string bankName, bool loadSamples = false) //Laddar bank, om master laddas också string bank
    {
        RuntimeManager.LoadBank(bankName + BankExtension, loadSamples);
        if (bankName == "Master") RuntimeManager.LoadBank(bankName + StringBankExtension, loadSamples);
        AudioDebug.Print("Loading " + bankName + BankExtension);
    }

    public void UnloadBank(string bankName) //Unloadar en bank
    {
        RuntimeManager.UnloadBank(bankName + BankExtension);
        if (bankName == "Master") RuntimeManager.UnloadBank(bankName + StringBankExtension);
        AudioDebug.Print("Unloading " + bankName + BankExtension);
    }

    [Serializable]
    public struct BankToLoadOnStart
    {
        public string bankName;
        public bool loadSamples;
    }
    
    public BankToLoadOnStart[] banksToLoadOnStart =
    {
        new() { bankName = "Master", loadSamples = false },
    };

    private void LoadStartBanks()
    {
        foreach (var bank in banksToLoadOnStart)
        {
            LoadBank(bank.bankName, bank.loadSamples);
        }
    }

    #endregion
    
    #region Game States

    private void OnPauseEvent(bool paused) //Kallas av GameManagerSO och sätter parametern Paused till 
    {
        SetGlobalParameter("Paused", paused ? 1 : 0);
        SetPause(paused);
    }

    // private void FixedUpdate()
    // {
    //     foreach (var eventList in eventLists)
    //     {
    //         eventList.CheckOcclusions();
    //     }
    // }

    #endregion

    #region SceneLoading 

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GetListener();
        CombatChecker.ResetCombatList();
        OnPauseEvent(false);
    }

    private void OnSceneUnloaded(Scene scene)
    {
        CleanupInstances();
    }

    public void CleanupInstances()
    {
        foreach (var eventList in eventLists)
        {
            eventList.CleanupInstanceList(); 
        }
    }

    public static GameObject Listener;

    private static void GetListener()
    {
        var cameras = GameObject.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var camera in cameras)
        {
            if (camera.TryGetComponent(typeof(StudioListener), out var studioListener))
            {
                Listener = camera.gameObject;
                AudioDebug.Print("Successfully found listener");
                return;
            }
        }
        AudioDebug.Print("No listener found", true);
    }
    
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

    #region Quitting / Stop All

    public void StopAllEvents() //Self explanatory
    {
        RuntimeManager.StudioSystem.getBank(MasterBankPath, out var masterBank);
        masterBank.getBusList(out var busList);

        foreach (var bus in busList)
        {
            bus.stopAllEvents(STOP_MODE.ALLOWFADEOUT);
        }

        AudioDebug.Print("Stopped all events");
    }

    public void SetPause(bool paused)
    {
        RuntimeManager.StudioSystem.getBank(MasterBankPath, out var masterBank);
        masterBank.getBusList(out var busList);

        foreach (var bus in busList)
        {
            bus.getPath(out var path);
            if (path == "bus:/Sound")
            {
                bus.setPaused(paused);
            }
        }

        AudioDebug.Print("Set pause to " + paused);
    }

    public void
        StopAndReleaseAllInstances() //Typ samma som ovan men denna releasar också instanser, oklart om detta är onödigt eller ej.
    {
        foreach (var eventList in eventLists)
        {
            eventList.StopAndReleaseAllInstances();
        }

        AudioDebug.Print("Stopped and released all eventInstances");
    }
    
    private void OnApplicationQuit()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        //StopAndReleaseAllInstances();
        StopAllEvents();
    }

    #endregion
}
