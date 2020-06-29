using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_dialogueBox = null;
    [SerializeField]
    private FancyText m_fancyTextComponent = null;
    [SerializeField]
    private GameObject m_lineFinished = null;

    private bool m_revealing = false;
    public bool Revealing
    {
        get { return m_revealing; }
    }

    private bool m_active = false;

    private void Awake() 
    {
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
                m_lineFinished.SetActive(!revealing);
                m_revealing = revealing;
                Debug.Log(m_revealing);
            }
        }
    }

    public void DisplayText(string text)
    {
        m_revealing = true;
        m_active = true;
        m_lineFinished.SetActive(false);

        m_dialogueBox.SetActive(true);
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

        m_dialogueBox.SetActive(false);
    }
}
