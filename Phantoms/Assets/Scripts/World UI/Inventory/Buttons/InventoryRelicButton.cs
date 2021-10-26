using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class InventoryRelicButton : InventoryGenericButton
{
    [Header("Button References")]
    [SerializeField]
    private GameObject m_currentRelicEquipParent = null;
    [SerializeField]
    private TextMeshProUGUI m_currentRelicEquipText = null;
    [SerializeField]
    private List<GameObject> m_RPCostTicks = new List<GameObject>();

    public InventoryUIRelicsTab RelicsInventory = null;
    public RelicInstance Data = null;
    
    ///////////////////////////////////////////////////////////////////////////
    /// Public functions for clicking, hovering, and setting up the button. ///
    ///////////////////////////////////////////////////////////////////////////
    public override void OnClick()
    {
        base.OnClick();

        if (RelicsInventory && Data != null)
        {
            RelicsInventory.SelectRelic(this);
        }
    }

    public override void SetupButton()
    {
        base.SetupButton();

        if (Data != null)
        {
            if (m_icon != null)
            {
                m_icon.sprite = Data.Data.Icon;
            }
            if (m_currentRelicEquipText != null && m_currentRelicEquipParent != null)
            {
                if (Data.Equipped)
                {
                    m_currentRelicEquipParent.SetActive(true);
                    m_currentRelicEquipText.text = Data.User.GetDisplayName();
                }
                else
                {
                    m_currentRelicEquipParent.SetActive(false);
                }
            }
            int index = 0;
            for(index = 0; index < Data.Data.Points && index < m_RPCostTicks.Count; index++)
            {
                m_RPCostTicks[index].SetActive(true);
            }
            while (index < m_RPCostTicks.Count)
            {
                m_RPCostTicks[index].SetActive(false);
                index++;
            }
        }
    }
}
