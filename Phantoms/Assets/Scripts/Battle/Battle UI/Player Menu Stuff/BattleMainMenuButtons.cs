using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleMainMenuButtons : MonoBehaviour
{
    [System.Serializable]
    private class BattleMainButton
    {
        public Button ButtonComponent = null;
        public Animator AnimatorComponent = null;
        public Image LineImage = null;
        public ShakyImage BoxImage = null;
    }

    [SerializeField]
    [Tooltip("List from bottom to top!")]
    private BattleMainButton[] m_buttons = null;

    [SerializeField]
    private Color m_selectedColor = Color.white;

    [SerializeField]
    private Color m_furthestColor = Color.magenta;

    [SerializeField]
    private Color m_disabledColor = Color.gray;

    public void OnButtonSelect(GameObject buttonObject)
    {
        if (buttonObject != null)
        {
            Button button = buttonObject.GetComponent<Button>();
            if (button)
            {
                SetCurrentButton(button);
            }
        }
    }

    public void SetCurrentButton(Button button)
    {
        if (button == null || !button.interactable) { return; }

        for (int i = 0; i < m_buttons.Length; i++)
        {
            if (m_buttons[i].ButtonComponent == button)
            {
                SetCurrentButton(i);
                break;
            }
        }
    }

    public void SetCurrentButton(int index)
    {
        if (index < 0 || index >= m_buttons.Length)
        {
            return;
        }

        for (int i = 0; i < m_buttons.Length; i++)
        {


            float distance = Mathf.Clamp01(Mathf.Abs(((float)i - (float)index) / (float)m_buttons.Length));
            Color col = Color.Lerp(m_selectedColor, m_furthestColor, distance);

            if (!m_buttons[i].ButtonComponent.interactable)
            {
                col = Color.Lerp(col, m_disabledColor, 0.5f);
            }

            m_buttons[i].LineImage.color = col;
            m_buttons[i].BoxImage.color = col;
            if (i == index)
            {
                m_buttons[i].BoxImage.ShakeDistance = 0f;
            }
            else
            {
                m_buttons[i].BoxImage.ShakeDistance = 2f;
            }

            if (i <= index)
            {
                m_buttons[i].ButtonComponent.transform.SetAsLastSibling();
            }
            else
            {
                m_buttons[i].ButtonComponent.transform.SetAsFirstSibling();
            }

            if (m_buttons[i].AnimatorComponent != null)
            {
                if (i == index)
                {
                    m_buttons[i].AnimatorComponent.SetBool("Selected", true);
                }
                else
                {
                    m_buttons[i].AnimatorComponent.SetBool("Selected", false);
                }
            }
        }
    }
}
