using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ares;

public class InventoryUIItemsTab : InventoryTab
{
    [Header("Prefab References")]
    [SerializeField]
    private GameObject m_itemButtonPrefab = null;

    [Header("Scene References")]
    [SerializeField]
    private Transform m_itemButtonParent = null;

    [Header("Current Phantom Selection Things")]
    [SerializeField]
    private TextMeshProUGUI m_itemNameText = null;
    [SerializeField]
    private TextMeshProUGUI m_itemDescriptionText = null;

    // private variables
    private List<InventoryItemButton> m_itemButtons = new List<InventoryItemButton>();
    private ItemInstanceData m_currentItem = null;

    // Start is called before the first frame update
    void Start()
    {
        ResetPhantomList();
    }

    /// Overridden base tab functions ///
    public override void OpenTab()
    {
        base.OpenTab();
        ResetPhantomList();
    }

    public void ResetPhantomList()
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
        }
    }

    // Private Helper Functions
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

    private bool IsItemUsable(ItemInstanceData itemData)
    {
        if (itemData == null) { return false; }

        // Some other quick things to check
        //if (itemData.AresData.ValidTargetGroups == BattleInteractorData.TargetGroupGroups.Opponents) { return false; }
        //if (it) (itemData.AresData.ValidTargetGroups == BattleInteractorData.TargetGroupGroups.Opponents) { return false; }

        foreach(ItemAction action in itemData.AresData.Actions)
        {
            if (action.Action == ChainEvaluator.ActionType.Heal)
            {
                if (action.PowerMode == ActionChainValueEvaluator.PowerType.Constant || action.PowerMode == ActionChainValueEvaluator.PowerType.Random)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
