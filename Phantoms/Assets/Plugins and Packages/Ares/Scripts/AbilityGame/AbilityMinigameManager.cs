using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;


[CustomEditor(typeof(AbilityMinigameManager))]
public class AbilityMinigameManagerEditor : Editor
{
    private SerializedProperty m_abilityMinigames;
    private string[] m_minigameTypes;

    void OnEnable()
    {
        m_abilityMinigames = serializedObject.FindProperty("m_abilityMinigames");

        m_minigameTypes = System.Enum.GetNames(typeof(AbilityMinigameType));

        m_abilityMinigames.arraySize = m_minigameTypes.Length;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (m_abilityMinigames.arraySize != m_minigameTypes.Length)
        {
            m_abilityMinigames.arraySize = m_minigameTypes.Length;
        }
        for (int i = 0; i < m_minigameTypes.Length; i++)
        {
            SerializedProperty minigame = m_abilityMinigames.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(minigame, new GUIContent(m_minigameTypes[i]));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif

public class AbilityMinigameManager : MonoBehaviour
{
    [SerializeField]
    private List<AbilityMinigame> m_abilityMinigames = null;


    private static AbilityMinigameManager s_instance = null;
    public static AbilityMinigameManager Instance
    {
        get { return s_instance; }
    }

    private Dictionary<AbilityMinigameType, AbilityMinigame> m_typeToMinigame = new Dictionary<AbilityMinigameType, AbilityMinigame>();
    private AbilityMinigame m_currentMinigame = null;
    public static AbilityMinigame CurrentMinigame { get { return s_instance.m_currentMinigame; } }

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }

        AbilityMinigameType[] minigameTypes = System.Enum.GetValues(typeof(AbilityMinigameType)) as AbilityMinigameType[];
        for (int i = 0; i < minigameTypes.Length; i++)
        {
            m_typeToMinigame.Add(minigameTypes[i], m_abilityMinigames[i]);
            if (m_abilityMinigames[i] != null)
            {
                m_abilityMinigames[i].gameObject.SetActive(false);
            }
        }
    }

    public static bool HasMinigame(AbilityMinigameType type)
    {
        return (s_instance.m_typeToMinigame.ContainsKey(type) && (s_instance.m_typeToMinigame[type] != null));
    }

    public static void StartMinigame(AbilityMinigameData data)
    {
        s_instance.StartMinigameInternal(data);
    }

    private void StartMinigameInternal(AbilityMinigameData data)
    {
        m_currentMinigame = m_typeToMinigame[data.type];
        switch (data.type)
        {
            case AbilityMinigameType.ANGER:
                try
                {
                    (m_currentMinigame as ButtonMashMinigame).Data = (ButtonMashMinigameData)data;
                }
                catch
                {
                    Debug.LogError("Minigame data is not valid!");
                }
                break;
            case AbilityMinigameType.JOY_METER:
                try
                {
                    (m_currentMinigame as TimingMeterMinigame).Data = (TimingMeterMinigameData)data;
                }
                catch
                {
                    Debug.LogError("Minigame data is not valid!");
                }
                break;
            case AbilityMinigameType.FEAR:
                try
                {
                    (m_currentMinigame as BulletDodgeMinigame).Data = (BulletDodgeMinigameData)data;
                }
                catch
                {
                    Debug.LogError("Minigame data is not valid!");
                }
                break;
            default:
                Debug.Log("No data.");
                break;
        }
        m_currentMinigame.gameObject.SetActive(true);
        m_currentMinigame.StartMinigame();
    }
}
