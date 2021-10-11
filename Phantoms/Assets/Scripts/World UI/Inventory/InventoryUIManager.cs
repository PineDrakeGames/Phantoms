using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryUIManager : MonoBehaviour
{
    ////////////////////////
    /// Serialized Items ///
    ////////////////////////
    [Header("Party UI Items")]
    [SerializeField]
    private InventoryPartyMember m_playerMember = null;
    [SerializeField]
    private InventoryPartyMember m_currentPartnerMember = null;
    [SerializeField]
    private List<InventoryPartyMember> m_partyMembers = new List<InventoryPartyMember>();

    [Header("Other UI Items")]
    [SerializeField]
    private GameObject m_inventoryParent = null;

    [SerializeField]
    private List<InventoryTab> m_tabs = null;

    /////////////////////////////
    /// Static Instance stuff ///
    ////////////////////////////
    private static InventoryUIManager s_instance = null;
    public static InventoryUIManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<InventoryUIManager>();
            }
            return s_instance;
        }
    }

    public static InventoryTab CurrentTab
    {
        get
        {
            if (Instance.m_isTabOpen)
            {
                return Instance.m_tabs[Instance.m_currentTabIndex];
            }
            else
            {
                return null;
            }
        }
    }

    /////////////////////////
    /// Private Variables ///
    /////////////////////////
    private bool m_isInventoryOpen = false;
    public bool IsOpen { get { return m_isInventoryOpen; } }
    private bool m_isTabOpen = false;
    public bool IsTabOpen { get { return m_isTabOpen; } }
    private int m_currentTabIndex = 0;

    private InventoryPartyMember m_currentSelectedPartyMember = null;
    public InventoryPartyMember CurrentSelectedPartyMember
    {
        get { return m_currentSelectedPartyMember; }
    }

    private List<UserBattleInstanceData> m_allPartyMembers = new List<UserBattleInstanceData>();
    public List<UserBattleInstanceData> AllPartyMembers { get { return m_allPartyMembers; } }

    public Dictionary<UserBattleInstanceData, InventoryPartyMember> PartyDataToInventory = new Dictionary<UserBattleInstanceData, InventoryPartyMember>();

    /// Public Events ///
    public UnityEvent OnSelectedPartyMemberUpdate = new UnityEvent();

    private InventoryPartyMember.DisplayMode m_currentDisplayMode = InventoryPartyMember.DisplayMode.DEFAULT;

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            s_instance = this;
        }
    }

    private void Start()
    {
        CloseTab();
        SetPartyMembers();
        m_isInventoryOpen = true;
        CloseInventory();
    }

    private void Update()
    {
        // Check for input to open/close inventory
        // TODO: Don't just check keys, go through some input manager thing...
        if (Input.GetKeyDown(KeyCode.Tab) && !PauseMenu.Instance.Paused)
        {
            if (m_isInventoryOpen && m_isTabOpen)
            {
                CloseTab();
            }
            else
            {
                ToggleInventory();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_isTabOpen)
            {
                CloseTab();
            }
            else if (m_isInventoryOpen)
            {
                CloseInventory();
            }
            else
            {
                PauseMenu.Instance.TogglePause();
            }
        }
    }

    ///////////////////////////////////
    /// Public Navigation Functions ///
    ///////////////////////////////////
    public void ToggleInventory()
    {
        if (m_isInventoryOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }

    public void OpenInventory()
    {
        if (m_isInventoryOpen) { return; }

        m_isInventoryOpen = true;
        m_inventoryParent.SetActive(true);
        m_tabs[m_currentTabIndex].OpenTab();
        SetPartyMembers();
        CloseTab();
        Time.timeScale = 0f;
    }

    public void CloseInventory()
    {
        if (!m_isInventoryOpen) { return; }

        m_isInventoryOpen = false;
        m_inventoryParent.SetActive(false);
        m_tabs[m_currentTabIndex].CloseTab();
        Time.timeScale = 1f;
    }

    public void SetTab(int newTabIndex)
    {
        m_isTabOpen = true;

        m_tabs[m_currentTabIndex].CloseTab();
        InventoryPhantomContextMenu.Instance.HideMenu();
        if (newTabIndex < m_tabs.Count && newTabIndex >= 0)
        {
            m_currentTabIndex = newTabIndex;
            m_tabs[m_currentTabIndex].OpenTab();
            m_currentDisplayMode = m_tabs[m_currentTabIndex].PartyDisplayMode;
            Debug.Log("Display mode to " + m_currentDisplayMode.ToString() + ", for tab " + m_tabs[m_currentTabIndex].GetType());
            SetPartyMembers();
        }
        else
        {
            CloseTab();
        }
    }

    public void SetTab(InventoryTab newTab)
    {
        if (newTab == null)
        {
            CloseTab();
            return;
        }
        if (!m_tabs.Contains(newTab))
        {
            m_tabs.Add(newTab);
        }
        SetTab(m_tabs.IndexOf(newTab));
    }

    public void CloseTab()
    {
        m_isTabOpen = false;
        for (int i = 0; i < m_tabs.Count; i++)
        {
            m_tabs[i].CloseTab();
        }
        m_currentDisplayMode = InventoryPartyMember.DisplayMode.DEFAULT;
        SetPartyMembers();
    }

    ////////////////////////////////
    /// Public Utility Functions ///
    ////////////////////////////////

    public void SetPartyMembers()
    {
        m_allPartyMembers.Clear();
        PartyDataToInventory.Clear();

        m_playerMember.PartyMemberData = DataManager.Instance.GetPlayerBattleInstanceData();
        m_playerMember.PartyMemberData.SetCurrentStats();
        m_allPartyMembers.Add(m_playerMember.PartyMemberData);
        PartyDataToInventory.Add(m_playerMember.PartyMemberData, m_playerMember);
        m_playerMember.CurrentDisplay = m_currentDisplayMode;
        m_playerMember.ResetState();


        PhantomInstanceData currentPhantom = PlayerInventoryManager.Instance.GetCurrentPhantom();
        if (currentPhantom != null)
        {
            currentPhantom.SetCurrentStats();
            m_currentPartnerMember.gameObject.SetActive(true);
            m_currentPartnerMember.PartyMemberData = currentPhantom;
            m_allPartyMembers.Add(currentPhantom);
            PartyDataToInventory.Add(currentPhantom, m_currentPartnerMember);
            m_currentPartnerMember.CurrentDisplay = m_currentDisplayMode;
            m_currentPartnerMember.ResetState();

        }
        else
        {
            m_currentPartnerMember.gameObject.SetActive(false);
        }

        // This assumes that the current party member is always the first in the list!!!
        // If that changes this will need reworking.
        for (int i = 0; i < m_partyMembers.Count; i++)
        {
            InventoryPartyMember partyMemberUI = m_partyMembers[i];
            if (partyMemberUI == null) { continue; }
            if (i + 1 < PlayerInventoryManager.Instance.Phantoms.Count)
            {
                partyMemberUI.gameObject.SetActive(true);
                partyMemberUI.PartyMemberData = PlayerInventoryManager.Instance.Phantoms[i + 1];
                partyMemberUI.PartyMemberData.SetCurrentStats();
                m_allPartyMembers.Add(partyMemberUI.PartyMemberData);
                PartyDataToInventory.Add(partyMemberUI.PartyMemberData, partyMemberUI);
                partyMemberUI.CurrentDisplay = m_currentDisplayMode;
                partyMemberUI.ResetState();
            }
            else
            {
                partyMemberUI.gameObject.SetActive(false);
            }
        }
    }

    public void OnPartyMemberClick(InventoryPartyMember partyMember, RectTransform rect)
    {
        if (!m_isTabOpen)
        {
            if (m_currentSelectedPartyMember == partyMember)
            {
                InventoryPhantomContextMenu.Instance.HideMenu();
                m_currentSelectedPartyMember.ResetState();
                m_currentSelectedPartyMember = null;
            }
            else
            {
                if (partyMember != null)
                {
                    partyMember.Select();
                    InventoryPhantomContextMenu.Instance.SetupMenu(partyMember.PartyMemberData, rect);
                }
                if (m_currentSelectedPartyMember != null)
                {
                    m_currentSelectedPartyMember.ResetState();
                }
                m_currentSelectedPartyMember = partyMember;
            }
        }
        else
        {
            // For now, all tabs just need to have party members selected
            if (m_currentSelectedPartyMember == partyMember)
            {
                m_currentSelectedPartyMember.ResetState();
                m_currentSelectedPartyMember = null;
            }
            else
            {
                if (partyMember != null)
                {
                    partyMember.Select();
                }
                if (m_currentSelectedPartyMember != null)
                {
                    m_currentSelectedPartyMember.ResetState();
                }
                m_currentSelectedPartyMember = partyMember;
            }
        }

        OnSelectedPartyMemberUpdate.Invoke();
    }

    public void DeselectPartyMember()
    {
        if (m_currentSelectedPartyMember != null)
        {
            m_currentSelectedPartyMember.ResetState();
            m_currentSelectedPartyMember = null;
            OnSelectedPartyMemberUpdate.Invoke();
        }
    }

}
