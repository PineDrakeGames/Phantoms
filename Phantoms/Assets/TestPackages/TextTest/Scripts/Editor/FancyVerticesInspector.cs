using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(FancyVertices))]
public class FancyVerticesInspector : PropertyDrawer
{
    SerializedProperty vertices = null;

    const int BOX_SIZE = 10;

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        vertices = property.FindPropertyRelative("m_vertices");

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var vertexRect0 = new Rect(position.x, position.y + 10, BOX_SIZE, BOX_SIZE);
        var vertexRect1 = new Rect(position.x + 30, position.y + 10, BOX_SIZE, BOX_SIZE);
        var vertexRect2 = new Rect(position.x, position.y + 40, BOX_SIZE, BOX_SIZE);
        var vertexRect3 = new Rect(position.x + 30, position.y + 40, BOX_SIZE, BOX_SIZE);


        EditorGUI.PropertyField(vertexRect0, vertices.GetArrayElementAtIndex(1), GUIContent.none);
        EditorGUI.PropertyField(vertexRect1, vertices.GetArrayElementAtIndex(2), GUIContent.none);
        EditorGUI.PropertyField(vertexRect2, vertices.GetArrayElementAtIndex(0), GUIContent.none);
        EditorGUI.PropertyField(vertexRect3, vertices.GetArrayElementAtIndex(3), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 60f;
    }
}
