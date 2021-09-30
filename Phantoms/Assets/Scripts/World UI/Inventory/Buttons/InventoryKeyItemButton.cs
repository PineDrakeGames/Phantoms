using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class InventoryKeyItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button References")]
    [SerializeField]
    private Image m_itemIcon = null;
    [SerializeField]
    private TextMeshProUGUI m_itemQuantityText = null;

    public InventoryUIKeyItemsTab KeyItemsInventory = null;
    public KeyItemInstanceData Data = null;

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
        if (KeyItemsInventory && Data != null)
        {
            KeyItemsInventory.SelectKeyItem(Data);
        }
    }

    public void SetButton()
    {
        if (Data != null)
        {
            if (m_itemIcon != null)
            {
                m_itemIcon.sprite = Data.Data.Sprite;
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
