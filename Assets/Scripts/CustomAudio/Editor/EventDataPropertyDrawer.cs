using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(EventData))]
public class EventDataPropertyDrawer : PropertyDrawer
{
    public bool Show;
    public bool ShowDebug;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var eventName = property.FindPropertyRelative("eventName");
        var eventReference = property.FindPropertyRelative("eventReference");
        var banks = property.FindPropertyRelative("banks");
        var isOneShot = property.FindPropertyRelative("isOneShot");
        var is3D = property.FindPropertyRelative("is3D");
        var isDoppler = property.FindPropertyRelative("isDoppler");
        var minDistance = property.FindPropertyRelative("minDistance");
        var maxDistance = property.FindPropertyRelative("maxDistance");
        var debug = property.FindPropertyRelative("debug");
        var parameters = property.FindPropertyRelative("parameters");
        
        EditorGUI.BeginProperty(position, label, property);
        var lbl = new GUIContent()
        {
            text = eventName.stringValue,
        };
        var pos = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        
        property.isExpanded = EditorGUI.Foldout(pos, property.isExpanded, lbl);

        if (property.isExpanded)
        {
            pos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            lbl.text = "Event Name";
            EditorGUI.PropertyField(pos, eventName, lbl);
        
            pos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;
            lbl.text = "Event Reference";
        
            EditorGUI.PropertyField(pos, eventReference, lbl, true);

            if (debug.boolValue)
            {
                pos.y += EditorGUI.GetPropertyHeight(eventReference) + EditorGUIUtility.standardVerticalSpacing;
                lbl.text = "Banks";
                EditorGUI.PropertyField(pos, banks, lbl);
            
                pos.y += EditorGUI.GetPropertyHeight(banks) + EditorGUIUtility.standardVerticalSpacing;
                lbl.text = "isOneShot";
                EditorGUI.PropertyField(pos, isOneShot, lbl);
            
                pos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                lbl.text = "is3D";
                EditorGUI.PropertyField(pos, is3D, lbl);
                
                pos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                lbl.text = "minDistance";
                EditorGUI.PropertyField(pos, minDistance, lbl);
            
                pos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                lbl.text = "maxDistance";
                EditorGUI.PropertyField(pos, maxDistance, lbl);
            
                pos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                lbl.text = "Parameters";
                EditorGUI.PropertyField(pos, parameters, lbl);
            }
        }
        
        EditorGUI.EndProperty();
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var eventName = property.FindPropertyRelative("eventName");
        var eventReference = property.FindPropertyRelative("eventReference");
        var guid = property.FindPropertyRelative("guid");
        var banks = property.FindPropertyRelative("banks");
        var isOneShot = property.FindPropertyRelative("isOneShot");
        var is3D = property.FindPropertyRelative("is3D");
        var minDistance = property.FindPropertyRelative("minDistance");
        var maxDistance = property.FindPropertyRelative("maxDistance");
        var debug = property.FindPropertyRelative("debug");
        var parameters = property.FindPropertyRelative("parameters");
        
        var height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;
        if (!property.isExpanded) return height;
        
        height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;
        height += EditorGUI.GetPropertyHeight(eventReference);
        if (debug.boolValue)
        {
            height += 3 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2);
            height += 0.5f * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2);
            height += EditorGUI.GetPropertyHeight(banks) + EditorGUIUtility.standardVerticalSpacing * 2;
            height += EditorGUI.GetPropertyHeight(parameters) + EditorGUIUtility.standardVerticalSpacing * 2;
        }

        return height;
    }
}
