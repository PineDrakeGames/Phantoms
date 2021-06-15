using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootFlower : MonoBehaviour
{
    [SerializeField]
    private Animator m_root = null;

    [SerializeField]
    private Animator m_flower = null;

    [SerializeField]
    private bool m_saveTrigger = false;
    [SerializeField]
    private string m_triggerID = string.Empty;

    private bool m_triggered = false;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(m_triggerID))
        {
            m_triggerID = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name + "_" + gameObject.name;
        }
    }
#endif

    private void Awake()
    {
        m_triggered = false;
        if (m_saveTrigger)
        {
            m_triggered = SaveDataManager.CheckFlag(m_triggerID);
        }

        Initialize();
    }

    private void OnEnable()
    {
        Initialize();
    }

    public void OnRootHit()
    {
        if (!m_triggered)
        {
            m_triggered = true;
            m_root.SetBool("On", false);
            m_flower.SetBool("On", true);
            if (m_saveTrigger)
            {
                SaveDataManager.SetFlag(m_triggerID);
            }
        }
    }

    private void Initialize()
    {
        m_root.SetBool("On", !m_triggered);
        m_root.SetTrigger("Reset");
        m_flower.SetBool("On", m_triggered);
        m_flower.SetTrigger("Reset");
    }
}
