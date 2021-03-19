using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class InventoryItemTargetButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button References")]
    [SerializeField]
    private TextMeshProUGUI m_combatantNameText = null;
    [SerializeField]
    private TextMeshProUGUI m_currentHPText = null;
    [SerializeField]
    private TextMeshProUGUI m_currentMPText = null;

    public InventoryUIItemsTab ItemsInventory = null;
    public CombatantInstanceData Data = null;

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
        if (ItemsInventory && Data != null)
        {
            ItemsInventory.UseItemWithTarget(Data);
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
            if (m_currentHPText != null)
            {
                m_currentHPText.text = string.Format("<b>HP</b> <color=red>{0}/{1}</color>", Data.CurrentHP, Data.CurrentStats.MaxHP);
            }
            if (m_currentMPText != null)
            {
                m_currentMPText.text = string.Format("<b>MP</b> <color=blue>{0}/{1}</color>", Data.CurrentMana, Data.CurrentStats.Mana);
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
