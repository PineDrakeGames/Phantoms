using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootFlower : MonoBehaviour
{
    [System.Serializable]
    private class Root
    {
        public AttackTarget rootTarget = null;
        public Animator rootAnimator = null;
        [HideInInspector]
        public bool Hit = false;
    }
    
    [Header("Settings")]
    [SerializeField]
    private bool m_requireAllRoots = false;

    [Header("Flowers and Roots")]
    [SerializeField]
    private Root[] m_roots = null;

    [SerializeField]
    private Animator[] m_flowers = null;

    [Header("Save Settings")]
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

    public void OnRootHit(AttackTarget attackedRoot)
    {
        if (!m_triggered)
        {
            // If we require all the roots for this, check if all the roots are hit - and 
            // return early if not.
            if (m_requireAllRoots)
            {
                bool hitAllRoots = true;
                foreach (Root root in m_roots)
                {
                    if (root.rootTarget == attackedRoot)
                    {
                        root.rootAnimator.SetBool("On", false);
                        root.Hit = true;
                    }
                    else if (!root.Hit)
                    {
                        hitAllRoots = false;
                    }
                }
                if (!hitAllRoots)
                {
                    return;
                }
            }

            m_triggered = true;
            foreach (Root root in m_roots)
            {
                root.rootAnimator.SetBool("On", false);
            }
            foreach (Animator flower in m_flowers)
            {
                flower.SetBool("On", true);
            }
            if (m_saveTrigger)
            {
                SaveDataManager.SetFlag(m_triggerID);
            }
        }
    }

    private void Initialize()
    {
        foreach (Root root in m_roots)
        {
            root.rootAnimator.SetBool("On", !m_triggered);
            root.rootAnimator.SetTrigger("Reset");
        }
        foreach (Animator flower in m_flowers)
        {
            flower.SetBool("On", m_triggered);
            flower.SetTrigger("Reset");
        }
    }
}
