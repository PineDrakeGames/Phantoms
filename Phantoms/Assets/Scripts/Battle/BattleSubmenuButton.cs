using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class BattleSubmenuButton : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    [Header("References to items on button.")]
    [SerializeField]
    private TextMeshProUGUI m_buttonText;

    [HideInInspector]
    public BattlePlayerMenu BattleMenu = null;

    private string m_buttonName = null;
    private string m_buttonDesc = null;
    public string ButtonName
    {
        get { return m_buttonName; }
        set
        {
            m_buttonName = value;
            if (m_buttonText) { m_buttonText.text = value; }
        }
    }
    public string ButtonDesc
    {
        get { return m_buttonDesc; }
        set { m_buttonDesc = value; }
    }
    
    private Button m_button;
    public Button ButtonComponent
    {
        get
        {
            if (!m_button)
            {
                m_button = GetComponent<Button>();
            }
            return m_button;
        }
    }

    private void Awake() {
        m_button = GetComponent<Button>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        BattleMenu.SetDescription(m_buttonDesc);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_button.Select();
    }
}
