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
        RefreshListCache();
        RefreshGlobalParameterCache();
    }

    #endregion

    #region Events

    public EventList[] eventLists;

    private Dictionary<string, EventList> _eventListCache;

    private void RefreshListCache()
    {
        _eventListCache = new Dictionary<string, EventList>();
        foreach (var list in eventLists)
        {
            _eventListCache.Add(list.category, list);
        }
    }

    private bool TryGetEventList(string path, out EventList eventList, out string eventName)
    {
        var split = path.Split('/');
        if (_eventListCache.TryGetValue(split[0], out eventList))
        {
            eventName = split[1];
            return true;
        }
        eventName = null;
        return false;
    }

    #endregion

    #region Global Parameters

    public Dictionary<string, PARAMETER_ID> GlobalParameterCache;

    private void RefreshGlobalParameterCache()
    {
        GlobalParameterCache = new Dictionary<string, PARAMETER_ID>();
        RuntimeManager.StudioSystem.getParameterDescriptionList(out var descriptionList);
        foreach (var paramDesc in descriptionList)
        {
            Debug.Log(paramDesc.name);
            GlobalParameterCache.Add(paramDesc.name, paramDesc.id);
        }
    }

    public void SetGlobalParameter(string paramName, float paramValue)
    {
        if (GlobalParameterCache.TryGetValue(paramName, out PARAMETER_ID id))
        {
            RuntimeManager.StudioSystem.setParameterByID(id, paramValue);
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
    
    public void CreateInstance(string path, GameObject gameObject = null, bool attachToObject = false, bool followObject = true)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.CreateInstance(eventName, gameObject, attachToObject, followObject);
        }
    }
    
    public void ReleaseInstance(string path, GameObject gameObject = null)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.ReleaseInstance(eventName, gameObject);
        }
    }
    
    public void StartEvent(string path, GameObject gameObject = null)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.StartEvent(eventName, gameObject);
        }
    }

    public void StopEvent(string path, STOP_MODE stopMode, GameObject gameObject = null)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.StopEvent(eventName, stopMode, gameObject);
        }
    }
    
    public void SetParameter(string path, string paramName, float paramValue, GameObject gameObject = null)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.SetParameter(eventName, paramName, paramValue, gameObject);
        }
    }

    public void KeyOff(string path, GameObject gameObject = null)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.KeyOff(eventName, gameObject);
        }
    }
    
    #endregion
    
    #region SFX

    public void PlayOneShot(string path, string[] paramNames = null, float[] paramValues = null,
        GameObject gameObj = null)
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
        }
    }

    public void SetVolume(string vcaName, float volume)
    {
        if (VcaCache.TryGetValue(vcaName, out var vca))
        {
            vca.setVolume(volume);
        }
    }

    public float GetVolume(string vcaName)
    {
        if (VcaCache.TryGetValue(vcaName, out var vca))
        {
            vca.getVolume(out var volume);
            return volume;
        }
        
        return 0;
    }

    public void SetAllToVolume(float volume)
    {
        foreach (var vca in VcaCache)
        {
            vca.Value.setVolume(volume);
        }
    }

    public void SetAllVolumes(string[] vcaNames, float[] volumes)
    {
        for (int i = 0; i < vcaNames.Length; i++)
        {
            if (VcaCache.TryGetValue(vcaNames[i], out var vca))
            {
                vca.setVolume(volumes[i]);
            }
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
    }

    public void UnloadBank(string bankName)
    {
        RuntimeManager.UnloadBank(bankName + BankExtension);
        if (bankName == "Master") RuntimeManager.UnloadBank(bankName + StringBankExtension);
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
    
    #region Reverb
    
    //REVERB STUFF HERE
    
    #endregion
    
    #region Debug //TODO STUFF HERE

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
    }
    
    #endregion
}
