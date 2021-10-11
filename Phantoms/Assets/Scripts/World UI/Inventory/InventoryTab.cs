using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryTab : MonoBehaviour
{
    [Header("Tab References")]
    [SerializeField]
    protected GameObject m_tabParent = null;

    [Header("Tab Options/Preferences")]
    [SerializeField]
    public InventoryPartyMember.DisplayMode PartyDisplayMode = InventoryPartyMember.DisplayMode.DEFAULT;

    public virtual void OpenTab()
    {
        m_tabParent.SetActive(true);
        if (InventoryUIManager.CurrentTab != this)
        {
            InventoryUIManager.Instance.SetTab(this);
        }
        InventoryPhantomContextMenu.Instance.HideMenu();
        InventoryPhantomDetails.Instance.HideDescription();
    }

    public virtual void CloseTab()
    {
        m_tabParent.SetActive(false);
    }
}
