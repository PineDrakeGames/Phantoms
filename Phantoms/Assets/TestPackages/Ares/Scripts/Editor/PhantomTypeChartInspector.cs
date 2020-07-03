using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PhantomTypeChart))]
public class PhantomTypeChartInspector : Editor
{
    private SerializedProperty m_attackTypes;
    private string[] m_typeNames;
    private string[] m_typeMultipliers;

    private const int COLUMN_WIDTH = 75;
    private const int ROW_HEIGHT = 30;

    private Color defaultColor;


    void OnEnable()
    {
        m_attackTypes = serializedObject.FindProperty("m_attackTypes");

        m_typeNames = System.Enum.GetNames(typeof(PhantomType));
        m_typeMultipliers = System.Enum.GetNames(typeof(TypeMultiplier));
        defaultColor = GUI.backgroundColor;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Left is attacking type, top is defending type.");
        EditorGUILayout.LabelField("Matchup enum refers to the defender - whether they are weak, immune, or resistant to the attack.");
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Types", GUILayout.Width(COLUMN_WIDTH), GUILayout.Height(ROW_HEIGHT));
        for (int i = 0; i < m_typeNames.Length; i++)
        {
            EditorGUILayout.LabelField(m_typeNames[i], GUILayout.Width(COLUMN_WIDTH), GUILayout.Height(ROW_HEIGHT));
        }
        EditorGUILayout.EndHorizontal();

        m_attackTypes.arraySize = m_typeNames.Length;

        for (int i = 0; i < m_typeNames.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(m_typeNames[i], GUILayout.Width(COLUMN_WIDTH), GUILayout.Height(ROW_HEIGHT));
            SerializedProperty attackType = m_attackTypes.GetArrayElementAtIndex(i);
            SerializedProperty attackTypeArray = attackType.FindPropertyRelative("TypeMatchups");
            attackTypeArray.arraySize = m_typeNames.Length;
            for (int j = 0; j < m_typeNames.Length; j++)
            {
                SerializedProperty defendingType = attackTypeArray.GetArrayElementAtIndex(j);
                SetColor(defendingType);
                EditorGUILayout.PropertyField(defendingType, GUIContent.none, true, GUILayout.Width(COLUMN_WIDTH), GUILayout.Height(ROW_HEIGHT));
            }
            EditorGUILayout.EndHorizontal();
        }


        serializedObject.ApplyModifiedProperties();
    }

    private void SetColor(SerializedProperty defendingType)
    {
        string type = m_typeMultipliers[defendingType.enumValueIndex];
        switch(type)
        {
            case "WEAK":
                GUI.backgroundColor = Color.green;
                break;
            case "RESISTANT":
                GUI.backgroundColor = Color.magenta;
                break;
            case "IMMUNE":
                GUI.backgroundColor = Color.cyan;
                break;
            default:
                GUI.backgroundColor = defaultColor;
                break;
            
        }
        
    }
}
