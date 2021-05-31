using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleText : MonoBehaviour
{
    // Serialize Fields
    [SerializeField]
    private GameObject m_textParent = null;

    [SerializeField]
    private TMP_Text m_textComponent = null;

    [SerializeField]
    private GameObject m_continueButton = null;

    // Static instance stuff
    private static BattleText s_instance = null;
    public static BattleText Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<BattleText>();
                s_instance.CreateInstance();
            }
            return s_instance;
        }
    }

    // Private variables just for storing things, with public getters just in case.
    private bool m_showingText = false;
    public static bool ShowingText { get { return Instance.m_showingText; } }
    private bool m_battlePaused = false;
    public static bool BattlePaused { get { return Instance.m_battlePaused; } }

    Ares.BattleDelayElement m_battleDelayer = null;

    private bool m_isTimedPause = false;
    private float m_remainingPauseTime = 0f;

    //////////////////////////////////////////////////////
    /// Unity Functions and stuff to make the instance ///
    //////////////////////////////////////////////////////
    private void Awake()
    {
        // Set this to the instance if there is no other
        if (s_instance == null)
        {
            s_instance = this;
            CreateInstance();
        }
        // Destroy this if there's already another instance
        if (s_instance != this)
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (BattleManager.Instance != null && BattleManager.Instance.OnBattleStart != null)
        {
            BattleManager.Instance.OnBattleStart.RemoveListener(OnBattleStart);
        }
    }

    private void Update()
    {
        if (m_battlePaused && m_isTimedPause)
        {
            m_remainingPauseTime -= Time.deltaTime;
            if (m_remainingPauseTime <= 0)
            {
                ResumeBattle();
            }
        }
    }

    private void CreateInstance()
    {
        if (m_battleDelayer == null)
        {
            m_battleDelayer = gameObject.AddComponent<Ares.BattleDelayElement>();
        }

        BattleManager.Instance.OnBattleStart.AddListener(OnBattleStart);
        OnBattleStart();
        HideTextInternal();
    }

    ////////////////////////////////////////////
    /// Public static functions to be called ///
    ////////////////////////////////////////////
    public static void SetText(string text, bool pauseBattle = false, float maxPauseTime = 4f)
    {
        Instance.SetTextInternal(text, pauseBattle, maxPauseTime);
    }

    public static void HideText()
    {
        Instance.HideTextInternal();
    }

    ////////////////////////////////////////////
    /// Public functions, mostly for buttons ///
    ////////////////////////////////////////////
    public void ResumeBattle()
    {
        HideTextInternal();
    }

    public void OnBattleStart()
    {
        if (BattleManager.Instance != null && BattleManager.Instance.CurrentBattle != null)
        {
            m_battleDelayer.LinkToBattle(BattleManager.Instance.CurrentBattle);
        }
    }

    ////////////////////////////////////////////////////
    /// private internal functions with actual logic ///
    ////////////////////////////////////////////////////
    private void SetTextInternal(string text, bool pauseBattle, float maxPauseTime)
    {
        m_textParent.SetActive(true);
        m_textComponent.text = text;

        if (pauseBattle)
        {
            // Pause Battle
            m_battleDelayer.RequestBattleDelayLock(Ares.DelayRequestReason.UIEvent);

            m_battlePaused = true;
            m_continueButton.SetActive(true);

            if (maxPauseTime > 0f)
            {
                m_isTimedPause = true;
                m_remainingPauseTime = maxPauseTime;
            }
            else
            {
                m_isTimedPause = false;
            }
        }
        else
        {
            m_continueButton.SetActive(false);
        }
    }

    private void HideTextInternal()
    {
        m_textParent.SetActive(false);

        if (m_battlePaused)
        {
            // Resume battle
			m_battleDelayer.ReleaseBattleDelayLock();
            
            m_battlePaused = false;
            m_isTimedPause = false;
        }
    }
}
