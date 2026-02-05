using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleItem))]
public class SimpleItemEditor : Editor
{
    public bool showLevels = true;
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SimpleItem item = (SimpleItem)target;
        EditorGUILayout.Space ();
        
        showLevels = EditorGUILayout.Foldout (showLevels, "Item grid space ("+item.itemGridSize.Length+")");
        
        if (showLevels)
        {
            EditorGUI.indentLevel++;
            
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < 4; x++)
            {
                EditorGUILayout.BeginVertical();
                for (int y = 0; y < 4; y++)
                {
                    item.itemGridSize[x, y] = EditorGUILayout.Toggle(item.itemGridSize[x, y]);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        // input = new int[4][];
        // itemSize = new int[4][];
        
        // Rect r = EditorGUILayout.BeginVertical();
        //
        // EditorGUILayout.BeginHorizontal();
        // itemSize[0][0] = EditorGUILayout.IntField(input[0][0]);
        // itemSize[0][1] = EditorGUILayout.IntField(input[0][1]);
        // itemSize[0][2] = EditorGUILayout.IntField(input[0][2]);
        // itemSize[0][3] = EditorGUILayout.IntField(input[0][3]);
        // EditorGUILayout.EndHorizontal();
        //
        // EditorGUILayout.BeginHorizontal();
        // itemSize[1][0] = EditorGUILayout.IntField(input[1][0]);
        // itemSize[1][1] = EditorGUILayout.IntField(input[1][1]);
        // itemSize[1][2] = EditorGUILayout.IntField(input[1][2]);
        // itemSize[1][3] = EditorGUILayout.IntField(input[1][3]);
        // EditorGUILayout.EndHorizontal();
        //
        // EditorGUILayout.BeginHorizontal();
        // itemSize[2][0] = EditorGUILayout.IntField(input[2][0]);
        // itemSize[2][1] = EditorGUILayout.IntField(input[2][1]);
        // itemSize[2][2] = EditorGUILayout.IntField(input[2][2]);
        // itemSize[2][3] = EditorGUILayout.IntField(input[2][3]);
        // EditorGUILayout.EndHorizontal();
        //
        // EditorGUILayout.BeginHorizontal();
        // itemSize[3][0] = EditorGUILayout.IntField(input[3][0]);
        // itemSize[3][1] = EditorGUILayout.IntField(input[3][1]);
        // itemSize[3][2] = EditorGUILayout.IntField(input[3][2]);
        // itemSize[3][3] = EditorGUILayout.IntField(input[3][3]);
        // EditorGUILayout.EndHorizontal();
        //
        // EditorGUILayout.EndVertical();
        // SerializedProperty wha = serializedObject.FindProperty("wha");
        // EditorGUI.BeginProperty(r, GUIContent.none, wha);
    }
}
