#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class EventDataRefresher
{
    private static EventList[] _eventLists;

    [MenuItem("CustomAudio/RefreshEventData")]
    public static void RefreshEventData()
    {
        _eventLists = Resources.LoadAll<EventList>("EventLists");
        
        foreach (var list in _eventLists)
        {
            list.FillEventData();
        }
    }

    public static EventList[] GetEventLists()
    {
        _eventLists = Resources.LoadAll<EventList>("EventLists/");
        return _eventLists;
    }
}
#endif
