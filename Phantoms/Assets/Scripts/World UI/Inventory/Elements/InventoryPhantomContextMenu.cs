using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPhantomContextMenu : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField]
    private GameObject m_menuParent = null;
    [SerializeField]
    private RectTransform m_gameCanvas = null;

    [Header("Button References")]
    [SerializeField]
    private GameObject m_setAsPartnerButton = null;
    [SerializeField]
    private GameObject m_detailsButton = null;
    [SerializeField]
    private GameObject m_useItemButton = null;

    private static InventoryPhantomContextMenu s_instance = null;
    public static InventoryPhantomContextMenu Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<InventoryPhantomContextMenu>();
            }
            return s_instance;
        }
    }

    private RectTransform m_rect = null;
    private UserBattleInstanceData m_currentUser = null;
    private RectTransform m_currentPartyMemberUI = null;

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }

        m_rect = GetComponent<RectTransform>();

        HideMenu();
    }

    public void ShowMenu()
    {
        m_menuParent.SetActive(true);
    }

    public void HideMenu()
    {
        m_menuParent.SetActive(false);
    }

    public void SetupMenu(UserBattleInstanceData user, RectTransform partymemberUI)
    {
        if (user == m_currentUser && m_menuParent.activeSelf)
        {
            InventoryUIManager.Instance.DeselectPartyMember();
            HideMenu();
            return;
        }

        ShowMenu();

        m_currentUser = user;
        m_currentPartyMemberUI = partymemberUI;

        // Check if the Set As Partner button should be available
        if (user.GetType() == typeof(PhantomInstanceData) && user != PlayerInventoryManager.Instance.GetCurrentPhantom())
        {
            m_setAsPartnerButton.SetActive(true);
        }
        else
        {
            m_setAsPartnerButton.SetActive(false);
        }

        // Move the menu to be next to the rect transform
        Vector3[] corners = new Vector3[4];
        partymemberUI.GetWorldCorners(corners);

        Vector2 canvasPos = m_gameCanvas.InverseTransformPoint(Vector3.Lerp(corners[0], corners[2], 0.5f));
        Vector2 menuPos = new Vector2();
        menuPos.y = canvasPos.y;
        menuPos.x = canvasPos.x + (partymemberUI.rect.width/2f) + 20f + (m_rect.rect.width/2f);
        m_rect.anchoredPosition = menuPos;

        Debug.Log(partymemberUI.anchoredPosition + ", " + menuPos);
    }

    public void ShowDetails()
    {
        if (m_currentUser != null)
        {
            InventoryPhantomDetails.Instance.UpdatePhantomDisplay(m_currentUser);
        }
        HideMenu();
    }

    public void SetAsPartner()
    {
        if (m_currentUser != null && m_currentUser.GetType() == typeof(PhantomInstanceData) && m_currentUser != PlayerInventoryManager.Instance.GetCurrentPhantom())
        {
            PlayerInventoryManager.Instance.SetCurrentPhantom(m_currentUser as PhantomInstanceData);
            InventoryUIManager.Instance.SetPartyMembers();
        }

        HideMenu();
    }
}
