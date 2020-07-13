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

    // Reference to each of the main menu buttons, both for disabling them when the option is not available, and for menu navigation.
    [SerializeField]
    private Button m_abilitiesButton = null;
    [SerializeField]
    private Button m_itemsButton = null;
    [SerializeField]
    private Button m_tacticsButton = null;


    [Header("Sub-menu items")]
    [SerializeField]
    private GameObject m_subMenuParent = null;
    [SerializeField]
    private GameObject m_content = null;
    [SerializeField]
    private Button m_backButton = null;
    [SerializeField]
    private GameObject m_buttonPrefab = null;

    [SerializeField]
    private GameObject m_descriptionObject = null;
    [SerializeField]
    private TextMeshProUGUI m_descriptionText = null;

    [SerializeField]
    private GameObject m_arrowIndicator = null;


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
        m_mainMenuParent.SetActive(false);
        SetArrowIndicator();
        HideSubmenu();
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

    public void SetArrowIndicator(Actor actor = null)
    {
        if (actor == null)
        {
            m_arrowIndicator.SetActive(false);
        }
        else
        {
            m_arrowIndicator.SetActive(true);
            // TODO: Either set offset in prefab or in data
            m_arrowIndicator.transform.position = actor.transform.position + (Vector3.up * 1.5f);
        }
    }


    /////////////////////////////////////////////////////////////////////////////////
    /// Functions called by the main menu buttons to load the different submenus. ///
    /////////////////////////////////////////////////////////////////////////////////

    public void TacticsMenu()
    {
        SetState(BattleMenuState.TACTICS);

        ClearSubmenu();

        BattleSubmenuButton submenuButton = null;
        submenuButton = AddSubmenuButton("Run", "Run away from battle");
        // TODO: Run?
        submenuButton = AddSubmenuButton("Swap", "Swap turns with your partner");
        submenuButton.ButtonComponent.onClick.AddListener(m_battleManager.SwapTurns);
        submenuButton = AddSubmenuButton("Skip", "Skip your turn");
        submenuButton.ButtonComponent.onClick.AddListener(delegate { m_actionInput.SkipCallback(); });

        ShowSubmenu();
    }

    public void AbilitiesMenu()
    {
        SetState(BattleMenuState.ABILITIES);

        ClearSubmenu();

        BattleSubmenuButton submenuButton = null;
        foreach (Ability ability in m_actionInput.ValidAbilities)
        {
            submenuButton = AddSubmenuButton(ability.Data.DisplayName, ability.Data.Description);
            submenuButton.ClickEvent.AddListener(delegate { TargetMenuAbility(ability); });
        }

        ShowSubmenu();
    }

    public void ItemsMenu()
    {
        SetState(BattleMenuState.ITEMS);

        ClearSubmenu();

        BattleSubmenuButton submenuButton = null;
        foreach (Item item in m_actionInput.ValidItems)
        {
            submenuButton = AddSubmenuButton(item.Data.DisplayName, item.Data.Description);
            submenuButton.ClickEvent.AddListener(delegate { TargetMenuItem(item); });
        }

        ShowSubmenu();
    }

    public void TargetMenuAbility(Ability ability)
    {
        SetState(BattleMenuState.TARGETING);

        ClearSubmenu();

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
                    submenuButton.ClickEvent.AddListener(delegate { SetTarget(actor); });
                    submenuButton.SelectEvent.AddListener( delegate {SetArrowIndicator(actor); });
                }
                break;
            case BattleInteractorData.TargetType.NumberOfActors:
                // Add a button for each actor, with the addition of removing the button when used.
                foreach (Actor actor in validTargets)
                {
                    submenuButton = AddSubmenuButton(actor.DisplayName, null);
                    submenuButton.ClickEvent.AddListener(delegate
                    {
                        SetTarget(actor);
                        submenuButton.gameObject.SetActive(false);
                    });
                    submenuButton.SelectEvent.AddListener( delegate {SetArrowIndicator(actor); });
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

        ShowSubmenu();
    }

    public void TargetMenuItem(Item item)
    {
        SetState(BattleMenuState.TARGETING);

        ClearSubmenu();

        m_currentItem = item;

        Actor[] validTargets = m_battleManager.CurrentBattle.GetValidTargets(m_currentActor, item);

        ShowSubmenu();
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

    public void ReturnToPrevMenu()
    {
        switch (m_currentState)
        {
            case BattleMenuState.MAIN:
                return;
            case BattleMenuState.ABILITIES:
            case BattleMenuState.TACTICS:
            case BattleMenuState.ITEMS:
                ReturnToMainMenu();
                return;
            case BattleMenuState.TARGETING:
                SetArrowIndicator();
                switch (m_prevState)
                {
                    case BattleMenuState.ABILITIES:
                        AbilitiesMenu();
                        break;
                    case BattleMenuState.TACTICS:
                        TacticsMenu();
                        break;
                    case BattleMenuState.ITEMS:
                        ItemsMenu();
                        break;
                }
                return;
        }
    }


    public void ReturnToMainMenu()
    {
        SetState(BattleMenuState.MAIN);
        m_mainMenuParent.SetActive(true);
        ClearSubmenu();
        HideSubmenu();

        switch (m_prevState)
        {
            case BattleMenuState.TACTICS:
                m_tacticsButton.Select();
                break;
            case BattleMenuState.ITEMS:
                m_itemsButton.Select();
                break;
            default:
                // Both if we were previously in the abilities menu, or from any other menu.
                m_abilitiesButton.Select();
                break;
        }
    }


    ///////////////////////////////////////////////////////////
    /// Private helper functions to manage the battle menu. ///
    ///////////////////////////////////////////////////////////

    private void SetState(BattleMenuState newState)
    {
        m_prevState = m_currentState;
        m_currentState = newState;
    }

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
        submenuButton.ClickEvent.RemoveAllListeners();
        submenuButton.SelectEvent.RemoveAllListeners();

        submenuButton.ClickEvent.AddListener(ClearSubmenu);
        submenuButton.ClickEvent.AddListener(HideSubmenu);
        submenuButton.SelectEvent.AddListener( delegate {SetDescription(description); });
        return submenuButton;
    }

    private void ShowSubmenu()
    {
        m_mainMenuParent.SetActive(false);
        m_subMenuParent.SetActive(true);
        m_descriptionObject.SetActive(true);

        foreach (BattleSubmenuButton submenuButton in m_subMenuButtons)
        {
            if (submenuButton.gameObject.activeSelf)
            {
                submenuButton.ButtonComponent.Select();
                return;
            }
        }

        m_backButton.Select();
        SetDescription();
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
