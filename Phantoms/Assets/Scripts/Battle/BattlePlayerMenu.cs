using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Ares;
using TMPro;

public class BattlePlayerMenu : MonoBehaviour
{
    /// Serialized Fields ///

    [Header("Reference to the main manager")]
    [SerializeField]
    private BattleManager m_battleManager = null;

    [SerializeField]
    private BattleCameraManager m_battleCamera = null;

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

    [Header("UI Items")]
    [SerializeField]
    private Transform m_playerHealthIndicators = null;
    [SerializeField]
    private Transform m_enemyHealthIndicators = null;
    [SerializeField]
    private GameObject m_healthIndicator = null;

    [Header("3D Indicators")]
    [SerializeField]
    private GameObject m_arrowIndicator = null;
    [SerializeField]
    private GameObject m_currentTurnIndicator = null;
    [SerializeField]
    private GameObject m_damageIndicator = null;
    [SerializeField]
    private Animator m_damageIndicatorAnimation = null;
    [SerializeField]
    private TextMeshPro m_damageIndicatorText = null;

    public Dictionary<Actor, HealthIndicator> ActorToHealthIndicator = new Dictionary<Actor, HealthIndicator>();

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
    private List<BattleGroup> m_actionGroupTargets = new List<BattleGroup>();
    public List<BattleGroup> ActionGroupTargets
    {
        get { return m_actionGroupTargets; }
    }

    private Vector3 enemyCenter = Vector3.zero;


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

    private enum ActionType
    {
        ABILITY,
        ITEM
    }

    private static BattlePlayerMenu s_instance = null;
    public static BattlePlayerMenu Instance { get { return s_instance; } }


