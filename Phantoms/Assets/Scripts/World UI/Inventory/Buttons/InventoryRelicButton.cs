using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class InventoryRelicButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button References")]
    [SerializeField]
    private Image m_relicIcon = null;
    [SerializeField]
    private GameObject m_currentRelicEquipParent = null;
    [SerializeField]
    private TextMeshProUGUI m_currentRelicEquipText = null;
    [SerializeField]
    private List<GameObject> m_RPCostTicks = new List<GameObject>();

    public InventoryUIRelicsTab RelicsInventory = null;
    public RelicInstance Data = null;

    private Button m_buttonComponent = null;

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Awake()
    {
        m_buttonComponent = GetComponent<Button>();
        m_buttonComponent.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        if (m_buttonComponent)
        {
            m_buttonComponent.onClick.RemoveAllListeners();
        }
    }

    ///////////////////////////////////////////////////////////////////////////
    /// Public functions for clicking, hovering, and setting up the button. ///
    ///////////////////////////////////////////////////////////////////////////
    public void OnClick()
    {
        if (RelicsInventory && Data != null)
        {
            // Select Relic!
        }
    }

    public void SetButton()
    {
        if (Data != null)
        {
            if (m_relicIcon != null)
            {
                m_relicIcon.sprite = Data.Data.Icon;
            }
            if (m_currentRelicEquipText != null && m_currentRelicEquipParent != null)
            {
                if (Data.Equipped)
                {
                    m_currentRelicEquipParent.SetActive(true);
                    m_currentRelicEquipText.text = Data.User.GetDisplayName();
                }
                else
                {
                    m_currentRelicEquipParent.SetActive(false);
                }
            }
            int index = 0;
            for(index = 0; index < Data.Data.Points && index < m_RPCostTicks.Count; index++)
            {
                m_RPCostTicks[index].SetActive(true);
            }
            while (index < m_RPCostTicks.Count)
            {
                m_RPCostTicks[index].SetActive(false);
                index++;
            }
        }
    }

    public void OnHover()
    {
        // future animation stuff?
    }

    public void OnStopHover()
    {
        // future animation stuff?
    }

    //////////////////////////
    /// IPointer functions ///
    //////////////////////////
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OnStopHover();
    }
}
