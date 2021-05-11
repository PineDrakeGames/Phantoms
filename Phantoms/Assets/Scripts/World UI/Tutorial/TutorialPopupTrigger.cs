using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPopupTrigger : MonoBehaviour
{
    [Header("TRIGGER ID STUFF")]
    [SerializeField]
    private bool m_saveDisabledState = true;
    [SerializeField]
    private string m_popupTriggerID = "Tutorial_";

    [Header("Key to Show")]
    [SerializeField]
    private SpecialKey m_specialKey = SpecialKey.NONE;
    [SerializeField]
    private KeyCode m_key = KeyCode.None;

    [Header("Options")]
    [SerializeField]
    private bool m_showOnStart = false;
    [SerializeField]
    private bool m_showOnTriggerEnter = true;
    [SerializeField]
    private bool m_hideOnTriggerExit = true;
    [SerializeField]
    private bool m_disableOnTriggerExit = true;
    [SerializeField]
    private bool m_overrideCurrentTutorial = false;


    private bool m_active = false;
    private bool m_disabled = false;

    // TEMPORARY WAY OF "SAVING" WHAT TUTORIALS HAVE BEEN COMPLETED SO THEY DON'T TRIGGER AGAIN
    private static List<string> s_completedTutorials = new List<string>();

    private void Awake()
    {
        if (s_completedTutorials.Contains(m_popupTriggerID) && m_saveDisabledState)
        {
            this.enabled = false;
        }
    }

    public void Show()
    {
        if (!m_disabled && (!s_completedTutorials.Contains(m_popupTriggerID) || !m_saveDisabledState))
        {
            if (m_specialKey != SpecialKey.NONE)
            {
                TutorialControlPopup.Instance.ShowSpecialKeyPopup(m_specialKey, m_overrideCurrentTutorial);
            }
            else
            {
                TutorialControlPopup.Instance.ShowKeyPopup(m_key, m_overrideCurrentTutorial);
            }
        }
        m_active = true;
    }

    public void Hide()
    {
        TutorialControlPopup.Instance.HidePopup();
        m_active = false;
    }

    public void Disable()
    {
        m_disabled = true;
        if (m_active)
        {
            Hide();
        }

        if (!s_completedTutorials.Contains(m_popupTriggerID) && m_saveDisabledState)
        {
            s_completedTutorials.Add(m_popupTriggerID);
        }
    }

    public void StopDisable()
    {
        m_disabled = false;
        if (m_active) { Show(); }
    }


    ///////////////////////
    /// Unity Functions ///
    ///////////////////////

    private void Start()
    {
        if (m_showOnStart)
        {
            Show();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && m_showOnTriggerEnter)
        {
            Show();
        }    
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (m_disableOnTriggerExit)
            {
                Disable();
            }
            else if (m_hideOnTriggerExit)
            {
                Hide();
            }
        }  
    }
}
