using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_dialogueBox = null;
    [SerializeField]
    private FancyText m_fancyTextComponent = null;

    private Animator m_dialogueBoxAnim = null;

    private bool m_revealing = false;
    public bool Revealing
    {
        get { return m_revealing; }
    }
    private bool m_active = false;

    private void Awake()
    {
        m_dialogueBox.SetActive(true);
        m_dialogueBoxAnim = m_dialogueBox.GetComponent<Animator>();
        if (m_dialogueBoxAnim) { m_dialogueBoxAnim.SetBool("Active", false); }
        m_fancyTextComponent.SetText("");
        Hide();
    }


    private void Update()
    {
        if (m_active)
        {
            bool revealing = m_fancyTextComponent.Revealing;
            if (revealing != m_revealing)
            {
                if (m_dialogueBoxAnim) { m_dialogueBoxAnim.SetBool("Finished Line", !revealing); }
                m_revealing = revealing;
            }
        }
    }

    public void DisplayText(string text)
    {
        m_revealing = true;
        m_active = true;


        if (m_dialogueBoxAnim)
        {
            m_dialogueBoxAnim.SetBool("Active", true);
            m_dialogueBoxAnim.SetBool("Finished Line", false);
        }
        m_fancyTextComponent.SetText(text);
    }

    public void FinishLine()
    {
        m_fancyTextComponent.FinishLine();
    }

    public void Hide()
    {
        m_revealing = false;
        m_active = false;

        if (m_dialogueBoxAnim) { m_dialogueBoxAnim.SetBool("Active", false); }
    }
}
