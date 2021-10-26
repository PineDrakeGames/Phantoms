using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonSoundEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private string m_buttonHoverSoundID = "DEFAULT_HOVER";

    [SerializeField]
    private string m_buttonClickSoundID = "DEFAULT_CLICK";

    private Button m_buttonComponent = null;

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Awake()
    {
        m_buttonComponent = GetComponent<Button>();
        m_buttonComponent.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        AudioManager.PlaySound(m_buttonClickSoundID);
    }

    public void OnHover()
    {
        if (!string.IsNullOrEmpty(m_buttonHoverSoundID) && m_buttonComponent.interactable)
        {
            AudioManager.PlaySound(m_buttonHoverSoundID, 1f, 1f, false);
        }
    }

    public void OnStopHover()
    {
        // I dunno
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
