using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class InventoryRelicTargetButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button References")]
    [SerializeField]
    private TextMeshProUGUI m_combatantNameText = null;
    [SerializeField]
    private TextMeshProUGUI m_currentRPText = null;

    public InventoryUIRelicsTab RelicsInventory = null;
    public UserBattleInstanceData Data = null;

    private Button m_buttonComponent = null;

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

    ///////////////////////////////////////////////////////////////////////////
    /// Public functions for clicking, hovering, and setting up the button. ///
    ///////////////////////////////////////////////////////////////////////////
    public void OnClick()
    {
        if (RelicsInventory && Data != null)
        {
            // Equip Relic
        }
    }

    public void SetButton()
    {
        if (Data != null)
        {
            if (m_combatantNameText != null)
            {
                m_combatantNameText.text = Data.Data.DisplayName;
            }
            if (m_currentRPText != null)
            {
                m_currentRPText.text = string.Format("<b>Available RP</b> <color=yellow>{0}/{1}</color>", Data.CurrentStats.Relic - Data.CurrentRelicPoints, Data.CurrentStats.Relic);
            }
        }
    }

    public void OnHover()
    {
        // future animation stuff?
    }

    public void OnStopHover()
    {
        // future animation stuff?
    }

    public void Enable() { SetEnabled(true); }
    public void Disable() { SetEnabled(false); }
    public void SetEnabled(bool enabled = true)
    {
        if (!m_buttonComponent) { m_buttonComponent = GetComponent<Button>(); }
        m_buttonComponent.interactable = enabled;
    }

    //////////////////////////
    /// IPointer functions ///
    //////////////////////////
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OnStopHover();
    }
}
