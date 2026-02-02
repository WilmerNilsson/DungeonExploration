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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        } else 
        {
            Instance = this;
        }
        
        DontDestroyOnLoad(this);
        
        LoadStartBanks();
        RefreshVcaCache();
        RefreshEventListCache();
        RefreshAllEventCaches();
        RefreshGlobalParameterCache();

        if (debug && !showOnlyWarnings)
        {
            Debug.Log("AudioManager Initialized");
        }
    }

    #endregion

    #region Events

    public EventList[] eventLists;

    private Dictionary<string, EventList> _eventListCache;

    private void RefreshEventListCache()
    {
        _eventListCache = new Dictionary<string, EventList>();
        if (eventLists == null)
        {
            if (debug)
            {
                Debug.LogWarning("No eventLists found, unable to add any to the eventList cache");
            }
            return;
        }
        
        foreach (var list in eventLists)
        {
            _eventListCache.Add(list.category, list);

            if (debug && !showOnlyWarnings)
            {
                Debug.Log("Added " + list.category + " to eventList cache");
            }
        }
    }

    private void RefreshAllEventCaches()
    {
        foreach (var eventList in eventLists)
        {
            eventList.RefreshEventCache();
        }
    }

    private bool TryGetEventList(string path, out EventList eventList, out string eventName)
    {
        var split = path.Split('/');
        if (_eventListCache.TryGetValue(split[0], out eventList))
        {
            eventName = split[1];
            if (debug && !showOnlyWarnings)
            {
                Debug.Log("Successfully retrieved " + path);
            }
            
            return true;
        }

        if (debug)
        {
            Debug.LogWarning("Failed to get " + path + ". Does the event or list exist?");
        }
        
        eventName = null;
        return false;
    }

    #endregion

    #region Global Parameters

    private Dictionary<string, PARAMETER_ID> _globalParameterCache;

    private void RefreshGlobalParameterCache()
    {
        _globalParameterCache = new Dictionary<string, PARAMETER_ID>();
        RuntimeManager.StudioSystem.getParameterDescriptionList(out var descriptionList);
        foreach (var paramDesc in descriptionList)
        {
            Debug.Log(paramDesc.name);
            _globalParameterCache.Add(paramDesc.name, paramDesc.id);

            if (debug && !showOnlyWarnings)
            {
                Debug.Log("Added " + paramDesc.name + " to parameter cache");
            }
        }
    }

    public void SetGlobalParameter(string paramName, float paramValue)
    {
        if (_globalParameterCache.TryGetValue(paramName, out var id))
        {
            RuntimeManager.StudioSystem.setParameterByID(id, paramValue);

            if (debug && !showOnlyWarnings)
            {
                Debug.Log("Successfully set " + paramName + " to " + paramValue);
            }
        }
        else
        {
            if (debug)
            {
                Debug.LogWarning("Failed to set " + paramName + " to " + paramValue);
            }
        }
    }
    
    #endregion
    
    #region Looping Events

    public void ResetInstanceList(string category)
    {
        if (_eventListCache.TryGetValue(category, out var eventList))
        {
            eventList.ResetInstanceList();
        }
    }
    
    public void CreateInstance(string path, GameObject gameObj = null, bool attachToObject = false, bool followObject = true)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.CreateInstance(eventName, gameObj, attachToObject, followObject);
        }
    }
    
    public void ReleaseInstance(string path, GameObject gameObj = null)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.ReleaseInstance(eventName, gameObj);
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

    public void PlayOneShot(string path, string[] paramNames = null, float[] paramValues = null,
        GameObject gameObj = null, bool followObject = true)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.PlayOneShot(eventName, paramNames, paramValues, gameObj);
        }
    }
    
    #endregion
    
    #region VA
    
    #endregion
    
    #region VCA
    
    private const string MasterBankPath = "bank:/Master";
    
    public Dictionary<string, VCA> VcaCache { get; private set; }

    private void RefreshVcaCache()
    { 
        VcaCache = new Dictionary<string, VCA>();
        RuntimeManager.StudioSystem.getBank(MasterBankPath, out var masterBank);
        masterBank.getVCAList(out var vcaList);
        foreach (var vca in vcaList)
        {
            vca.getPath(out var path);
            var split = path.Split('/');
            VcaCache.Add(split[^1], vca);
            
            if (debug && !showOnlyWarnings)
            {
                Debug.Log("Added " + split[^1] + " to vcaCache");
            }
        }
    }

    public void SetVolume(string vcaName, float volume)
    {
        if (VcaCache.TryGetValue(vcaName, out var vca))
        {
            vca.setVolume(volume);
            if (debug && !showOnlyWarnings)
            {
                Debug.Log("Set " + vcaName + " volume to " + volume);
            }
        }
    }

    public float GetVolume(string vcaName)
    {
        if (VcaCache.TryGetValue(vcaName, out var vca))
        {
            vca.getVolume(out var volume);
            
            if (debug && !showOnlyWarnings)
            {
                Debug.Log("Successfully retrieved volume for vca: " + vcaName);
            }
            
            return volume;
        }
        
        if (debug)
        {
            Debug.LogWarning("Failed to get volume for vca: " + vcaName);
        }
        
        return 0;
    }

    public void SetAllToVolume(float volume)
    {
        foreach (var vca in VcaCache)
        {
            vca.Value.setVolume(volume);
        }
        if (debug && !showOnlyWarnings)
        {
            Debug.Log("Set volume for all VCAs to " + volume);
        }
    }

    public void SetAllVolumes(string[] vcaNames, float[] volumes)
    {
        for (int i = 0; i < vcaNames.Length; i++)
        {
            SetVolume(vcaNames[i], volumes[i]);
        }
    }

    public void GetAllVolumes(out string[] vcaNames, out float[] volumes)
    {
        var tempNameList = new List<string>();
        var tempVolumeList = new List<float>();
        foreach (var vca in VcaCache)
        {
            tempNameList.Add(vca.Key);
            vca.Value.getVolume(out var volume);
            tempVolumeList.Add(volume);
            if (debug && !showOnlyWarnings)
            {
                Debug.Log("Successfully retrieved volume for " + vca.Key);
            }
        }
        vcaNames = tempNameList.ToArray();
        volumes = tempVolumeList.ToArray();
    }

    #endregion

    #region Banks

    private const string BankExtension = ".bank";
    private const string StringBankExtension = ".strings.bank";

    public void LoadBank(string bankName)
    {
        RuntimeManager.LoadBank(bankName + BankExtension);
        if (bankName == "Master") RuntimeManager.LoadBank(bankName + StringBankExtension);
        if (debug && !showOnlyWarnings)
        {
            Debug.Log("Loading " + bankName + BankExtension);
        }
    }

    public void UnloadBank(string bankName)
    {
        RuntimeManager.UnloadBank(bankName + BankExtension);
        if (bankName == "Master") RuntimeManager.UnloadBank(bankName + StringBankExtension);
        if (debug && !showOnlyWarnings)
        {
            Debug.Log("Unloading " + bankName + BankExtension);
        }
    }

    public string[] banksToLoadOnStart = { "Master" };

    private void LoadStartBanks()
    {
        foreach (var bank in banksToLoadOnStart)
        {
            LoadBank(bank);
        }
    }

    #endregion

    #region SceneLoading //TODO: STUFF HERE

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }

    private void OnSceneUnloaded(Scene scene)
    {
        
    }
    
    #endregion
    
    #region Debug

    public bool debug;

    public bool showOnlyWarnings;

    #endregion

    #region Extras

    public void StopAllEvents()
    {
        RuntimeManager.StudioSystem.getBank(MasterBankPath, out var masterBank);
        masterBank.getBusList(out var busList);

        foreach (var bus in busList)
        {
            bus.stopAllEvents(STOP_MODE.ALLOWFADEOUT);
        }

        if (debug && !showOnlyWarnings)
        {
            Debug.Log("Stopped all events");
        }
    }

    public void StopAndReleaseAllInstances()
    {
        foreach (var eventList in eventLists)
        {
            eventList.StopAndReleaseAllInstances();
        }
    }
    
    #endregion
}
