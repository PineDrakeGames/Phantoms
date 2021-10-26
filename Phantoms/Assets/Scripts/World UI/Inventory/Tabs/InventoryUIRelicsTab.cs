using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUIRelicsTab : InventoryTab
{
    [Header("Prefab References")]
    [SerializeField]
    private GameObject m_relicButtonPrefab = null;

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

    private List<InventoryRelicButton> m_relicButtons = new List<InventoryRelicButton>();
    private InventoryRelicButton m_currentRelic = null;

    void Start()
    {
        ResetRelicList();
    }

    /// Overridden base tab functions ///
    public override void OpenTab()
    {
        base.OpenTab();
        ResetRelicList();
        UpdateRelicDisplay();
        InventoryUIManager.Instance.OnSelectedPartyMemberUpdate.AddListener(OnPartyMemberSelect);
    }

    public override void CloseTab()
    {
        base.CloseTab();
        InventoryUIManager.Instance.OnSelectedPartyMemberUpdate.RemoveListener(OnPartyMemberSelect);
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
            relicButton.SetupButton();
        }
    }

    public void SelectRelic(InventoryRelicButton data)
    {
        if (data != null && data != m_currentRelic)
        {
            if (m_currentRelic != null)
            {
                m_currentRelic.Selected = false;
            }
            m_currentRelic = data;
            UpdateRelicDisplay();
        }
    }

    public void OnPartyMemberSelect()
    {
        if (m_currentRelic != null)
        {
            UpdateRelicDisplay();
        }
    }

    public void UpdateRelicDisplay()
    {
        if (m_currentRelic != null)
        {
            if (m_relicNameText != null)
            {
                m_relicNameText.text = m_currentRelic.Data.Data.DisplayName;
            }
            if (m_relicDescriptionText != null)
            {
                m_relicDescriptionText.text = m_currentRelic.Data.Data.Description;
            }
            if (m_relicRPRequirementText != null)
            {
                m_relicRPRequirementText.text = "Costs " + m_currentRelic.Data.Data.Points + " RP";
            }

            if (m_equipButton)
            {
                // TODO: UPDATE BUTTON!
                m_equipButton.interactable = true;
                if (m_equipButtonText)
                {
                    if (m_currentRelic.Data.Equipped)
                    {
                        m_equipButtonText.text = "Unequip from " + m_currentRelic.Data.User.GetDisplayName();
                    }
                    else
                    {
                        // Check if someone is selected
                        if (InventoryUIManager.Instance.CurrentSelectedPartyMember != null)
                        {
                            UserBattleInstanceData data = InventoryUIManager.Instance.CurrentSelectedPartyMember.PartyMemberData;
                            if (data.CanEquipRelic(m_currentRelic.Data))
                            {
                                m_equipButton.interactable = true;
                                m_equipButtonText.text = "Equip To " + data.GetDisplayName();
                            }
                            else
                            {
                                m_equipButton.interactable = false;
                                m_equipButtonText.text = data.GetDisplayName() + " Can't Equip";
                            }
                        }
                        else
                        {
                            m_equipButton.interactable = false;
                            m_equipButtonText.text = "Select a target";
                        }
                    }
                }
            }

            // Update the party member list
            foreach (UserBattleInstanceData partyMember in InventoryUIManager.Instance.AllPartyMembers)
            {
                bool isActive = partyMember.CanEquipRelic(m_currentRelic.Data) || (m_currentRelic.Data.User == partyMember);
                InventoryPartyMember inventoryMember = InventoryUIManager.Instance.PartyDataToInventory[partyMember];
                if (!isActive)
                {
                    if (InventoryUIManager.Instance.CurrentSelectedPartyMember == inventoryMember)
                    {
                        InventoryUIManager.Instance.DeselectPartyMember();
                    }
                    InventoryUIManager.Instance.PartyDataToInventory[partyMember].Disable();
                    Debug.Log(partyMember.GetDisplayName() + " Can't Equip!");
                }
                else
                {
                    if (InventoryUIManager.Instance.CurrentSelectedPartyMember != inventoryMember)
                    {
                        InventoryUIManager.Instance.PartyDataToInventory[partyMember].ResetState();
                    }
                    Debug.Log(partyMember.GetDisplayName() + " Can Equip!");
                }
            }
        }
        else
        {
            if (m_relicNameText != null)
            {
                m_relicNameText.text = "Select a Relic!";
            }
            if (m_relicDescriptionText != null)
            {
                m_relicDescriptionText.text = "";
            }
            if (m_relicRPRequirementText)
            {
                m_relicRPRequirementText.text = "";
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
            if (m_currentRelic.Data.Equipped)
            {
                UserBattleInstanceData user = m_currentRelic.Data.User;
                user.UnequipRelic(m_currentRelic.Data);
                m_currentRelic.SetupButton();
                UpdateRelicDisplay();
            }
            else
            {
                EquipRelicWithTarget(InventoryUIManager.Instance.CurrentSelectedPartyMember.PartyMemberData);
            }

            InventoryUIManager.Instance.SetPartyMembers();
        }
    }

    public void EquipRelicWithTarget(UserBattleInstanceData user)
    {
        if (m_currentRelic != null && user.CanEquipRelic(m_currentRelic.Data))
        {
            user.EquipRelic(m_currentRelic.Data);
            m_currentRelic.SetupButton();
            UpdateRelicDisplay();
            InventoryUIManager.Instance.SetPartyMembers();
        }
    }

    ////////////////////////////////
    /// Private Helper Functions ///
    ////////////////////////////////

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
}
