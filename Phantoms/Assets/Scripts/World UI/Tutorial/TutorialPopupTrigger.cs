using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPopupTrigger : MonoBehaviour
{
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


    private bool m_active = false;
    private bool m_disabled = false;

    public void Show()
    {
        if (!m_disabled)
        {
            if (m_specialKey != SpecialKey.NONE)
            {
                TutorialControlPopup.Instance.ShowSpecialKeyPopup(m_specialKey);
            }
            else
            {
                TutorialControlPopup.Instance.ShowKeyPopup(m_key);
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
        Hide();
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
