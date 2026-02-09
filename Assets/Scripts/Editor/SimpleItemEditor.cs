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

    private const int gridWidth = SimpleItem.GridWidth;
    private const int gridHeight = SimpleItem.GridHeight;


    public bool[] itemGrid;
    private void OnEnable()
    {
        item = (SimpleItem)target;
        Bools = serializedObject.FindProperty("itemGridSize");
    }

    private int GetIndexAtPoint(int x, int y)
    {
        return x + gridWidth * y;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        
        EditorGUILayout.Space ();
        
        showLevels = EditorGUILayout.Foldout (showLevels, $"Item grid space ({item.itemGridSize.Length})");
        
        if (showLevels)
        {
            EditorGUI.indentLevel++;
            
            //we begin with the vertical first row at y=0, then we do another row at y=1 etc
            EditorGUILayout.BeginVertical();
            for (int y = 0; y < gridHeight; y++)
            {
                //each row has a width and a element is inserted for each x
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < gridWidth; x++)
                {
                    //the index is then in order
                    //0123
                    //4567
                    //each y thus increases the index by 4 (in the example, the const int gridWidth in general)
                    //int index = x + gridWidth * y;

                    //we do however want to invert the y
                    int invY = (gridHeight-1) - y;

                    int index = x + gridWidth * invY;

                    EditorGUILayout.PropertyField(Bools.GetArrayElementAtIndex(index), GUIContent.none, GUILayout.Width(20f));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("test"))
        {
            bool[,] tempSize = new bool[gridWidth, gridHeight];

            bool[,] xxNxo = { { true, true },
                                  { false, true} };

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    tempSize[x, y] = item.itemGridSize[GetIndexAtPoint(x, y)];

                    if(tempSize[x, y])
                    {
                        Debug.Log($"{x},{y} is, it is the {GetIndexAtPoint(x, y)} item");
                    }
                }
            }

            if(true)
            {
                for (int y = 0; y < 2; y++)
                {
                    for (int x = 0; x < 2; x++)
                    {
                        Debug.Log($"{x},{y} is {tempSize[x, y]}, match? {tempSize[x, y] == xxNxo[x, y]}");
                    }
                }
            }


        }
        
        serializedObject.ApplyModifiedProperties();
        
    }
}

#endif
