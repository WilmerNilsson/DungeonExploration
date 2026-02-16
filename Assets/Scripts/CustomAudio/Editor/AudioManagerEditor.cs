#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    private SerializedProperty _eventLists;
    private SerializedProperty _banksToLoadOnStart;
    private SerializedProperty _debug;
    private SerializedProperty _showOnlyWarnings;
    private AudioManager _audioManager;
    private SerializedProperty _occlusionChecker;
    
    public void OnEnable()
    {
        _eventLists = serializedObject.FindProperty("eventLists");
        _banksToLoadOnStart = serializedObject.FindProperty("banksToLoadOnStart");
        _debug = serializedObject.FindProperty("debug");
        _showOnlyWarnings = serializedObject.FindProperty("showOnlyWarnings");
        _audioManager = (AudioManager)target;
        _occlusionChecker = serializedObject.FindProperty("occlusionChecker");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(_eventLists);

        if (GUILayout.Button("Fill Eventdata"))
        {
            _audioManager.FillAllEventData();
        }
        
        EditorGUILayout.Separator();
        
        EditorGUILayout.PropertyField(_banksToLoadOnStart);
        
        EditorGUILayout.Separator();
        
        EditorGUILayout.PropertyField(_occlusionChecker);
        
        EditorGUILayout.Separator();
        
        if (GUILayout.Button("Toggle Debug"))
        {
            _audioManager.ToggleDebug();
        }

        if (_debug.boolValue)
        {
            EditorGUILayout.Separator();
            
            EditorGUILayout.PropertyField(_debug);
            EditorGUILayout.PropertyField(_showOnlyWarnings);
            
        }
        
        
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
