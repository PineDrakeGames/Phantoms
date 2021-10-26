using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Animator))]
public abstract class InventoryGenericButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [Header("Base References")]
    [SerializeField]
    protected Image m_icon = null;

    private bool m_selected = false;
    public bool Selected
    {
        get { return m_selected; }
        set
        {
            m_selected = value;
            m_buttonAnimator.SetBool("Selected", m_selected);
        }
    }

    protected Button m_buttonComponent = null;
    protected Animator m_buttonAnimator = null;

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    protected void Awake()
    {
        m_buttonComponent = GetComponent<Button>();
        m_buttonAnimator = GetComponent<Animator>();
        m_buttonComponent.onClick.AddListener(OnClick);
    }

    protected void OnDestroy()
    {
        if (m_buttonComponent)
        {
            m_buttonComponent.onClick.RemoveAllListeners();
        }
    }


    public virtual void OnClick()
    {
        Selected = true;
    }
    public virtual void SetupButton()
    {
        m_buttonAnimator.SetBool("Selected", Selected);
    }

    public virtual void OnHover()
    {
        m_buttonAnimator.SetBool("Highlighted", true);
    }

    public virtual void OnStopHover()
    {
        // future animation stuff?
        m_buttonAnimator.SetBool("Highlighted", false);
    }

    //////////////////////////
    /// IPointer functions ///
    //////////////////////////
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (m_buttonComponent.interactable)
        {
            OnHover();
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (m_buttonComponent.interactable)
        {
            OnStopHover();
        }
    }
}
