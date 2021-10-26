using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// NOTE(CJ): This is mostly the same as InventoryUIItemsTab, but currently they're considered different things.
// Feels like this could be abstracted, but if these are the only two cases, this is probably fine for now?

public class InventoryUIKeyItemsTab : InventoryTab
{
    [Header("Prefab References")]
    [SerializeField]
    private GameObject m_itemButtonPrefab = null;


    [Header("Scene References")]
    [SerializeField]
    private Transform m_itemButtonParent = null;

    [Header("Current Item Selection Things")]
    [SerializeField]
    private TextMeshProUGUI m_itemNameText = null;
    [SerializeField]
    private TextMeshProUGUI m_itemDescriptionText = null;
    [SerializeField]
    private Button m_useItemButton = null;
    [SerializeField]
    private TextMeshProUGUI m_useItemButtonText = null;

    // private variables
    private List<InventoryKeyItemButton> m_keyItemButtons = new List<InventoryKeyItemButton>();
    private InventoryKeyItemButton m_currentKeyItem = null;

    // Start is called before the first frame update
    void Start()
    {
        ResetItemList();
    }

    /// Overridden base tab functions ///
    public override void OpenTab()
    {
        base.OpenTab();
        ResetItemList();
        UpdateItemDisplay();
        InventoryUIManager.Instance.OnSelectedPartyMemberUpdate.AddListener(OnPartyMemberSelect);
    }

    public override void CloseTab()
    {
        base.CloseTab();
        InventoryUIManager.Instance.OnSelectedPartyMemberUpdate.RemoveListener(OnPartyMemberSelect);
    }

    public void ResetItemList()
    {
        foreach(InventoryKeyItemButton button in m_keyItemButtons)
        {
            button.gameObject.SetActive(false);
        }
        foreach(KeyItemInstanceData data in PlayerInventoryManager.Instance.KeyItems)
        {
            InventoryKeyItemButton itemButton = GetButton();
            itemButton.Data = data;
            itemButton.SetupButton();
        }
    }

    public void SelectKeyItem(InventoryKeyItemButton data)
    {
        if (data != null && data != m_currentKeyItem)
        {
            if (m_currentKeyItem != null)
            {
                m_currentKeyItem.Selected = false;
            }
            m_currentKeyItem = data;
            UpdateItemDisplay();
        }
    }

    public void OnPartyMemberSelect()
    {
        if (m_currentKeyItem != null)
        {
            UpdateItemDisplay();
        }
    }

    public void UpdateItemDisplay()
    {
        if (m_currentKeyItem != null)
        {
            if (m_itemNameText != null)
            {
                m_itemNameText.text = m_currentKeyItem.Data.Data.DisplayName;
            }
            if (m_itemDescriptionText != null)
            {
                m_itemDescriptionText.text = m_currentKeyItem.Data.Data.Description;
            }
            if (m_useItemButton)
            {
                m_useItemButton.gameObject.SetActive(false);
            }
        }
        else
        {
            if (m_itemNameText != null)
            {
                m_itemNameText.text = "Select a Key Item!";
            }
            if (m_itemDescriptionText != null)
            {
                m_itemDescriptionText.text = "";
            }
            if (m_useItemButton)
            {
                m_useItemButton.interactable = false;
                m_useItemButton.gameObject.SetActive(false);
            }
        }
    }

    public void UseItem()
    {
        if (m_currentKeyItem != null && m_currentKeyItem.Data.Quantity > 0)
        {
            // Some key items may have uses in the future, but for now, nope!
        }
    }

    public void UseItemWithTarget(CombatantInstanceData combatant)
    {
        if (m_currentKeyItem != null && m_currentKeyItem.Data.Quantity > 0)
        {
            // Same as normal use item - no use right now, but maybe later?
        }
    }

    ////////////////////////////////
    /// Private Helper Functions ///
    ////////////////////////////////
    private InventoryKeyItemButton GetButton()
    {
        InventoryKeyItemButton returnButton = null;

        foreach(InventoryKeyItemButton button in m_keyItemButtons)
        {
            if (!button.gameObject.activeSelf)
            {
                returnButton = button;
                button.gameObject.SetActive(true);
                break;
            }
        }

        if (returnButton == null)
        {
            GameObject instancedButton = Instantiate(m_itemButtonPrefab, m_itemButtonParent);
            returnButton = instancedButton.GetComponent<InventoryKeyItemButton>();
            returnButton.KeyItemsInventory = this;
            m_keyItemButtons.Add(returnButton);
        }

        return returnButton;
    }
}
