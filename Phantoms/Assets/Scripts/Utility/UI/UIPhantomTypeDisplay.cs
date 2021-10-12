using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPhantomTypeDisplay : MonoBehaviour
{
    [SerializeField]
    private PhantomType m_type = PhantomType.NONE;
    public PhantomType Type
    {
        get { return m_type; }
        set
        {
            SetType(value);    
        }
    }



    [SerializeField]
    private List<PhantomTypeColorAssigner> m_phantomColorAssigners = new List<PhantomTypeColorAssigner>();

    [SerializeField]
    private TMP_Text m_phantomDisplayText = null;


    public void SetType(PhantomType type)
    {
        if (m_type == type)
        {
            return;
        }

        m_type = type;

        foreach(PhantomTypeColorAssigner colorAssigner in m_phantomColorAssigners)
        {
            colorAssigner.Type = type;
        }

        // TODO: Pull this from somewhere else, not here!
        if (m_phantomDisplayText != null)
        {
            string displayName = m_type.ToString();
            m_phantomDisplayText.text = displayName;
        }
    }
}
