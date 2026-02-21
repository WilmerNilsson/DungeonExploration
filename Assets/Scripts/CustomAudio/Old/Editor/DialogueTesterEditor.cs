#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueTester))]
public class DialogueTesterEditor : Editor
{
    private SerializedProperty path;
    private SerializedProperty lineParamenter;
    private SerializedProperty lineIndex;
    private DialogueTester dialogueTester;

    public void OnEnable()
    {
        path = serializedObject.FindProperty("path");
        lineParamenter = serializedObject.FindProperty("lineParameter");
        lineIndex = serializedObject.FindProperty("lineIndex");
        dialogueTester = (DialogueTester)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(path);
        EditorGUILayout.PropertyField(lineParamenter);
        EditorGUILayout.PropertyField(lineIndex);
        
        EditorGUILayout.Separator();

        if (GUILayout.Button("Initialize"))
        {
            dialogueTester.InitializeDialogue();
        }

        if (GUILayout.Button("Say Line"))
        {
            dialogueTester.SayLine();
        }

        if (GUILayout.Button("Stop Line"))
        {
            dialogueTester.StopLine();
        }

        if (GUILayout.Button("End Dialogue"))
        {
            dialogueTester.EndDialogue();
        }
        
        
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
