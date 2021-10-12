using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class InventoryPartyMember : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    ////////////////////////
    /// Serialize Fields ///
    ////////////////////////
    [Header("UI References")]
    [SerializeField]
    private Image m_iconFill = null;
    [SerializeField]
    private Image m_iconLines = null;
    [SerializeField]
    private TMP_Text m_nameText = null;

    [SerializeField]
    private UIPhantomTypeDisplay m_typeOneDisplayer = null;
    [SerializeField]
    private UIPhantomTypeDisplay m_typeTwoDisplayer = null;

    [Header("HP and MP Display")]
    [SerializeField]
    private GameObject m_healthAndManaDisplayParent = null;
    [SerializeField]
    private Image m_HPImageFill = null;
    [SerializeField]
    private TMP_Text m_HPText = null;
    [SerializeField]
    private Image m_MPImageFill = null;
    [SerializeField]
    private TMP_Text m_MPText = null;
    [SerializeField]
    private TMP_Text m_levelText = null;

    [Header("RP Display")]
    [SerializeField]
    private GameObject m_relicDisplayParent = null;
    [SerializeField]
    private TMP_Text m_availableRelicText = null;
    [SerializeField]
    private List<InventoryRelicTick> m_relicTicks = null;


    [Header("Selection UI Items")]
    [SerializeField]
    private CanvasGroup m_canvasGroup = null;
    [SerializeField]
    private Image m_backingImage = null;
    [SerializeField]
    private Color m_defaultColor = Color.white;
    [SerializeField]
    private Color m_hoveredColor = Color.white;
    [SerializeField]
    private Color m_selectedColor = Color.white;
    [SerializeField]
    private float m_disabledAlpha = 0.7f;

    ///////////////////
    /// States Data ///
    ///////////////////
    public enum InventoryPartyMemberState
    {
        DEFAULT,
        DISABLED,
        HOVERED,
        SELECTED
    }

    private InventoryPartyMemberState m_state = InventoryPartyMemberState.DEFAULT;

    public enum DisplayMode
    {
        DEFAULT,
        RELIC
    }

    private DisplayMode m_displayMode = DisplayMode.DEFAULT;
    public DisplayMode CurrentDisplay
    {
        get { return m_displayMode; }
        set { SetDisplayMode(value); }
    }

    //////////////////////////
    /// Private References ///
    //////////////////////////
    private UserBattleInstanceData m_partyMemberData = null;

    public UserBattleInstanceData PartyMemberData
    {
        get { return m_partyMemberData; }
        set { SetPartyMember(value); }
    }

    private Button m_button = null;
    private RectTransform m_rect = null;


    ///////////////////////
    /// Unity Functions ///
    ///////////////////////

    private void Awake()
    {
        m_rect = GetComponent<RectTransform>();
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(OnClick);

        ResetState();
    }

    private void OnDestroy()
    {
        if (m_button)
        {
            m_button.onClick.RemoveListener(OnClick);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (m_state == InventoryPartyMemberState.DEFAULT)
        {
            SetState(InventoryPartyMemberState.HOVERED);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (m_state == InventoryPartyMemberState.HOVERED)
        {
            SetState(InventoryPartyMemberState.DEFAULT);
        }
    }


    ////////////////////////
    /// Public Functions ///
    ////////////////////////
    public void SetPartyMember(UserBattleInstanceData data)
    {
        if (data == m_partyMemberData)
        {
            UpdateDisplay();
            return;
        }

        if (m_partyMemberData != null)
        {
            m_partyMemberData.OnStatsUpdate.RemoveListener(UpdateUI);
        }
        if (data != null)
        {
            data.OnStatsUpdate.AddListener(UpdateUI);
        }
        m_partyMemberData = data;
        UpdateUI();
        UpdateDisplay();
    }

    public void UpdateUI()
    {
        if (m_partyMemberData == null)
        {
            // TODO: Hide everything maybe?
            return;
        }

        // Not all of these UI references might be here, just update the ones that are.
        if (m_iconFill) { m_iconFill.sprite = m_partyMemberData.Data.IconFill; }
        if (m_iconLines) { m_iconLines.sprite = m_partyMemberData.Data.IconLines; }
        if (m_nameText) { m_nameText.text = m_partyMemberData.GetDisplayName(); }
        if (m_levelText)
        {
            m_levelText.text = "Level: " + m_partyMemberData.Level;
        }

        if (m_partyMemberData is PhantomInstanceData)
        {
            PhantomInstanceData phantom = m_partyMemberData as PhantomInstanceData;
            if (m_typeOneDisplayer != null)
            {
                if (phantom.PhanData.MainType != PhantomType.NONE)
                {
                    m_typeOneDisplayer.gameObject.SetActive(true);
                    m_typeOneDisplayer.Type = phantom.PhanData.MainType;
                }
                else
                {
                    m_typeOneDisplayer.gameObject.SetActive(false);
                }
            }

            if (m_typeTwoDisplayer != null)
            {
                if (phantom.PhanData.SecondType != PhantomType.NONE)
                {
                    m_typeTwoDisplayer.gameObject.SetActive(true);
                    m_typeTwoDisplayer.Type = phantom.PhanData.SecondType;
                }
                else
                {
                    m_typeTwoDisplayer.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if (m_typeOneDisplayer != null)
            {
                m_typeOneDisplayer.gameObject.SetActive(false);
            }

            if (m_typeTwoDisplayer != null)
            {
                m_typeTwoDisplayer.gameObject.SetActive(false);
            }
        }
    }

    public void UpdateDisplay()
    {
        switch (m_displayMode)
        {
            case DisplayMode.DEFAULT:
                m_healthAndManaDisplayParent.SetActive(true);
                m_relicDisplayParent.SetActive(false);

                if (m_HPImageFill)
                {
                    m_HPImageFill.fillAmount = Mathf.Clamp01((float)m_partyMemberData.CurrentHP / (float)m_partyMemberData.CurrentStats.MaxHP);
                }
                if (m_HPText)
                {
                    m_HPText.text = m_partyMemberData.CurrentHP.ToString() + "/" + m_partyMemberData.CurrentStats.MaxHP.ToString();
                }
                if (m_MPImageFill)
                {
                    m_MPImageFill.fillAmount = Mathf.Clamp01((float)m_partyMemberData.CurrentMana / (float)m_partyMemberData.CurrentStats.Mana);
                }
                if (m_MPText)
                {
                    m_MPText.text = m_partyMemberData.CurrentMana.ToString() + "/" + m_partyMemberData.CurrentStats.Mana.ToString();
                }
                break;

            case DisplayMode.RELIC:
                m_healthAndManaDisplayParent.SetActive(false);
                m_relicDisplayParent.SetActive(true);

                int totalRelicPoints = m_partyMemberData.CurrentStats.Relic;
                int availableRelicPoints = m_partyMemberData.CurrentStats.Relic - m_partyMemberData.CurrentRelicPoints;
                if (m_availableRelicText) { m_availableRelicText.text = availableRelicPoints + "/" + totalRelicPoints; }

                for (int i = 0; i < m_relicTicks.Count; i++)
                {
                    InventoryRelicTick relicTick = m_relicTicks[i];
                    if (i < totalRelicPoints)
                    {
                        relicTick.gameObject.SetActive(true);
                        relicTick.SetTick(i < availableRelicPoints);
                    }
                    else
                    {
                        relicTick.gameObject.SetActive(false);
                    }
                }

                break;
        }
    }

    public void OnClick()
    {
        if (m_state != InventoryPartyMemberState.DISABLED)
        {
            InventoryUIManager.Instance.OnPartyMemberClick(this, m_rect);
        }
    }

    public void ResetState()
    {
        SetState(InventoryPartyMemberState.DEFAULT);
    }

    public void Disable()
    {
        SetState(InventoryPartyMemberState.DISABLED);
    }

    public void Select()
    {
        SetState(InventoryPartyMemberState.SELECTED);
    }

    public void SetState(InventoryPartyMemberState newState)
    {
        switch (newState)
        {
            case InventoryPartyMemberState.DEFAULT:
                m_canvasGroup.alpha = 1f;
                m_backingImage.color = m_defaultColor;
                m_button.interactable = true;
                break;
            case InventoryPartyMemberState.DISABLED:
                m_canvasGroup.alpha = m_disabledAlpha;
                m_backingImage.color = m_defaultColor;
                m_button.interactable = false;
                break;
            case InventoryPartyMemberState.HOVERED:
                m_canvasGroup.alpha = 1f;
                m_backingImage.color = m_hoveredColor;
                m_button.interactable = true;
                break;
            case InventoryPartyMemberState.SELECTED:
                m_canvasGroup.alpha = 1f;
                m_backingImage.color = m_selectedColor;
                m_button.interactable = true;
                break;
        }

        m_state = newState;
    }

    public void SetDisplayMode(DisplayMode newDisplay)
    {
        m_displayMode = newDisplay;

        UpdateDisplay();
    }
}
