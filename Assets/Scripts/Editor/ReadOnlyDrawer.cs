using UnityEngine;
using UnityEditor;

public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // save previous GUI enabled value
        var perviousGUIState = GUI.enabled;

        // disable edit for property
        GUI.enabled = false;

        // draw property
        EditorGUI.PropertyField(position, property, label);

        // setting old GUI enabled value
        GUI.enabled = perviousGUIState;
    }
}