    public void OnBattleStart()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }

        // Test stuff, making a few buttons.
        m_mainMenuParent.SetActive(false);
        SetArrowIndicator();
        HideSubmenu();

        m_battleManager.CurrentBattle.OnTurnStart.AddListener(SetCurrentTurnIndicator);

        foreach (Actor actor in m_battleManager.CurrentBattle.Actors)
        {
            if (m_battleManager.CurrentBattle.ActorInfo[actor].IsParticipating)
            {
                GameObject gameObj = null;
                HealthIndicator indicator = null;
                if (actor.Group.Name == "Player")
                {
                    gameObj = Instantiate(m_healthIndicator, m_playerHealthIndicators);
                }
                else
                {
                    gameObj = Instantiate(m_healthIndicator, m_enemyHealthIndicators);
                }
                indicator = gameObj.GetComponent<HealthIndicator>();
                indicator.Actor = actor;
                indicator.BattleStart();
                ActorToHealthIndicator.Add(actor, indicator);
            }
        }

        enemyCenter = Vector3.zero;
        int totalEnemies = 0;
        foreach (Actor enemy in m_battleManager.CurrentBattle.Actors)
        {
            if (enemy.Group.Name != "Player")
            {
                enemyCenter += enemy.transform.position;
                totalEnemies += 1;
            }
        }
        enemyCenter /= totalEnemies;
    }

    private void OnDestroy()
    {
        if (m_battleManager && m_battleManager.CurrentBattle != null)
        {
            m_battleManager.CurrentBattle.OnTurnStart.RemoveListener(SetCurrentTurnIndicator);
        }
    }

    /////////////////////////////////
    /// Public UI event Listeners ///
    /////////////////////////////////
    public void SetCurrentTurnIndicator(Actor actor)
    {
        if (m_currentTurnIndicator && actor)
        {
            m_currentTurnIndicator.transform.position = actor.gameObject.transform.position;
        }
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Public functions to be called by other scripts to update/set data in the battle player menu. ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void SetupMenu(Actor actor, ActionInput actionInput)
    {
        m_actionInput = actionInput;
        m_currentActor = actor;


        m_battleCamera.SetCameraOverShoulder(actor.transform.position, enemyCenter);
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

    public void SetDamageIndicator(Vector3 position, int damage)
    {
        m_damageIndicator.transform.position = position;
        m_damageIndicatorText.text = damage.ToString();
        m_damageIndicatorAnimation.SetTrigger("Play");
    }


    /////////////////////////////////////////////////////////////////////////////////
    /// Functions called by the main menu buttons to load the different submenus. ///
    /////////////////////////////////////////////////////////////////////////////////

    // Sets up the tactics menu - adds options based on what's available.
    public void TacticsMenu()
    {
        SetState(BattleMenuState.TACTICS);

        ClearSubmenu();

        BattleSubmenuButton submenuButton = null;
        
        // Catch button (if available)
        if (CanCatch())
        {
            submenuButton = AddSubmenuButton("Catch", "Catch that phantom!");
            submenuButton.ClickEvent.AddListener(m_battleManager.CatchPhantom);
        }

        // Check if swapping turns is an option first.
        if (CanSwapTurn())
        {
            submenuButton = AddSubmenuButton("Swap", "Swap turns with your partner");
            submenuButton.ClickEvent.AddListener(m_battleManager.SwapTurns);
        }

        if (CanSwitchPhantom())
        {
            submenuButton = AddSubmenuButton("Switch Phantom", "Switch out your current phantom partner");
            submenuButton.ClickEvent.AddListener(SwitchPhantomsMenu);
        }

        // Just to skip a turn
        submenuButton = AddSubmenuButton("Skip", "Skip your turn");
        Debug.Log("Adding listener to the button!");
        submenuButton.ClickEvent.AddListener(delegate { m_actionInput.SkipCallback(); });

        // TODO: Run button
        submenuButton = AddSubmenuButton("Run", "Run away from battle");

        ShowSubmenu();
    }

    // Shows all the abilities available for the current actor
    public void AbilitiesMenu()
    {
        SetState(BattleMenuState.ABILITIES);

        ClearSubmenu();

        BattleSubmenuButton submenuButton = null;
        foreach (Ability ability in m_actionInput.ValidAbilities)
        {
            submenuButton = AddSubmenuButton(ability.Data.DisplayName, ability.Data.Description, ability.Data.ManaCost.ToString() + " Mana");
            submenuButton.ClickEvent.AddListener(delegate { TargetMenuAbility(ability); });
        }

        List<Ability> validAbilities = new List<Ability>(m_actionInput.ValidAbilities);
        foreach(Ability ability in m_currentActor.Abilities)
        {
            if (m_battleManager.CurrentBattle.IsAbilityValidWithoutMana(m_currentActor, ability))
            {
                submenuButton = AddSubmenuButton(ability.Data.DisplayName, ability.Data.Description, ability.Data.ManaCost.ToString() + " Mana");
                submenuButton.ButtonComponent.interactable = false;
            }
        }

        ShowSubmenu();
    }

    // Shows all the items available for the current actor
    public void ItemsMenu()
    {
        SetState(BattleMenuState.ITEMS);

        ClearSubmenu();

        BattleSubmenuButton submenuButton = null;
        foreach (Item item in m_actionInput.ValidItems)
        {
            submenuButton = AddSubmenuButton(item.Data.DisplayName, item.Data.Description, "x" + item.RemainingUses.ToString());
            submenuButton.ClickEvent.AddListener(delegate { TargetMenuItem(item); });
        }

        ShowSubmenu();
    }

    public void SwitchPhantomsMenu()
    {
        SetState(BattleMenuState.TARGETING);
        ClearSubmenu();

        BattleSubmenuButton submenuButton = null;
        foreach(Actor actor in m_currentActor.Group.Actors)
        {
            if (!m_battleManager.CurrentBattle.ActorInfo[actor].IsParticipating && actor.HP > 0)
            {
                submenuButton = AddSubmenuButton(actor.DisplayName, "Switch to " + actor.DisplayName, actor.HP.ToString() + "/" + actor.MaxHP.ToString() + " HP");
                submenuButton.ClickEvent.AddListener(delegate { m_battleManager.SwapPhantoms(actor); });
            }
        }

        ShowSubmenu();
    }

    // Sets up the targeting menu for an ability.
    public void TargetMenuAbility(Ability ability)
    {
        m_currentAbility = ability;
        TargetMenuGeneric(ActionType.ABILITY);
    }

    // Sets up the targeting menu for an item.
    public void TargetMenuItem(Item item)
    {
        m_currentItem = item;
        TargetMenuGeneric(ActionType.ITEM);
    }

    // Called when a target is selected for an ability
    public void SetAbilityTarget(Actor actor)
    {
        m_actionTargets.Add(actor);
        switch (m_currentAbility.Data.TargetType)
        {
            case BattleInteractorData.TargetType.SingleActor:
                m_actionInput.AbilitySelectCallback(m_currentAbility);
                m_battleCamera.ResetCamera();
                break;
            case BattleInteractorData.TargetType.NumberOfActors:
                if (m_actionTargets.Count >= m_currentAbility.Data.NumberOfTargets)
                {
                    m_actionInput.AbilitySelectCallback(m_currentAbility);
                    m_battleCamera.ResetCamera();
                }
                break;
        }
    }

    // Called when a target is selected for an item
    public void SetItemTarget(Actor actor)
    {
        m_actionTargets.Add(actor);
        switch (m_currentItem.Data.TargetType)
        {
            case BattleInteractorData.TargetType.SingleActor:
                m_actionInput.ItemSelectCallback(m_currentItem);
                m_battleCamera.ResetCamera();
                break;
            case BattleInteractorData.TargetType.NumberOfActors:
                if (m_actionTargets.Count >= m_currentAbility.Data.NumberOfTargets)
                {
                    m_actionInput.ItemSelectCallback(m_currentItem);
                    m_battleCamera.ResetCamera();
                }
                break;
        }
    }

    // Used to navigate menues
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

    // Returns to the main menu for the player.
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

    // Updates the battle menu state - should have any logic required for entering a new state here
    private void SetState(BattleMenuState newState)
    {
        m_prevState = m_currentState;
        m_currentState = newState;
    }

    // Either grabs a pooled button or makes a new one, depening on needs - and sets up the button.
    private BattleSubmenuButton AddSubmenuButton(string name, string description, string info = "")
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
        submenuButton.ButtonInfo = info;
        submenuButton.ButtonComponent.interactable = true;

        // Remove all previous listeners, and add any onclick listeners that all buttons would have.
        submenuButton.ClickEvent = new UnityEvent();
        submenuButton.SelectEvent = new UnityEvent();

        submenuButton.ClickEvent.AddListener(ClearSubmenu);
        submenuButton.ClickEvent.AddListener(HideSubmenu);
        submenuButton.SelectEvent.AddListener(delegate { SetDescription(description); });
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
        foreach (BattleSubmenuButton item in m_subMenuButtons)
        {
            item.ClickEvent = new UnityEvent();
            item.SelectEvent = new UnityEvent();
        }
    }

    // Some delegates used only in this function
    delegate void SelectCallback();
    private void TargetMenuGeneric(ActionType actionType)
    {
        SetState(BattleMenuState.TARGETING);

        m_actionTargets.Clear();
        m_actionGroupTargets.Clear();

        ClearSubmenu();

        // Set things up based on the action type
        Actor[] validTargets;
        BattleInteractorData.TargetType targetType;
        BattleInteractorData.TargetGroupGroups targetGroup;
        SelectCallback callback;

        if (actionType == ActionType.ABILITY)
        {
            validTargets = m_battleManager.CurrentBattle.GetValidTargets(m_currentActor, m_currentAbility);
            targetType = m_currentAbility.Data.TargetType;
            targetGroup = m_currentAbility.Data.ValidTargetGroups;
            callback = (delegate { m_actionInput.AbilitySelectCallback(m_currentAbility); });
        }
        else
        {
            validTargets = m_battleManager.CurrentBattle.GetValidTargets(m_currentActor, m_currentItem);
            targetType = m_currentItem.Data.TargetType;
            targetGroup = m_currentItem.Data.ValidTargetGroups;
            callback = (delegate { m_actionInput.ItemSelectCallback(m_currentItem); });
        }

        BattleSubmenuButton submenuButton = null;

        switch (targetType)
        {
            case BattleInteractorData.TargetType.SingleActor:
                // Add a button for each actor, for now.
                foreach (Actor actor in validTargets)
                {
                    submenuButton = AddSubmenuButton(actor.DisplayName, null);
                    if (actionType == ActionType.ABILITY)
                    {
                        submenuButton.ClickEvent.AddListener(delegate { SetAbilityTarget(actor); });
                    }
                    else
                    {
                        submenuButton.ClickEvent.AddListener(delegate { SetItemTarget(actor); });

                    }
                    submenuButton.SelectEvent.AddListener(delegate { SetArrowIndicator(actor); });
                }
                ShowSubmenu();
                break;
            case BattleInteractorData.TargetType.NumberOfActors:
                // Add a button for each actor, with the addition of removing the button when used.
                foreach (Actor actor in validTargets)
                {
                    submenuButton = AddSubmenuButton(actor.DisplayName, null);
                    if (actionType == ActionType.ABILITY)
                    {
                        submenuButton.ClickEvent.AddListener(delegate { SetAbilityTarget(actor); });
                    }
                    else
                    {
                        submenuButton.ClickEvent.AddListener(delegate { SetItemTarget(actor); });
                    }
                    submenuButton.ClickEvent.AddListener(delegate
                    {
                        submenuButton.gameObject.SetActive(false);
                    });
                    submenuButton.SelectEvent.AddListener(delegate { SetArrowIndicator(actor); });
                }
                ShowSubmenu();
                break;
            case BattleInteractorData.TargetType.AllActorsInGroup:
                // TODO
                Debug.Log("Setting targets");
                switch(targetGroup)
                {
                    case BattleInteractorData.TargetGroupGroups.Allies:
                        m_actionGroupTargets.Add(m_currentActor.Group);
                        break;
                    case BattleInteractorData.TargetGroupGroups.Opponents:
                        m_actionGroupTargets.Add(validTargets[0].Group);
                        break;
                    case BattleInteractorData.TargetGroupGroups.All:
                        m_actionGroupTargets.AddRange(m_battleManager.CurrentBattle.Groups);
                        break;
                }
                callback();
                m_battleCamera.ResetCamera();
                break;
            case BattleInteractorData.TargetType.AllActors:
                // Just add all valid targets then do the ability select callback.
                m_actionTargets.AddRange(validTargets);
                callback();
                m_battleCamera.ResetCamera();
                break;
        }
    }

    // Private functions to check if certain options should be available

    // Checks if the player can swap turns between them and their phantom
    private bool CanSwapTurn()
    {
        BattleGroup group = m_currentActor.Group;
        foreach (Actor actor in group.Actors)
        {
            if (actor != m_currentActor && m_battleManager.CurrentBattle.HasRemainingTurns(actor))
            {
                return true;
            }
        }
        return false;
    }

    // Checks if the player can catch in the current fight
    private bool CanCatch()
    {
        return m_battleManager.CanCatch();
    }

    // Checks if the player can swap out their current phantom for another one
    private bool CanSwitchPhantom()
    {
        // Really, just check if there is at least 2 phantoms that the player owns...
        return (PlayerInventoryManager.Instance.Phantoms.Count > 1);
    }
}
