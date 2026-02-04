using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    private SerializedProperty eventLists;
    private SerializedProperty banksToLoadOnStart;
    private SerializedProperty debug;
    private SerializedProperty showOnlyWarnings;
    private SerializedProperty showExtraInfo;
    private AudioManager audioManager;
    
    
    public void OnEnable()
    {
        eventLists = serializedObject.FindProperty("eventLists");
        banksToLoadOnStart = serializedObject.FindProperty("banksToLoadOnStart");
        debug = serializedObject.FindProperty("debug");
        showOnlyWarnings = serializedObject.FindProperty("showOnlyWarnings");
        showExtraInfo = serializedObject.FindProperty("showExtraInfo");
        audioManager = (AudioManager)target;
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
        
        if (GUILayout.Button("Toggle Debug"))
        {
            audioManager.ToggleDebug();
        }

        if (debug.boolValue)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(debug);
            EditorGUILayout.PropertyField(showOnlyWarnings);
            EditorGUILayout.PropertyField(showExtraInfo);
            
            EditorGUILayout.Separator();

            if (showExtraInfo.boolValue)
            {
                GUILayout.Label("VCAs:", EditorStyles.boldLabel);
                if (Application.isPlaying)
                {
                    foreach (var vca in AudioManager.Instance.VcaCache)
                    {
                        EditorGUILayout.SelectableLabel(vca.Key, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    }
                }
                EditorGUILayout.Separator();
                
                GUILayout.Label("Global Parameters:", EditorStyles.boldLabel);
                if (Application.isPlaying)
                {
                    foreach (var param in AudioManager.Instance._globalParameterCache)
                    {
                        EditorGUILayout.SelectableLabel(param.Key, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    }
                }
            }
        }
        
        
        serializedObject.ApplyModifiedProperties();
    }
}
