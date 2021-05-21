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
        public Image ImageComponent = null;
    }

    [SerializeField]
    [Tooltip("List from bottom to top!")]
    private BattleMainButton[] m_buttons = null;

    [SerializeField]
    private Color m_selectedColor = Color.white;

    [SerializeField]
    private Color m_furthestColor = Color.magenta;

    private void Start()
    {
        SetCurrentButton(0);
    }

    public void OnButtonSelect(GameObject buttonObject)
    {
        if (buttonObject != null)
        {
            Button button = buttonObject.GetComponent<Button>();
            if (button)
            {
                SetCurrentButton(button);
                Debug.Log(button.name);
            }
        }
    }

    public void SetCurrentButton(Button button)
    {
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
            m_buttons[i].ImageComponent.color = col;

            if (i <= index)
            {
                m_buttons[i].ButtonComponent.transform.SetAsLastSibling();
            }
            else
            {
                m_buttons[i].ButtonComponent.transform.SetAsFirstSibling();
            }
        }
    }
}
