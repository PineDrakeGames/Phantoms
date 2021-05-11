using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialControlPopup : MonoBehaviour
{

    // This script is kinda set up badly, ideally special and normal keys wouldn't be differently queued

    ////////////////////////
    /// Serialize Fields ///
    ////////////////////////
    [Header("Needed Data")]
    [SerializeField]
    private SpecialKeysTable m_specialKeysTable = null;

    [Header("Scene References")]
    [SerializeField]
    private CanvasGroup m_tutorialControlPopup = null;

    [SerializeField]
    private GameObject m_keyParent = null;
    [SerializeField]
    private TMP_Text m_keyText = null;

    [SerializeField]
    private Image m_specialKeyImage = null;

    [Header("Display Tuning Data")]
    [SerializeField]
    private float m_fadeDuration = 1f;

    /////////////////////////
    /// Private Variables ///
    /////////////////////////
    private enum PopupState
    {
        Off,
        FadingIn,
        On,
        FadingOut
    }

    private PopupState m_currentState = PopupState.Off;
    private bool m_queuedPopup = false;
    private KeyCode m_queuedKey = KeyCode.None;
    private SpecialKey m_queuedSpecialKey = SpecialKey.NONE;

    private KeyCode m_currentKey = KeyCode.None;
    private SpecialKey m_currentSpecialKey = SpecialKey.NONE;

    private float m_currentFadeTime = 0f;

    private static TutorialControlPopup s_instance = null;
    public static TutorialControlPopup Instance
    {
        get
        {
            if (!s_instance)
            {
                s_instance = FindObjectOfType<TutorialControlPopup>();
            }
            return s_instance;
        }
    }

    //////////////////////
    // Unity Functions ///
    //////////////////////
    private void Awake() 
    {
        m_tutorialControlPopup.alpha = 0f;
        if (s_instance == null)
        {
            s_instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        switch(m_currentState)
        {
            case PopupState.Off:
                if (m_queuedPopup)
                {
                    if (m_queuedSpecialKey == SpecialKey.NONE)
                    {
                        ShowKeyPopup(m_queuedKey);
                    }
                    else
                    {
                        ShowSpecialKeyPopup(m_queuedSpecialKey);
                    }
                }
                break;
            case PopupState.On:
                break;
            case PopupState.FadingIn:
                m_currentFadeTime += Time.deltaTime;
                if (m_currentFadeTime >= m_fadeDuration)
                {
                    m_currentFadeTime = m_fadeDuration;
                    m_currentState = PopupState.On;
                }
                m_tutorialControlPopup.alpha = (m_currentFadeTime / m_fadeDuration);
                break;
            case PopupState.FadingOut:
                m_currentFadeTime -= Time.deltaTime;
                if (m_currentFadeTime <= 0f)
                {
                    m_currentFadeTime = 0f;
                    m_currentState = PopupState.Off;
                }
                m_tutorialControlPopup.alpha = (m_currentFadeTime / m_fadeDuration);
                break;
        }
    }


    ////////////////////////////////////////////////////////
    /// Public Functions to show various tutorial popups ///
    ////////////////////////////////////////////////////////
    public void HidePopup()
    {
        m_currentState = PopupState.FadingOut;
    }

    public void ShowKeyPopup(KeyCode key, bool Override = false)
    {
        if (m_currentState != PopupState.Off)
        {
            if (m_currentKey == key)
            {
                // Do nothing if already fading in or on
                if (m_currentState == PopupState.FadingOut && Override) { m_currentState = PopupState.FadingIn; }
            }
            else
            {
                m_queuedPopup = true;
                m_queuedKey = key;
                m_queuedSpecialKey = SpecialKey.NONE;

                if (Override) { m_currentState = PopupState.FadingOut; }
            }
        }
        else
        {
            m_currentKey = key;
            m_currentSpecialKey = SpecialKey.NONE;

            m_keyParent.SetActive(true);
            m_keyText.text = key.ToString();
            m_specialKeyImage.gameObject.SetActive(false);

            m_currentState = PopupState.FadingIn;
        }
    }

    public void ShowSpecialKeyPopup(SpecialKey specialKey, bool Override = false)
    {
        if (m_currentState != PopupState.Off)
        {
            if (m_currentSpecialKey == specialKey)
            {
                // Do nothing if already fading in or on
                if (m_currentState == PopupState.FadingOut && Override) { m_currentState = PopupState.FadingIn; }
            }
            else
            {
            m_queuedPopup = true;
            m_queuedKey = KeyCode.None;
            m_queuedSpecialKey = specialKey;

            if (Override) { m_currentState = PopupState.FadingOut; }
            }
        }
        else
        {
            m_currentKey = KeyCode.None;
            m_currentSpecialKey = specialKey;

            m_keyParent.SetActive(false);
            m_specialKeyImage.gameObject.SetActive(true);
            m_specialKeyImage.sprite = m_specialKeysTable.KeyToIcon[specialKey];

            m_currentState = PopupState.FadingIn;
        }
    }
}
