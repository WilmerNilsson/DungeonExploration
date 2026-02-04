using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleItem))]
public class SimpleItemEditor : Editor
{
    private int[][] itemSize = new int[4][];
    public override void OnInspectorGUI()
    {
        Rect r = EditorGUILayout.BeginVertical();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.IntField(itemSize[0][0]);
        EditorGUILayout.IntField(itemSize[0][1]);
        EditorGUILayout.IntField(itemSize[0][2]);
        EditorGUILayout.IntField(itemSize[0][3]);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.IntField(itemSize[1][0]);
        EditorGUILayout.IntField(itemSize[1][1]);
        EditorGUILayout.IntField(itemSize[1][2]);
        EditorGUILayout.IntField(itemSize[1][3]);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.IntField(itemSize[2][0]);
        EditorGUILayout.IntField(itemSize[2][1]);
        EditorGUILayout.IntField(itemSize[2][2]);
        EditorGUILayout.IntField(itemSize[2][3]);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.IntField(itemSize[3][0]);
        EditorGUILayout.IntField(itemSize[3][1]);
        EditorGUILayout.IntField(itemSize[3][2]);
        EditorGUILayout.IntField(itemSize[3][3]);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
    }
}
