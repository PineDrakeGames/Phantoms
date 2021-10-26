using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUIItemsTab : InventoryTab
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
    private List<InventoryItemButton> m_itemButtons = new List<InventoryItemButton>();
    private InventoryItemButton m_currentItem = null;

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
        foreach(InventoryItemButton button in m_itemButtons)
        {
            button.gameObject.SetActive(false);
        }
        foreach(ItemInstanceData data in PlayerInventoryManager.Instance.Items)
        {
            InventoryItemButton itemButton = GetButton();
            itemButton.Data = data;
            itemButton.SetupButton();
        }
    }

    public void SelectItem(InventoryItemButton data)
    {
        if (data != null && data != m_currentItem)
        {
            if (m_currentItem != null)
            {
                m_currentItem.Selected = false;
            }
            m_currentItem = data;
            UpdateItemDisplay();
        }
    }

    public void OnPartyMemberSelect()
    {
        if (m_currentItem != null)
        {
            UpdateItemDisplay();
        }
    }

    public void UpdateItemDisplay()
    {
        if (m_currentItem != null)
        {
            if (m_itemNameText != null)
            {
                m_itemNameText.text = m_currentItem.Data.AresData.DisplayName;
            }
            if (m_itemDescriptionText != null)
            {
                m_itemDescriptionText.text = m_currentItem.Data.AresData.Description;
            }
            if (m_useItemButton)
            {
                m_useItemButton.gameObject.SetActive(m_currentItem.Data.Data.CanUseOutOfBattle );

                if (m_currentItem.Data.Quantity <= 0)
                {
                    m_useItemButton.interactable = false;
                    m_useItemButtonText.text = "All out!";
                }
                else if (m_currentItem.Data.Data.HealsAll)
                {
                    m_useItemButton.interactable = true;
                    m_useItemButtonText.text = "Use On Everybody";
                    // Select all!
                }
                else
                {
                    // Check if someone is selected
                    if (InventoryUIManager.Instance.CurrentSelectedPartyMember != null)
                    {
                        m_useItemButton.interactable = true;
                        m_useItemButtonText.text = "Use On " + InventoryUIManager.Instance.CurrentSelectedPartyMember.PartyMemberData.GetDisplayName();
                    }
                    else
                    {
                        m_useItemButton.interactable = false;
                        m_useItemButtonText.text = "Select a target";
                    }
                }
            }
        }
        else
        {
            if (m_itemNameText != null)
            {
                m_itemNameText.text = "Select an Item!";
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
        if (m_currentItem != null && m_currentItem.Data.Quantity > 0 && m_currentItem.Data.Data.CanUseOutOfBattle)
        {
            if (m_currentItem.Data.Data.HealsAll)
            {
                foreach(CombatantInstanceData combatant in InventoryUIManager.Instance.AllPartyMembers)
                {
                    combatant.RestoreHealth(m_currentItem.Data.Data.HP);
                    combatant.RestoreMana(m_currentItem.Data.Data.Mana);
                }
                m_currentItem.Data.Quantity -= 1;
                m_currentItem.SetupButton();
            }
            else
            {
                UseItemWithTarget(InventoryUIManager.Instance.CurrentSelectedPartyMember.PartyMemberData);
            }

            if (m_currentItem.Data.Quantity <= 0)
            {
                m_currentItem.Selected = false;
                m_currentItem = null;
                UpdateItemDisplay();
            }

            InventoryUIManager.Instance.SetPartyMembers();
        }
    }

    public void UseItemWithTarget(CombatantInstanceData combatant)
    {
        if (m_currentItem != null && m_currentItem.Data.Quantity > 0 && m_currentItem.Data.Data.CanUseOutOfBattle)
        {
            combatant.RestoreHealth(m_currentItem.Data.Data.HP);
            combatant.RestoreMana(m_currentItem.Data.Data.Mana);
            m_currentItem.Data.Quantity -= 1;
            m_currentItem.SetupButton();
        }
    }

    ////////////////////////////////
    /// Private Helper Functions ///
    ////////////////////////////////
    private InventoryItemButton GetButton()
    {
        InventoryItemButton returnButton = null;

        foreach(InventoryItemButton button in m_itemButtons)
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
            returnButton = instancedButton.GetComponent<InventoryItemButton>();
            returnButton.ItemsInventory = this;
            m_itemButtons.Add(returnButton);
        }

        return returnButton;
    }
}
