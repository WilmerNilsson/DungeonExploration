#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class EventDataRefresher
{
    private static EventList[] _eventLists;

    [MenuItem("Tools/Refresh Event Data")]
    public static void RefreshEventData()
    {
        _eventLists = Resources.LoadAll<EventList>("EventLists");
        Debug.Log("Refreshing data in " + _eventLists.Length + " eventlists");
        foreach (var list in _eventLists)
        {
            list.FillEventData();
        }
    }
    
    [MenuItem("Tools/Force Save All Eventlists")]
    public static void ForceSaveAllLists()
    {
        _eventLists = Resources.LoadAll<EventList>("EventLists");
        Debug.Log("Saving " + _eventLists.Length + " eventlists");
        foreach (var list in _eventLists)
        {
            list.ForceSave();
        }
    }

    
    public static EventList[] GetEventLists()
    {
        _eventLists = Resources.LoadAll<EventList>("EventLists/");
        return _eventLists;
    }
}
#endif
