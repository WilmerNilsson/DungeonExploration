#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Instruction))]
public class InstructionPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var command = property.FindPropertyRelative("command");
        var path = property.FindPropertyRelative("path");
        var gameObj = property.FindPropertyRelative("gameObj");
        var followObject = property.FindPropertyRelative("followObject");
        var stopMode = property.FindPropertyRelative("stopMode");
        var parametersToSet = property.FindPropertyRelative("parametersToSet");
        var bankName = property.FindPropertyRelative("bankName");
        var loadSampleData = property.FindPropertyRelative("loadSampleData");
        
        EditorGUI.BeginProperty(position, label, property);
        var pos = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        var lbl = new GUIContent()
        {
            text = "",
        };

        var index = label.text.Substring(label.text.Length - 1, 1)[0];
        
        lbl.text = "Instruction " + index.ToString();
        
        property.isExpanded = EditorGUI.Foldout(pos, property.isExpanded, lbl);

        if (property.isExpanded)
        {
            pos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            lbl.text = "Command";
            EditorGUI.PropertyField(pos, command, lbl);

            if (command.enumValueIndex < 9)
            {
                pos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                lbl.text = "Path";
                EditorGUI.PropertyField(pos, path, lbl);

                if (command.enumValueIndex != 1 && command.enumValueIndex != 5)
                {
                    pos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;
                    lbl.text = "Game Object";
                    EditorGUI.PropertyField(pos, gameObj, lbl);
                }

                if (command.enumValueIndex == 0 || command.enumValueIndex == 8)
                {
                    pos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    lbl.text = "Follow Object";
                    EditorGUI.PropertyField(pos, followObject, lbl);
                }
            }

            if (command.enumValueIndex == 3)
            {
                pos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;
                lbl.text = "Stop Mode";
                EditorGUI.PropertyField(pos, stopMode, lbl);
            }

            if (command.enumValueIndex == 6 || command.enumValueIndex == 8 || command.enumValueIndex == 9)
            {
                pos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                lbl.text = "Parameters to Set";
                EditorGUI.PropertyField(pos, parametersToSet, lbl);
            }
            
            if (command.enumValueIndex > 9)
            {
                pos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                lbl.text = "Bank Name";
                EditorGUI.PropertyField(pos, bankName, lbl);
                if (command.enumValueIndex == 10)
                {
                    pos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;
                    lbl.text = "Load Sample Data";
                    EditorGUI.PropertyField(pos, loadSampleData, lbl);
                }
            }
        
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var command = property.FindPropertyRelative("command");
        var path = property.FindPropertyRelative("path");
        var gameObj = property.FindPropertyRelative("gameObj");
        var attachToObject = property.FindPropertyRelative("attachToObject");
        var followObject = property.FindPropertyRelative("followObject");
        var stopMode = property.FindPropertyRelative("stopMode");
        var parametersToSet = property.FindPropertyRelative("parametersToSet");
        var bankName = property.FindPropertyRelative("bankName");
        
        var height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if (!property.isExpanded) return height;
        
        height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
        if (command.enumValueIndex < 9)
        {
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;

            if (command.enumValueIndex != 1 && command.enumValueIndex != 5)
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;
            }

            if (command.enumValueIndex == 0 || command.enumValueIndex == 8)
            {
                height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            }
        }

        if (command.enumValueIndex == 3)
        {
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        if (command.enumValueIndex == 6 || command.enumValueIndex == 8 || command.enumValueIndex == 9)
        {
            height += EditorGUI.GetPropertyHeight(parametersToSet);
        }
        
        if (command.enumValueIndex > 9)
        {
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;
            if (command.enumValueIndex == 10)
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;
            }
        }
        
        return height;
    }
}
#endif