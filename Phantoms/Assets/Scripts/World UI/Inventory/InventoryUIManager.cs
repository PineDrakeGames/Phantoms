using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_inventoryParent = null;
    
    [SerializeField]
    private List<InventoryTab> m_tabs = null;

    private bool m_isInventoryOpen = false;
    private int m_currentTabIndex = 0;

    private void Start() 
    {
        foreach(InventoryTab tab in m_tabs)
        {
            tab.CloseTab();
        }

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
}
