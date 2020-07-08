using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ares;
using TMPro;

public class BattlePlayerMenu : MonoBehaviour
{
    /// Serialized Fields ///

    [Header("Reference to the main manager")]
    [SerializeField]
    private BattleManager m_battleManager = null;


    [Header("Main menu items")]
    [SerializeField]
    private GameObject m_mainMenuParent = null;


    [Header("Sub-menu items")]
    [SerializeField]
    private GameObject m_subMenuParent = null;
    [SerializeField]
    private GameObject m_content = null;
    [SerializeField]
    private GameObject m_buttonPrefab = null;

    [SerializeField]
    private GameObject m_descriptionObject = null;
    [SerializeField]
    private TextMeshProUGUI m_descriptionText = null;


    private Button m_currentMainMenuButton = null;
    private List<BattleSubmenuButton> m_subMenuButtons = new List<BattleSubmenuButton>();
    private ActionInput m_actionInput;
    private Actor m_currentActor = null;

    private Ability m_currentAbility = null;
    private Item m_currentItem = null;
    private List<Actor> m_actionTargets = new List<Actor>();
    public List<Actor> ActionTargets
    {
        get { return m_actionTargets; }
    }


    private enum BattleMenuState
    {
        INACTIVE,
        MAIN,
        TACTICS,
        ABILITIES,
        ITEMS,
        TARGETING
    }
    private BattleMenuState m_prevState = BattleMenuState.INACTIVE;
    private BattleMenuState m_currentState = BattleMenuState.MAIN;


    private void Start()
    {
        // Test stuff, making a few buttons.
        m_mainMenuParent.SetActive(true);
        HideSubmenu();
        m_currentMainMenuButton = m_mainMenuParent.GetComponentInChildren<Button>();
        m_currentMainMenuButton.Select();
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Public functions to be called by other scripts to update/set data in the battle player menu. ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void SetupMenu(Actor actor, ActionInput actionInput)
    {
        m_actionInput = actionInput;
        m_currentActor = actor;
        ReturnToMainMenu();
    }

    // Used to set the description of the submenu. When given no argument, hides the description box.
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


    /////////////////////////////////////////////////////////////////////////////////
    /// Functions called by the main menu buttons to load the different submenus. ///
    /////////////////////////////////////////////////////////////////////////////////

    public void TacticsMenu()
    {
        m_mainMenuParent.SetActive(false);
        ShowSubmenu();
        ClearSubmenu();

        BattleSubmenuButton submenuButton = null;
        submenuButton = AddSubmenuButton("Run", "Run away from battle");
        submenuButton = AddSubmenuButton("Swap", "Swap turns with your partner");
        submenuButton.ButtonComponent.onClick.AddListener(m_battleManager.SwapTurns);
        submenuButton = AddSubmenuButton("Skip", "Skip your turn");
    }

    public void AbilitiesMenu()
    {
        m_mainMenuParent.SetActive(false);
        ShowSubmenu();
        ClearSubmenu();

        BattleSubmenuButton submenuButton = null;
        foreach (Ability ability in m_actionInput.ValidAbilities)
        {
            submenuButton = AddSubmenuButton(ability.Data.DisplayName, ability.Data.Description);
            submenuButton.ButtonComponent.onClick.AddListener(delegate { TargetMenuAbility(ability); });

        }
    }

    public void ItemsMenu()
    {
        m_mainMenuParent.SetActive(false);
        ShowSubmenu();
        ClearSubmenu();

        BattleSubmenuButton submenuButton = null;
        foreach (Item item in m_actionInput.ValidItems)
        {
            submenuButton = AddSubmenuButton(item.Data.DisplayName, item.Data.Description);
            submenuButton.ButtonComponent.onClick.AddListener(delegate { TargetMenuItem(item); });
        }
    }

    public void TargetMenuAbility(Ability ability)
    {
        ShowSubmenu();
        ClearSubmenu();
        SetDescription();

        m_currentAbility = ability;

        Actor[] validTargets = m_battleManager.CurrentBattle.GetValidTargets(m_currentActor, ability);

        BattleSubmenuButton submenuButton = null;

        switch (ability.Data.TargetType)
        {
            case BattleInteractorData.TargetType.SingleActor:
                // Add a button for each actor, for now.
                foreach (Actor actor in validTargets)
                {
                    submenuButton = AddSubmenuButton(actor.DisplayName, null);
                    submenuButton.ButtonComponent.onClick.AddListener(delegate
                    {
                        SetTarget(actor);
                    });
                }
                break;
            case BattleInteractorData.TargetType.NumberOfActors:
                // Add a button for each actor, with the addition of removing the button when used.
                foreach (Actor actor in validTargets)
                {
                    submenuButton = AddSubmenuButton(actor.DisplayName, null);
                    submenuButton.ButtonComponent.onClick.AddListener(delegate
                    {
                        SetTarget(actor);
                        submenuButton.gameObject.SetActive(false);
                    });
                }
                break;
            case BattleInteractorData.TargetType.AllActorsInGroup:
                // TODO
                break;
            case BattleInteractorData.TargetType.AllActors:
                // Just add all valid targets then do the ability select callback.
                m_actionTargets.AddRange(validTargets);
                m_actionInput.AbilitySelectCallback(ability);
                break;
        }
    }

    public void TargetMenuItem(Item item)
    {
        m_currentItem = item;

        Actor[] validTargets = m_battleManager.CurrentBattle.GetValidTargets(m_currentActor, item);

    }

    public void SetTarget(Actor actor)
    {
        m_actionTargets.Add(actor);
        switch (m_currentAbility.Data.TargetType)
        {
            case BattleInteractorData.TargetType.SingleActor:
                m_actionInput.AbilitySelectCallback(m_currentAbility);
                break;
            case BattleInteractorData.TargetType.NumberOfActors:
                if (m_actionTargets.Count >= m_currentAbility.Data.NumberOfTargets)
                {
                    m_actionInput.AbilitySelectCallback(m_currentAbility);
                }
                break;
        }
    }

    public void ReturnToMainMenu()
    {
        m_mainMenuParent.SetActive(true);
        ClearSubmenu();
        HideSubmenu();
    }


    ///////////////////////////////////////////////////////////
    /// Private helper functions to manage the battle menu. ///
    ///////////////////////////////////////////////////////////

    private BattleSubmenuButton AddSubmenuButton(string name, string description)
    {
        BattleSubmenuButton submenuButton = null;
        foreach (BattleSubmenuButton item in m_subMenuButtons)
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

        // Remove all previous listeners, and add any onclick listeners that all buttons would have.
        submenuButton.ButtonComponent.onClick.RemoveAllListeners();
        submenuButton.ButtonComponent.onClick.AddListener(ClearSubmenu);
        submenuButton.ButtonComponent.onClick.AddListener(HideSubmenu);
        return submenuButton;
    }

    private void ShowSubmenu(bool resetMenu = false)
    {
        m_subMenuParent.SetActive(true);
        m_descriptionObject.SetActive(true);
    }

    private void HideSubmenu()
    {
        m_subMenuParent.SetActive(false);
        m_descriptionObject.SetActive(false);
    }

    private void ClearSubmenu()
    {
        foreach (Transform child in m_content.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
