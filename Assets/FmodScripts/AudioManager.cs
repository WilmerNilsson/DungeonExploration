using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Initialization
    
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null && Instance != this)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
        
        //LOAD BANK
        //GET VCA
        //REFRESH CACHE???
    }
    
    #endregion
    
    #region Events
    
    public EventList[] eventLists;

    public Dictionary<string, EventList> eventListCache;

    private void RefreshListCache()
    {
        eventListCache = new Dictionary<string, EventList>();
        foreach (var list in eventLists)
        {
            eventListCache.Add(list.category, list);
        }
    }

    #endregion

    #region Parameters

    #endregion

    #region Music

    #endregion

    #region SFX

    #endregion

    #region Ambience

    #endregion

    #region VCA

    #endregion

    #region Banks

    #endregion

    #region SceneLoading

    #endregion

    #region Extras

    #endregion
}
