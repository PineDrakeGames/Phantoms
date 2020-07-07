using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ares;
using TMPro;

public class BattlePlayerMenu : MonoBehaviour
{
    /// Serialized Fields ///

    [Header("Sub-menu items")]
    [SerializeField]
    private GameObject m_scrollView = null;
    [SerializeField]
    private GameObject m_content = null;
    [SerializeField]
    private GameObject m_buttonPrefab = null;

    [SerializeField]
    private GameObject m_descriptionObject = null;
    [SerializeField]
    private TextMeshProUGUI m_descriptionText = null;


    private List<BattleSubmenuButton> m_subMenuButtons = new List<BattleSubmenuButton>();


    private enum BattleMenuState
    {
        INACTIVE,
        MAIN,
        TACTICS,
        ABILITIES,
        ITEMS,
        TARGETING
    }
    private BattleMenuState m_prevState = BattleMenuState.MAIN;
    private BattleMenuState m_currentState = BattleMenuState.MAIN;


    private void Start() 
    {
        // Test stuff, making a few buttons.
        AddSubmenuButton("Run", "Run away from battle");
        AddSubmenuButton("Swap", "Swap turns with your partner");
        AddSubmenuButton("Skip", "Skip your turn");
        AddSubmenuButton("Test1", "This is a test 1");
        AddSubmenuButton("Test2", "This is a test 2");
        AddSubmenuButton("Test3", "This is a test 3");
        AddSubmenuButton("Test4", "This is a test 4");
        AddSubmenuButton("Test5", "This is a test 5");
        AddSubmenuButton("Test6", "This is a test 6");
        AddSubmenuButton("Test7", "This is a test 7");
        AddSubmenuButton("Test8", "This is a test 8");
        AddSubmenuButton("Test9", "This is a test 9");


    }

    public void SetupMenu(Actor actor, ActionInput actionInput)
    {

    }

    public void TacticsMenu()
    {

    }

    public void SetDescription(string description = null)
    {
        if (string.IsNullOrEmpty(description))
        {
            m_descriptionObject.SetActive(false);
            m_descriptionText.text = string.Empty;
        }
        else
        {
            m_descriptionObject.SetActive(true);
            m_descriptionText.text = description;
        }

    }

    private void AddSubmenuButton(string name, string description)
    {
        BattleSubmenuButton submenuButton = null;
        foreach(BattleSubmenuButton item in m_subMenuButtons)
        {
            if (!item.gameObject.activeSelf)
            {
                submenuButton = item;
                submenuButton.gameObject.SetActive(true);
                break;
            }
        }
        if (submenuButton == null)
        {
            GameObject buttonObject = Instantiate(m_buttonPrefab, m_content.transform);
            submenuButton = buttonObject.GetComponent<BattleSubmenuButton>();
            m_subMenuButtons.Add(submenuButton);
        }

        submenuButton.BattleMenu = this;
        submenuButton.ButtonName = name;
        submenuButton.ButtonDesc = description;
    }

    private void ClearSubmenu()
    {
        foreach (Transform child in m_content.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
