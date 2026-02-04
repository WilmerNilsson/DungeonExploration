using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioTrigger))]
public class AudioTriggerEditor : Editor
{
    private SerializedProperty _activatedByProperty;
    private SerializedProperty _tagToActivateProperty;
    private SerializedProperty _activateOnceProperty;
    private SerializedProperty _activationDelayProperty;
    private SerializedProperty _instructionsProperty;
    private AudioTrigger _audioTrigger;

    public void OnEnable()
    {
        _activatedByProperty = serializedObject.FindProperty("activatedBy");
        _tagToActivateProperty = serializedObject.FindProperty("tagToActivate");
        _activateOnceProperty = serializedObject.FindProperty("activateOnce");
        _activationDelayProperty = serializedObject.FindProperty("activationDelay");
        _instructionsProperty = serializedObject.FindProperty("instructions");
        _audioTrigger = (AudioTrigger)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        GUILayout.Label("Trigger Settings", EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(_activatedByProperty);
        if (_activatedByProperty.enumValueIndex < 2)
        {
            EditorGUILayout.PropertyField(_tagToActivateProperty);
        }

        if (_activatedByProperty.enumValueIndex < 4)
        {
            if (_activatedByProperty.enumValueIndex != 2)
            {
                EditorGUILayout.PropertyField(_activateOnceProperty);
            }
            EditorGUILayout.PropertyField(_activationDelayProperty);
        }
        
        EditorGUILayout.Separator();
        
        EditorGUILayout.PropertyField(_instructionsProperty);

        EditorGUILayout.Separator();
        
        if (GUILayout.Button("Activate"))
        {
            _audioTrigger.Activate();
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
