using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryKeyItemButton : InventoryGenericButton
{
    [Header("Button References")]
    [SerializeField]
    private TextMeshProUGUI m_itemQuantityText = null;

    public InventoryUIKeyItemsTab KeyItemsInventory = null;
    public KeyItemInstanceData Data = null;

    ///////////////////////////////////////////////////////////////////////////
    /// Public functions for clicking, hovering, and setting up the button. ///
    ///////////////////////////////////////////////////////////////////////////
    public override void OnClick()
    {
        base.OnClick();

        if (KeyItemsInventory && Data != null)
        {
            KeyItemsInventory.SelectKeyItem(this);
        }
    }

    public override void SetupButton()
    {
        base.SetupButton();

        if (Data != null)
        {
            if (m_icon != null)
            {
                m_icon.sprite = Data.Data.Sprite;
            }
            if (m_itemQuantityText != null)
            {
                if (Data.Data.CanStack)
                {
                    m_itemQuantityText.gameObject.SetActive(true);
                    m_itemQuantityText.text = "x" + Data.Quantity.ToString();
                }
                else
                {
                    m_itemQuantityText.gameObject.SetActive(false);
                }
            }
            
            m_buttonAnimator.SetBool("Disabled", Data.Quantity <= 0);
        }
    }
}
