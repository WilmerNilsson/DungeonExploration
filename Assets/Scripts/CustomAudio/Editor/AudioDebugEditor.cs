#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioDebug))]
public class AudioDebugEditor : Editor
{
    private SerializedProperty globalParamListProperty;
    private AudioDebug audioDebug;
    
    public void OnEnable()
    {
        globalParamListProperty = serializedObject.FindProperty("globalParams");
        audioDebug = (AudioDebug)target;
    }

    public override void OnInspectorGUI()
    {
        if (Application.isPlaying && AudioManager.IsValid)
        {
            audioDebug.GetGlobalParamList();
            EditorGUILayout.PropertyField(globalParamListProperty);
        }
        else
        {
            GUILayout.Label("Information will be displayed here when in playmode", EditorStyles.boldLabel);
        }
    }
}
#endif
