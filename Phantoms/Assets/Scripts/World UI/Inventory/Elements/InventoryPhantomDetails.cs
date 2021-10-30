using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryPhantomDetails : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField]
    private GameObject m_menuParent = null;

    [Header("Current Phantom Selection Things")]
    [SerializeField]
    private TextMeshProUGUI m_phantomNicknameText = null;
    [SerializeField]
    private Image m_iconFill = null;
    [SerializeField]
    private Image m_iconLines = null;
    [SerializeField]
    private TextMeshProUGUI m_phantomBackgroundText = null;
    [SerializeField]
    private TextMeshProUGUI m_phantomDescriptionText = null;
    [SerializeField]
    private TMP_Text m_phantomHPText = null;
    [SerializeField]
    private TMP_Text m_phantomMPText = null;
    [SerializeField]
    private TMP_Text m_phantomRPText = null;
    [SerializeField]
    private TMP_Text m_phantomAttackText = null;
    [SerializeField]
    private TMP_Text m_phantomDefenseText = null;
    [SerializeField]
    private InventoryAbility[] m_inventoryAbilities = null;

    // private variables
    private UserBattleInstanceData m_currentPartyMember = null;

    private static InventoryPhantomDetails s_instance = null;
    public static InventoryPhantomDetails Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<InventoryPhantomDetails>();
            }
            return s_instance;
        }
    }

    private void Awake() 
    {
        if (s_instance == null)
        {
            s_instance = this;
        }

        HideDescription();
    }

    public void ShowDescription()
    {
        m_menuParent.SetActive(true);
    }

    public void HideDescription()
    {
        m_menuParent.SetActive(false);
        InventoryUIManager.Instance.DeselectPartyMember();
    }

    public void UpdatePhantomDisplay(UserBattleInstanceData user)
    {
        m_currentPartyMember = user;
        ShowDescription();


        if (m_currentPartyMember != null)
        {
            m_currentPartyMember.SetCurrentStats();

            // Set name
            string name = "No Name";
            if (!string.IsNullOrEmpty(m_currentPartyMember.GetDisplayName())) { name = m_currentPartyMember.GetDisplayName(); }
            name += " - " + m_currentPartyMember.Level;
            m_phantomNicknameText.text = name;

            // Set Icon
            m_iconFill.sprite = m_currentPartyMember.Data.IconFill;
            m_iconLines.sprite = m_currentPartyMember.Data.IconLines;

            // Set background
            //m_phantomBackgroundText.text = m_currentPhantom.Background.ToString();

            // Set Description
            /// Mostly just filling this with a bunch of info for now to see how things are working.
            string description = "";
            description += m_currentPartyMember.Data.Description; /* + "\n\n";
            //description += "HP: " + m_currentPartyMember.CurrentHP + "/" + m_currentPartyMember.CurrentStats.MaxHP + " (Level ups: " + m_currentPartyMember.LevelUps.MaxHP + ")\n";
            //description += "Mana: " + m_currentPartyMember.CurrentStats.Mana + " (Level ups: " + m_currentPartyMember.LevelUps.Mana + ")\n";
            //description += "Relic: " + m_currentPartyMember.CurrentStats.Relic + " (Level ups: " + m_currentPartyMember.LevelUps.Relic + ")\n";
            //description += "Attack: " + m_currentPartyMember.CurrentStats.Attack;
            //description += " - Defense: " + m_currentPartyMember.CurrentStats.Defense + "\n";
            */
            m_phantomDescriptionText.text = description;

            if (m_phantomHPText) { m_phantomHPText.text = m_currentPartyMember.CurrentHP.ToString() + " / " + m_currentPartyMember.CurrentStats.MaxHP.ToString(); }
            if (m_phantomMPText) { m_phantomMPText.text = m_currentPartyMember.CurrentMana.ToString() + " / " + m_currentPartyMember.CurrentStats.Mana.ToString(); }
            if (m_phantomRPText) { m_phantomRPText.text = (m_currentPartyMember.CurrentStats.Relic - m_currentPartyMember.CurrentRelicPoints).ToString() + " / " + m_currentPartyMember.CurrentStats.Relic.ToString(); }
            if (m_phantomAttackText) { m_phantomAttackText.text = m_currentPartyMember.CurrentStats.Attack.ToString(); }
            if (m_phantomDefenseText) { m_phantomDefenseText.text = m_currentPartyMember.CurrentStats.Defense.ToString(); }


            for (int i = 0; i < m_inventoryAbilities.Length; i++)
            {
                InventoryAbility invAbility = m_inventoryAbilities[i];
                if (i < user.Data.Abilities.Length)
                {
                    invAbility.gameObject.SetActive(true);
                    invAbility.CurrentAbility = user.Data.Abilities[i];
                }
                else
                {
                    invAbility.gameObject.SetActive(false);
                }
            }
            
        }
        Canvas.ForceUpdateCanvases();
    }
}
