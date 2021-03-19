using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_inventoryParent = null;
    
    [SerializeField]
    private List<InventoryTab> m_tabs = null;


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
        get { return Instance.m_tabs[Instance.m_currentTabIndex]; }
    }

    private bool m_isInventoryOpen = false;
    private int m_currentTabIndex = 0;

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
        foreach(InventoryTab tab in m_tabs)
        {
            tab.CloseTab();
        }
        m_isInventoryOpen = true;
        CloseInventory();
    }

    private void Update()
    {
        // Check for input to open/close inventory
        // TODO: Don't just check keys, go through some input manager thing...
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_isInventoryOpen)
            {
                CloseInventory();
            }
            else
            {
                Application.Quit();
            }
        }
    }

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
    }

    public void CloseInventory()
    {
        if (!m_isInventoryOpen) { return; }

        m_isInventoryOpen = false;
        m_inventoryParent.SetActive(false);
        m_tabs[m_currentTabIndex].CloseTab();
    }

    public void SetTab(int newTabIndex)
    {
        if (newTabIndex == m_currentTabIndex) { return; }

        m_tabs[m_currentTabIndex].CloseTab();
        if (newTabIndex < m_tabs.Count)
        {
            m_currentTabIndex = newTabIndex;
            m_tabs[m_currentTabIndex].OpenTab();
        }
    }

    public void SetTab(InventoryTab newTab)
    {
        if (newTab == null)
        {
            return;
        }
        if (!m_tabs.Contains(newTab))
        {
            m_tabs.Add(newTab);
        }
        SetTab(m_tabs.IndexOf(newTab));
    }
}
