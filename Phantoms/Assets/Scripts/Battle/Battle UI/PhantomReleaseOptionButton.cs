using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class PhantomReleaseOptionButton : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_phantomName = null;
    [SerializeField]
    private TMP_Text m_phantomLevel = null;

    public BattleResultsManager ResultsManager = null;

    private Button m_buttonComponent = null;
    private PhantomInstanceData m_phantomInstance = null;

    // An added caution to make sure this button can't really be double clicked, to level up twice...
    private bool m_clicked = true;

    private void Awake()
    {
        m_buttonComponent = GetComponent<Button>();
        m_buttonComponent.onClick.AddListener(OnClick);
    }

    private void OnDestroy() {
        if (m_buttonComponent && m_buttonComponent.onClick != null)
        {
            m_buttonComponent.onClick.RemoveListener(OnClick);
        }
    }

    public void OnClick()
    {
        if (!m_clicked)
        {
            m_clicked = true;
            PlayerInventoryManager.Instance.RemovePhantom(m_phantomInstance);

            if (ResultsManager)
            {
                ResultsManager.AdvanceResults();
            }
        }
    }

    public void SetButton(PhantomInstanceData phantom)
    {
        m_phantomInstance = phantom;

        m_phantomName.text = m_phantomInstance.GetDisplayName();
        m_phantomLevel.text = "Level " + m_phantomInstance.Level.ToString();

        m_clicked = false;
    }
}
