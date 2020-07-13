using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(Button))]
public class BattleSubmenuButton : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    [Header("References to items on button.")]
    [SerializeField]
    private TextMeshProUGUI m_buttonText = null;

    [HideInInspector]
    public BattlePlayerMenu BattleMenu = null;

    public UnityEvent ClickEvent = new UnityEvent();
    public UnityEvent SelectEvent = new UnityEvent();

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

    private void Awake() 
    {
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(OnClick);
    }

    private void OnDestroy() 
    {
        m_button.onClick.RemoveListener(OnClick);
        ClickEvent.RemoveAllListeners();
        SelectEvent.RemoveAllListeners();
    }

    public void OnClick()
    {
        ClickEvent.Invoke();
    }

    public void OnSelect(BaseEventData eventData)
    {
        SelectEvent.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_button.Select();
    }
}
