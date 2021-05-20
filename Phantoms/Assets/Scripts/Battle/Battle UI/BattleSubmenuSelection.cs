using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleSubmenuSelection : MonoBehaviour
{
    [SerializeField]
    private Animator m_submenuSelectionAnimator = null;

    [SerializeField]
    private TextMeshProUGUI m_buttonText = null;
    [SerializeField]
    private TextMeshProUGUI m_buttonInfoText = null;
    [SerializeField]
    private TextMeshProUGUI m_buttonDescriptionText = null;

    private RectTransform rect = null;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void SetButtonSelection(BattleSubmenuButton button)
    {
        if (button != null)
        {
            rect.anchoredPosition = button.GetComponent<RectTransform>().anchoredPosition;
            m_buttonText.text = button.ButtonName;
            m_buttonInfoText.text = button.ButtonInfo;
            m_buttonDescriptionText.text = button.ButtonDesc;
            m_submenuSelectionAnimator.SetTrigger("Hover");
        }
        else
        {
            m_submenuSelectionAnimator.SetTrigger("Off");
        }
    }
}
