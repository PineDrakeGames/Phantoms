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
    [SerializeField]
    private GameObject m_itemTargetButtonPrefab = null;

    [Header("Scene References")]
    [SerializeField]
    private Transform m_itemButtonParent = null;

    [Header("Current Phantom Selection Things")]
    [SerializeField]
    private TextMeshProUGUI m_itemNameText = null;
    [SerializeField]
    private TextMeshProUGUI m_itemDescriptionText = null;
    [SerializeField]
    private Button m_useItemButton = null;

    [Header("Choose Item Target Things")]
    [SerializeField]
    private GameObject m_chooseItemTargetParent = null;
    [SerializeField]
    private Transform m_itemTargetListParent = null;
    [SerializeField]
    private InventoryItemTargetButton m_playerTargetButton = null;

    // private variables
    private List<InventoryItemButton> m_itemButtons = new List<InventoryItemButton>();
    private List<InventoryItemTargetButton> m_itemTargetButtons = new List<InventoryItemTargetButton>();
    private List<CombatantInstanceData> m_allCombatants = new List<CombatantInstanceData>();
    private ItemInstanceData m_currentItem = null;

    // Start is called before the first frame update
    void Start()
    {
        m_playerTargetButton.Data = DataManager.Instance.GetPlayerBattleInstanceData();
        ResetItemList();
        ResetTargetList();
        HideItemTargetMenu();
    }

    /// Overridden base tab functions ///
    public override void OpenTab()
    {
        base.OpenTab();
        ResetItemList();
        ResetTargetList();
        HideItemTargetMenu();
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
            itemButton.SetButton();
        }
    }

    public void ResetTargetList()
    {
        foreach(InventoryItemTargetButton button in m_itemTargetButtons)
        {
            button.gameObject.SetActive(false);
        }

        m_playerTargetButton.SetButton();

        m_allCombatants.Clear();
        m_allCombatants.Add(DataManager.Instance.GetPlayerBattleInstanceData());

        foreach(PhantomInstanceData data in PlayerInventoryManager.Instance.Phantoms)
        {
            InventoryItemTargetButton itemTargetButton = GetTargetButton();
            itemTargetButton.Data = data;
            itemTargetButton.SetButton();
            m_allCombatants.Add(data);
        }
    }

    public void SelectItem(ItemInstanceData data)
    {
        if (data != null && data != m_currentItem)
        {
            m_currentItem = data;
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
                m_useItemButton.interactable = m_currentItem.Data.CanUseOutOfBattle && m_currentItem.Quantity > 0;
            }
        }
        else
        {
            if (m_itemNameText != null)
            {
                m_itemNameText.text = "-";
            }
            if (m_itemDescriptionText != null)
            {
                m_itemDescriptionText.text = "-";
            }
            if (m_useItemButton)
            {
                m_useItemButton.interactable = false;
            }
        }
    }

    public void UseItem()
    {
        if (m_currentItem != null && m_currentItem.Quantity > 0 && m_currentItem.Data.CanUseOutOfBattle)
        {
            if (m_currentItem.Data.HealsAll)
            {
                foreach(CombatantInstanceData combatant in m_allCombatants)
                {
                    combatant.RestoreHealth(m_currentItem.Data.HP);
                    combatant.RestoreMana(m_currentItem.Data.Mana);
                }
                m_currentItem.Quantity -= 1;
                ResetItemList();
            }
            else
            {
                ShowItemTargetMenu();
            }
        }
    }

    public void UseItemWithTarget(CombatantInstanceData combatant)
    {
        if (m_currentItem != null && m_currentItem.Quantity > 0 && m_currentItem.Data.CanUseOutOfBattle)
        {
            combatant.RestoreHealth(m_currentItem.Data.HP);
            combatant.RestoreMana(m_currentItem.Data.Mana);
            m_currentItem.Quantity -= 1;
            ResetItemList();
            ResetTargetList();
            if (m_currentItem.Quantity == 0)
            {
                HideItemTargetMenu();
            }
        }
    }

    ////////////////////////////////
    /// Private Helper Functions ///
    ////////////////////////////////
    private void ShowItemTargetMenu()
    {
        ResetTargetList();
        m_chooseItemTargetParent.SetActive(true);
        if (m_useItemButton != null)
        {
            m_useItemButton.interactable = false;
        }
    }

    private void HideItemTargetMenu()
    {
        m_chooseItemTargetParent.SetActive(false);
        if (m_currentItem != null && m_useItemButton != null)
        {
            m_useItemButton.interactable = m_currentItem.Data.CanUseOutOfBattle && m_currentItem.Quantity > 0;
        }
    }

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

    private InventoryItemTargetButton GetTargetButton()
    {
        InventoryItemTargetButton returnButton = null;

        foreach(InventoryItemTargetButton button in m_itemTargetButtons)
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
            GameObject instancedButton = Instantiate(m_itemTargetButtonPrefab, m_itemTargetListParent);
            returnButton = instancedButton.GetComponent<InventoryItemTargetButton>();
            returnButton.ItemsInventory = this;
            m_itemTargetButtons.Add(returnButton);
        }

        return returnButton;
    }
}
