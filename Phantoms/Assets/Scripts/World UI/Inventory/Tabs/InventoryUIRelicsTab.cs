using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUIRelicsTab : InventoryTab
{
    [Header("Prefab References")]
    [SerializeField]
    private GameObject m_relicButtonPrefab = null;
    [SerializeField]
    private GameObject m_relicTargetButtonPrefab = null;

    [Header("Scene References")]
    [SerializeField]
    private Transform m_relicButtonParent = null;

    [Header("Current Relic Selection Things")]
    [SerializeField]
    private TextMeshProUGUI m_relicNameText = null;
    [SerializeField]
    private TextMeshProUGUI m_relicDescriptionText = null;
    [SerializeField]
    private TextMeshProUGUI m_relicRPRequirementText = null;
    [SerializeField]
    private Button m_equipButton = null;
    [SerializeField]
    private TextMeshProUGUI m_equipButtonText = null;

    [Header("Choose Relic Target Things")]
    [SerializeField]
    private GameObject m_chooseRelicTargetParent = null;
    [SerializeField]
    private Transform m_relicTargetListParent = null;
    [SerializeField]
    private InventoryRelicTargetButton m_playerTargetButton = null;

    private List<InventoryRelicButton> m_relicButtons = new List<InventoryRelicButton>();
    private List<InventoryRelicTargetButton> m_relicTargetButtons = new List<InventoryRelicTargetButton>();
    private RelicInstance m_currentRelic = null;

    void Start()
    {
        m_playerTargetButton.Data = DataManager.Instance.GetPlayerBattleInstanceData();
        ResetRelicList();
        ResetTargetList();
        HideRelicTargetMenu();
    }

    /// Overridden base tab functions ///
    public override void OpenTab()
    {
        base.OpenTab();
        ResetRelicList();
        ResetTargetList();
        HideRelicTargetMenu();
    }

    public void ResetRelicList()
    {
        foreach (InventoryRelicButton button in m_relicButtons)
        {
            button.gameObject.SetActive(false);
        }
        foreach (RelicInstance data in PlayerInventoryManager.Instance.Relics)
        {
            InventoryRelicButton relicButton = GetButton();
            relicButton.Data = data;
            relicButton.SetButton();
        }
    }

    public void ResetTargetList()
    {
        foreach (InventoryRelicTargetButton button in m_relicTargetButtons)
        {
            button.gameObject.SetActive(false);
        }

        m_playerTargetButton.SetButton();
        PlayerBattleInstanceData player = DataManager.Instance.GetPlayerBattleInstanceData();
        if (player.CanEquipRelic(m_currentRelic))
        {
            m_playerTargetButton.Disable();
        }
        else
        {
            m_playerTargetButton.Enable();
        }

        foreach (PhantomInstanceData data in PlayerInventoryManager.Instance.Phantoms)
        {
            InventoryRelicTargetButton relicTargetButton = GetTargetButton();
            relicTargetButton.Data = data;
            relicTargetButton.SetButton();
            if (data.CanEquipRelic(m_currentRelic))
            {
                relicTargetButton.Disable();
            }
            else
            {
                relicTargetButton.Enable();
            }
        }
    }

    public void SelectRelic(RelicInstance data)
    {
        if (data != null && data != m_currentRelic)
        {
            m_currentRelic = data;
            UpdateRelicDisplay();
        }
    }

    public void UpdateRelicDisplay()
    {
        if (m_currentRelic != null)
        {
            if (m_relicNameText != null)
            {
                m_relicNameText.text = m_currentRelic.Data.DisplayName;
            }
            if (m_relicDescriptionText != null)
            {
                m_relicDescriptionText.text = m_currentRelic.Data.Description;
            }
            if (m_relicRPRequirementText != null)
            {
                m_relicRPRequirementText.text = "Costs " + m_currentRelic.Data.Points + " RP";
            }

            if (m_equipButton)
            {
                // TODO: UPDATE BUTTON!
                m_equipButton.interactable = true;
                if (m_equipButtonText)
                {
                    if (m_currentRelic.Equipped)
                    {
                        m_equipButtonText.text = "Unequip from " + m_currentRelic.User.GetDisplayName();
                    }
                    else
                    {
                        m_equipButtonText.text = "Equip";
                    }
                }
            }
        }
        else
        {
            if (m_relicNameText != null)
            {
                m_relicNameText.text = "-";
            }
            if (m_relicDescriptionText != null)
            {
                m_relicDescriptionText.text = "-";
            }
            if (m_relicRPRequirementText)
            {
                m_relicRPRequirementText.text = "-";
            }
            if (m_equipButton)
            {
                // TODO: UPDATE BUTTON!
                m_equipButton.interactable = false;
                if (m_equipButtonText)
                {
                    m_equipButtonText.text = "-";
                }
            }
        }
    }

    public void ToggleEquipment()
    {
        if (m_currentRelic != null)
        {
            if (m_currentRelic.Equipped)
            {
                UserBattleInstanceData user = m_currentRelic.User;
                user.UnequipRelic(m_currentRelic);
                UpdateRelicDisplay();
            }
            else
            {
                ShowRelicTargetMenu();
            }
        }
    }

    public void EquipRelicWithTarget(UserBattleInstanceData user)
    {
        if (m_currentRelic != null && user.CanEquipRelic(m_currentRelic))
        {
            user.EquipRelic(m_currentRelic);
            ResetRelicList();
            ResetTargetList();
            HideRelicTargetMenu();
            UpdateRelicDisplay();
        }
    }

    ////////////////////////////////
    /// Private Helper Functions ///
    ////////////////////////////////
    private void ShowRelicTargetMenu()
    {
        m_chooseRelicTargetParent.SetActive(true);
        if (m_equipButton != null)
        {
            m_equipButton.interactable = false;
        }
    }

    private void HideRelicTargetMenu()
    {
        m_chooseRelicTargetParent.SetActive(false);
        if (m_currentRelic != null && m_equipButton != null)
        {
            m_equipButton.interactable = true;
        }
    }

    private InventoryRelicButton GetButton()
    {
        InventoryRelicButton returnButton = null;

        foreach (InventoryRelicButton button in m_relicButtons)
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
            GameObject instancedButton = Instantiate(m_relicButtonPrefab, m_relicButtonParent);
            returnButton = instancedButton.GetComponent<InventoryRelicButton>();
            returnButton.RelicsInventory = this;
            m_relicButtons.Add(returnButton);
        }

        return returnButton;
    }

    private InventoryRelicTargetButton GetTargetButton()
    {
        InventoryRelicTargetButton returnButton = null;

        foreach (InventoryRelicTargetButton button in m_relicTargetButtons)
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
            GameObject instancedButton = Instantiate(m_relicTargetButtonPrefab, m_relicTargetListParent);
            returnButton = instancedButton.GetComponent<InventoryRelicTargetButton>();
            returnButton.RelicsInventory = this;
            m_relicTargetButtons.Add(returnButton);
        }

        return returnButton;
    }
}
