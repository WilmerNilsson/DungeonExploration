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

    #region Parameters

    #endregion

    #region Music

    public void CreateInstance(string path)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.CreateInstance(eventName);
        }
    }

    public void ReleaseInstance(string path)
    {
        if (TryGetEventList(path, out var eventList, out var eventName))
        {
            eventList.ReleaseInstance(eventName);
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

    #region Ambience

    #endregion

    #region VCA
    
    public Dictionary<string, VCA> VcaCache { get; private set; }

    private const string MasterBankPath = "bank:/Master";

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

    #region SceneLoading

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
    }
    
    #endregion
}
