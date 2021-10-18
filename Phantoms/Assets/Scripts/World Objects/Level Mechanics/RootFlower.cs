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

    [SerializeField]
    private float platformSpawnDelay = 0f;

    [SerializeField]
    private PixelCrushers.DialogueSystem.Wrappers.DialogueSystemTrigger m_triggerCutscene = null;

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

    [Header("Effects")]
    [SerializeField]
    private SoundEffectData RootHit = null;
    [SerializeField]
    private SoundEffectData FlowerGrow = null;

    private bool m_triggered = false;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(m_triggerID))
        {
            int numRootFlowers = FindObjectsOfType<RootFlower>().Length;
            m_triggerID = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name + "_RootFlower_" + numRootFlowers.ToString();
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
        AudioManager.PlaySound3D(RootHit, attackedRoot.transform.position);

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
            TriggerRoot();
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

    private void TriggerRoot()
    {
        m_triggered = true;
        foreach (Root root in m_roots)
        {
            root.rootAnimator.SetBool("On", false);
        }
        
        if (m_saveTrigger)
        {
            SaveDataManager.SetFlag(m_triggerID);
        }
        if (m_triggerCutscene)
        {
            m_triggerCutscene.OnUse();
        }
        StartCoroutine(MakePlatformAfterDelay(platformSpawnDelay));
    }

    private IEnumerator MakePlatformAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        MakePlatforms();
    }

    private void MakePlatforms()
    {
        foreach (Animator flower in m_flowers)
        {
            AudioManager.PlaySound3D(FlowerGrow, flower.transform.position);
            flower.SetBool("On", true);
        }
    }
}
