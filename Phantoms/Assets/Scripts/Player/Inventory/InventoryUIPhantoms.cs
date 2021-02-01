using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUIPhantoms : InventoryTab
{
    [Header("Prefab References")]
    [SerializeField]
    private GameObject m_phantomButtonPrefab = null;

    [Header("Scene References")]
    [SerializeField]
    private Transform m_phantomButtonParent = null;

    [Header("Current Phantom Selection Things")]
    [SerializeField]
    private TextMeshProUGUI m_phantomNicknameText = null;
    [SerializeField]
    private TextMeshProUGUI m_phantomBackgroundText = null;
    [SerializeField]
    private TextMeshProUGUI m_phantomDescriptionText = null;

    // private variables
    private List<InventoryPhantomButton> m_phantomButtons = new List<InventoryPhantomButton>();
    private PhantomInstanceData m_currentPhantom = null;

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
        foreach(InventoryPhantomButton button in m_phantomButtons)
        {
            button.gameObject.SetActive(false);
        }
        foreach(PhantomInstanceData data in PlayerInventoryManager.Instance.Phantoms)
        {
            InventoryPhantomButton phantomButton = GetButton();
            phantomButton.Data = data;
            phantomButton.SetButton();
        }
    }

    public void SelectPhantom(PhantomInstanceData data)
    {
        if (data != null && data != m_currentPhantom)
        {
            m_currentPhantom = data;
            UpdatePhantomDisplay();
        }
    }

    public void UpdatePhantomDisplay()
    {
        if (m_currentPhantom != null)
        {
            m_currentPhantom.SetCurrentStats();

            // Set name
            string name = "No Name";
            if (!string.IsNullOrEmpty(m_currentPhantom.NickName)) { name = m_currentPhantom.NickName; }
            else if (!string.IsNullOrEmpty(m_currentPhantom.PhanData.PhantomDisplayName)) { name = m_currentPhantom.PhanData.PhantomDisplayName; }
            name += " - " + m_currentPhantom.Level;
            m_phantomNicknameText.text = name;

            // Set background
            m_phantomBackgroundText.text = m_currentPhantom.Background.ToString();

            // Set Description
            /// Mostly just filling this with a bunch of info for now to see how things are working.
            string description = "";
            description += m_currentPhantom.PhanData.PhantomDescription + "\n\n";
            description += "HP: " + m_currentPhantom.CurrentHP + "/" + m_currentPhantom.CurrentStats.MaxHP + " (Level ups: " + m_currentPhantom.LevelUps.MaxHP + ")\n";
            description += "Attack: " + m_currentPhantom.CurrentStats.Attack + " (Level ups: " + m_currentPhantom.LevelUps.Attack + ")\n";
            description += "Defense: " + m_currentPhantom.CurrentStats.Defense + " (Level ups: " + m_currentPhantom.LevelUps.Defense + ")\n";
            description += "Mana: " + m_currentPhantom.CurrentStats.Mana + " (Level ups: " + m_currentPhantom.LevelUps.Mana + ")\n";
            description += "Relic: " + m_currentPhantom.CurrentStats.Relic + " (Level ups: " + m_currentPhantom.LevelUps.Relic + ")\n";
            m_phantomDescriptionText.text = description;
        }
    }

    // Private Helper Functions
    private InventoryPhantomButton GetButton()
    {
        InventoryPhantomButton returnButton = null;

        foreach(InventoryPhantomButton button in m_phantomButtons)
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
            GameObject instancedButton = Instantiate(m_phantomButtonPrefab, m_phantomButtonParent);
            returnButton = instancedButton.GetComponent<InventoryPhantomButton>();
            returnButton.PhantomsInventory = this;
            m_phantomButtons.Add(returnButton);
        }

        return returnButton;
    }
}
