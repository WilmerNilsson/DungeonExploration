#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    private SerializedProperty _banksToLoadOnStart;
    private SerializedProperty _debug;
    private SerializedProperty _showOnlyWarnings;
    private AudioManager _audioManager;
    
    public void OnEnable()
    {
        _banksToLoadOnStart = serializedObject.FindProperty("banksToLoadOnStart");
        _debug = serializedObject.FindProperty("debug");
        _showOnlyWarnings = serializedObject.FindProperty("showOnlyWarnings");
        _audioManager = (AudioManager)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.Separator();
        
        EditorGUILayout.PropertyField(_banksToLoadOnStart);
        
        EditorGUILayout.Separator();
        
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
