using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class InventoryItemButton : InventoryGenericButton
{
    [Header("Button References")]
    [SerializeField]
    private TextMeshProUGUI m_itemQuantityText = null;

    public InventoryUIItemsTab ItemsInventory = null;
    public ItemInstanceData Data = null;

    ///////////////////////////////////////////////////////////////////////////
    /// Public functions for clicking, hovering, and setting up the button. ///
    ///////////////////////////////////////////////////////////////////////////
    public override void OnClick()
    {
        base.OnClick();

        if (ItemsInventory && Data != null)
        {
            ItemsInventory.SelectItem(this);
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
                m_itemQuantityText.text = "x" + Data.Quantity.ToString();
            }

            bool buttonDisabled = Data.Quantity <= 0;
            m_buttonComponent.interactable = !buttonDisabled;
            m_buttonAnimator.SetBool("Disabled", buttonDisabled);
        }
    }
}
