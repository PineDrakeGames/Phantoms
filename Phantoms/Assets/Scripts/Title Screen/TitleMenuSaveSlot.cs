using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class TitleMenuSaveSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [Header("Data References")]
    public TitleScreenManager Manager = null;
    public int SlotNumber = 0;

    [Header("Button References")]
    [SerializeField]
    private ShakyImage m_backingImage = null;
    [SerializeField]
    private TMP_Text m_buttonText = null;
    [SerializeField]
    private TMP_Text m_dateText = null;

    [SerializeField]
    private ColorReference m_defaultColor = null;
    [SerializeField]
    private ColorReference m_selectedColor = null;
    [SerializeField]
    private float m_hoveredShakeAmount = 5f;

    public enum SaveSlotState
    {
        DEFAULT,
        HOVERED,
        SELECTED
    }
    private SaveSlotState m_state = SaveSlotState.DEFAULT;
    
    private Button m_button = null;

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Start() 
    {
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(OnClick);
        SetDisplay();
        SetState(SaveSlotState.DEFAULT);
    }

    private void OnDestroy()
    {
        if (m_button)
        {
            m_button.onClick.RemoveListener(OnClick);
        }
    }

    /////////////////////////////////
    /// Hover and Click Functions ///
    /////////////////////////////////
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (m_state == SaveSlotState.DEFAULT)
        {
            SetState(SaveSlotState.HOVERED);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (m_state == SaveSlotState.HOVERED)
        {
            SetState(SaveSlotState.DEFAULT);
        }
    }

    public void OnClick()
    {
        SetState(SaveSlotState.SELECTED);
        Manager.SelectSaveSlot(this);

    }

    ////////////////////////
    /// Public Functions ///
    ////////////////////////
    public void SetDisplay()
    {
        SaveSlot slot = SaveSlotManager.GetSlotData(SlotNumber);
        if (slot != null)
        {
            m_buttonText.text = "Save Slot <b>" + (SlotNumber + 1) + "</b>";
            m_dateText.gameObject.SetActive(true);
            m_dateText.text = slot.LastSave;
        }
        else
        {
            m_buttonText.text = "New Save";
            m_dateText.gameObject.SetActive(false);
        }
    }

    public void SetState(SaveSlotState state)
    {
        switch(state)
        {
            case SaveSlotState.DEFAULT:
                m_backingImage.color = m_defaultColor.Color;
                m_backingImage.ShakeDistance = 0;
                m_button.interactable = true;
                break;
            case SaveSlotState.HOVERED:
                m_backingImage.color = m_defaultColor.Color;
                m_backingImage.ShakeDistance = m_hoveredShakeAmount;
                m_button.interactable = true;
                break;
            case SaveSlotState.SELECTED:
                m_backingImage.color = m_selectedColor.Color;
                m_backingImage.ShakeDistance = m_hoveredShakeAmount;
                m_button.interactable = true;
                break;
        }
        m_state = state;
    }
}
