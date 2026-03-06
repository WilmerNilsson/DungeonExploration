#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EventList))]
public class EventListEditor : Editor
{
    private SerializedProperty categoryProperty;
    private SerializedProperty eventsProperty;
    private SerializedProperty debugProperty;
    private EventList eventList;

    private void OnEnable()
    {
        categoryProperty = serializedObject.FindProperty("category");
        eventsProperty = serializedObject.FindProperty("events");
        debugProperty = serializedObject.FindProperty("debug");
        eventList = (EventList)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        GUILayout.Label("Event List", EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(categoryProperty, GUILayout.Height(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing));
        
        EditorGUILayout.Separator();
        
        if (GUILayout.Button("Fill Eventdata"))
        {
            eventList.FillEventData();
        }
        EditorGUILayout.PropertyField(eventsProperty);

        if (GUILayout.Button("Toggle Debug"))
        {
            eventList.ToggleDebug();
        }

        if (GUILayout.Button("Force Save"))
        {
            eventList.ForceSave();
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
