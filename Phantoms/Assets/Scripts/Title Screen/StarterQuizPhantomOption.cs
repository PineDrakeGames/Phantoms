using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StarterQuizPhantomOption : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField]
    Image m_phantomIconFill = null;
    [SerializeField]
    Image m_phantomIconLines = null;

    [Header("Scene References")]
    [SerializeField]
    StarterQuizManager m_manager = null;

    private Button m_buttonComponent = null;
    public Button ButtonComponent { get { return m_buttonComponent; } }

    private PhantomData m_phantom = null;
    public PhantomData Phantom
    {
        get { return m_phantom; }
        set { SetPhantom(value); }
    }

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Awake()
    {
        m_buttonComponent = GetComponent<Button>();
        m_buttonComponent.onClick.AddListener(OnClick);
    }

    private void OnDestroy() 
    {
        if (m_buttonComponent)
        {
            m_buttonComponent.onClick.RemoveAllListeners();
        }
    }


    ////////////////////////
    /// Public Functions ///
    ////////////////////////
    public void OnClick()
    {
        if (m_manager != null && m_phantom != null)
        {
            m_manager.SelectPhantom(m_phantom);
        }
    }

    public void SetPhantom(PhantomData newPhantom)
    {
        m_phantom = newPhantom;
        m_phantomIconFill.sprite = m_phantom.IconFill;
        m_phantomIconLines.sprite = m_phantom.IconLines;
    }
}
