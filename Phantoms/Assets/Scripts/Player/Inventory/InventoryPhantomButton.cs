using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class InventoryPhantomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button References")]
    [SerializeField]
    private TextMeshProUGUI m_buttonText = null;

    public InventoryUIPhantoms PhantomsInventory = null;
    public PhantomInstanceData Data = null;

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
        if (PhantomsInventory && Data != null)
        {
            PhantomsInventory.SelectPhantom(Data);
        }
    }

    public void SetButton()
    {
        if (Data != null)
        {
            string name = "No Name";
            if (!string.IsNullOrEmpty(Data.NickName)) { name = Data.NickName; }
            else if (!string.IsNullOrEmpty(Data.PhanData.PhantomDisplayName)) { name = Data.PhanData.PhantomDisplayName; }
            m_buttonText.text = name;
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
