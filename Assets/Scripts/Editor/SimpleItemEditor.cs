#if (UNITY_EDITOR)

using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

[CustomEditor(typeof(SimpleItem))]
public class SimpleItemEditor : Editor
{
    public bool showLevels = true;
    private SimpleItem item;

    private SerializedProperty Bools;
    
    public bool[] itemGrid;
    private void OnEnable()
    {
        item = (SimpleItem)target;
        Bools = serializedObject.FindProperty("itemGridSize");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        
        EditorGUILayout.Space ();
        
        showLevels = EditorGUILayout.Foldout (showLevels, "Item grid space ("+item.itemGridSize.Length+")");
        
        if (showLevels)
        {
            EditorGUI.indentLevel++;
            
            EditorGUILayout.BeginVertical();
            for (int x = 0; x < 4; x++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int y = 0; y < 4; y++)
                {
                    EditorGUILayout.PropertyField(Bools.GetArrayElementAtIndex(x + 4*y), GUIContent.none, GUILayout.Width(20));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("test"))
        {
            string test = "";
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    test += item.itemGridSize[x + 4*y];
                    test += " ";
                }
                test += "\n";
            }
            Debug.Log(test);
        }
        
        serializedObject.ApplyModifiedProperties();
        
    }
}

#endif
