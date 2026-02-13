#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    private SerializedProperty eventLists;
    private SerializedProperty banksToLoadOnStart;
    private SerializedProperty debug;
    private SerializedProperty showOnlyWarnings;
    private AudioManager audioManager;
    private SerializedProperty occlusionChecker;
    
    public void OnEnable()
    {
        eventLists = serializedObject.FindProperty("eventLists");
        banksToLoadOnStart = serializedObject.FindProperty("banksToLoadOnStart");
        debug = serializedObject.FindProperty("debug");
        showOnlyWarnings = serializedObject.FindProperty("showOnlyWarnings");
        audioManager = (AudioManager)target;
        occlusionChecker = serializedObject.FindProperty("occlusionChecker");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(eventLists);

        if (GUILayout.Button("Fill Eventdata"))
        {
            audioManager.FillAllEventData();
        }
        
        EditorGUILayout.Separator();
        
        EditorGUILayout.PropertyField(banksToLoadOnStart);
        
        EditorGUILayout.Separator();
        
        EditorGUILayout.PropertyField(occlusionChecker);
        
        EditorGUILayout.Separator();
        
        if (GUILayout.Button("Toggle Debug"))
        {
            audioManager.ToggleDebug();
        }

        if (debug.boolValue)
        {
            EditorGUILayout.Separator();
            
            EditorGUILayout.PropertyField(debug);
            EditorGUILayout.PropertyField(showOnlyWarnings);
            
        }
        
        
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
