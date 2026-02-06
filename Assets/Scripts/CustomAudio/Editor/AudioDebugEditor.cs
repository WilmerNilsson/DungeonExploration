#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioDebug))]
public class AudioDebugEditor : Editor
{
    
    //ÄR DET MÖJLIGT ATT KOLLA LISTAN INSTANSER I EVENTDESCRIPTION OCH JÄMFÖRA DE I INSTANCELIST FÖR ATT SEDAN SKAPA EN COMPOSITE LIST
    
    private SerializedProperty proceduresProperty;
    private SerializedProperty pathProperty;
    private AudioDebug audioDebug;
    
    public void OnEnable()
    {
        proceduresProperty = serializedObject.FindProperty("procedure");
        pathProperty = serializedObject.FindProperty("path");
        audioDebug = (AudioDebug)target;
    }

    private string text = "";
    private int lines;
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (Application.isPlaying && AudioManager.IsValid)
        {
            EditorGUILayout.PropertyField(pathProperty);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(proceduresProperty);
            
            if (GUILayout.Button("Execute"))
            {
                text = "";
                lines = 0;
                switch (proceduresProperty.enumValueIndex)
                {
                    case 0:
                        var strings = AudioManager.Instance.GetGlobalParameterList(out var values);
                        for (int i = 0; i < strings.Length; i++)
                        {
                            lines++;
                            text += strings[i] + ": " + values[i] + "\n";
                        }
                        break;
                    case 1:
                        var instances = AudioManager.Instance.GetEventInstanceList(pathProperty.stringValue);
                        lines = 1;
                        text = pathProperty.stringValue + " has Instances on these objects:" + "\n";
                        foreach (var instance in instances)
                        {
                            lines++;
                            text += instance + "\n";
                        }
                        break;
                    case 2:
                        if (AudioManager.Instance.TryEventData(pathProperty.stringValue, out var eventData))
                        {
                            text = eventData.eventName + " has these local parameters:";
                            
                        }
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                }
            }
            
            EditorGUILayout.SelectableLabel(text, EditorStyles.textField, GUILayout.Height(
                (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * lines));
        }
        else
        {
            GUILayout.Label("Information will be displayed here when in playmode", EditorStyles.boldLabel);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
