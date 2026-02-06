#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioDebug))]
public class AudioDebugEditor : Editor
{
    
    //ÄR DET MÖJLIGT ATT KOLLA LISTAN INSTANSER I EVENTDESCRIPTION OCH JÄMFÖRA DE I INSTANCELIST FÖR ATT SEDAN SKAPA EN COMPOSITE LIST
    
    private SerializedProperty globalParamListProperty;
    private AudioDebug audioDebug;
    
    public void OnEnable()
    {
        globalParamListProperty = serializedObject.FindProperty("globalParams");
        audioDebug = (AudioDebug)target;
    }

    private string text = "test";
    private string path = "";
    private int lines;
    
    public override void OnInspectorGUI()
    {
        if (Application.isPlaying && AudioManager.IsValid)
        {
            path = EditorGUILayout.TextField("Path", path);
            
            if (GUILayout.Button("Get Global Parameter List"))
            {
                text = "";
                lines = 0;
                var strings = AudioManager.Instance.GetGlobalParameterList(out var values);
                for (int i = 0; i < strings.Length; i++)
                {
                    lines++;
                    text += strings[i] + ": " + values[i] +  "\n";
                }
            }

            if (GUILayout.Button("Get EventInstance List"))
            {
                var strings = AudioManager.Instance.GetEventInstanceList(path);
                text = path + " has " + strings.Length + " instances on these objects:" + "\n";
                lines = 0;
                foreach (var name in strings)
                {
                    lines++;
                    text += name + "\n";
                }
            }
            
            EditorGUILayout.SelectableLabel(text, EditorStyles.textField, GUILayout.Height(
                (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * lines));
        }
        else
        {
            GUILayout.Label("Information will be displayed here when in playmode", EditorStyles.boldLabel);
        }
    }
}
#endif
