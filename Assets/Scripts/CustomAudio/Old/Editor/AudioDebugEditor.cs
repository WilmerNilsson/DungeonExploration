#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioDebug))]
public class AudioDebugEditor : Editor
{
    
    //ÄR DET MÖJLIGT ATT KOLLA LISTAN INSTANSER I EVENTDESCRIPTION OCH JÄMFÖRA DE I INSTANCELIST FÖR ATT SEDAN SKAPA EN COMPOSITE LIST
    
    private SerializedProperty proceduresProperty;
    private SerializedProperty pathProperty;
    private SerializedProperty textProperty;
    private SerializedProperty linesProperty;
    private AudioDebug audioDebug;
    
    public void OnEnable()
    {
        proceduresProperty = serializedObject.FindProperty("procedure");
        pathProperty = serializedObject.FindProperty("path");
        textProperty = serializedObject.FindProperty("text");
        linesProperty = serializedObject.FindProperty("lines");
        audioDebug = (AudioDebug)target;
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (Application.isPlaying && AudioManager.IsValid)
        {
            GUILayout.Label("AudioManager Debug Tool", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(proceduresProperty, GUIContent.none);
            EditorGUILayout.Separator();
            if (proceduresProperty.enumValueIndex is 1 or 2)
            {
                EditorGUILayout.PropertyField(pathProperty);
            }

            if (proceduresProperty.enumValueIndex != 5)
            {
                if (GUILayout.Button("Execute"))
                {
                    audioDebug.Execute();
                }
            }
            else
            {
                audioDebug.Execute();
            }
            
            EditorGUILayout.SelectableLabel(textProperty.stringValue, EditorStyles.textField, GUILayout.Height(
                (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * linesProperty.intValue));
        }
        else
        {
            GUILayout.Label("AudioManager Debug Tool", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(proceduresProperty, GUIContent.none);
            if (proceduresProperty.enumValueIndex is 1 or 2)
            {
                EditorGUILayout.PropertyField(pathProperty);
            }
            EditorGUILayout.SelectableLabel("Information will be displayed here when in play mode", EditorStyles.textField, GUILayout.Height(
                EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
