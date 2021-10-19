using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Ares;
using TMPro;

public class BattlePlayerMenu : MonoBehaviour
{
    /////////////////////////
    /// Serialized Fields ///
    /////////////////////////

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

    [Header("Partner menu items")]
    [SerializeField]
    private GameObject m_partnerMenuParent = null;

    // Reference to each of the main menu buttons, both for disabling them when the option is not available, and for menu navigation.
    [SerializeField]
    private Button m_partnerAbilitiesButton = null;
    [SerializeField]
    private Button m_partnerItemsButton = null;
    [SerializeField]
    private Button m_partnerTacticsButton = null;

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
    private BattleSubmenuSelection selectionObject = null;

    [Header("Confirm Menu")]
    [SerializeField]
    private GameObject m_confirmMenuParent = null;

    [Header("Phantom Catching Menu")]
    [SerializeField]
    private BattlePhantomCatchManager m_phantomCatcher = null;

    [Header("UI Items")]
    [SerializeField]
    private Transform m_playerHealthIndicators = null;
    [SerializeField]
    private Transform m_enemyHealthIndicators = null;
    [SerializeField]
    private GameObject m_healthIndicatorPrefab = null;
    [SerializeField]
    private GameObject m_enemyHealthIndicatorPrefab = null;

    [Header("3D Indicators")]
    [SerializeField]
    private GameObject m_currentTurnIndicator = null;
    [SerializeField]
    private GameObject m_targetIndicatorPrefab = null;


    ////////////////////////////////////
    /// Public Bools to Lock Options ///
    ////////////////////////////////////
    public bool CanRun = true;
    public bool CanCatch = true;
    public bool CanSwap = true;
    public bool CanSwitch = true;
    public bool CanSkip = true;
    public bool CanUseAbilities = true;
    public bool CanUseItems = true;
    public bool CanUseTactics = true;

    /////////////////////
    /// Private Enums ///
    /////////////////////

    private enum BattleMenuState
    {
        INACTIVE,
        MAIN,
        TACTICS,
        ABILITIES,
        ITEMS,
        TARGETING,
        CATCHING
    }
    private enum ActionType
    {
        ABILITY,
        ITEM
    }

    /////////////////////////
    /// Private Variables ///
    /////////////////////////

    private GameObject m_currentTargetIndicator = null;
    private List<GameObject> m_targetIndicators = new List<GameObject>();

    private List<BattleSubmenuButton> m_subMenuButtons = new List<BattleSubmenuButton>();
    private ActionInput m_actionInput;
    private Actor m_currentActor = null;
    private bool m_isKeeperTurn = true;

    private Ability m_currentAbility = null;
    private Item m_currentItem = null;
    private List<Actor> m_actionTargets = new List<Actor>();

    private List<BattleGroup> m_actionGroupTargets = new List<BattleGroup>();

    private Vector3 enemyCenter = Vector3.zero;

    private BattleMenuState m_prevState = BattleMenuState.INACTIVE;
    private BattleMenuState m_currentState = BattleMenuState.MAIN;

    private UnityEvent OnConfirm = new UnityEvent();

    //////////////////////
    /// Public Getters ///
    //////////////////////

    public Dictionary<Actor, HealthIndicator> ActorToHealthIndicator = new Dictionary<Actor, HealthIndicator>();
    public List<Actor> ActionTargets { get { return m_actionTargets; } }
    public List<BattleGroup> ActionGroupTargets { get { return m_actionGroupTargets; } }

    /////////////////////////////
    /// Static Instance Stuff ///
    /////////////////////////////
    private static BattlePlayerMenu s_instance = null;
    public static BattlePlayerMenu Instance { get { return s_instance; } }


    ///////////////////////
    /// Unity Functions ///
    ///////////////////////

    private void Awake()
    {
        // Just making sure there are no locks by default when the battle starts, let other scripts lock the battle.
        CanRun = true;
        CanCatch = true;
        CanSwap = true;
        CanSwitch = true;
        CanSkip = true;
        CanUseAbilities = true;
        CanUseItems = true;
        CanUseTactics = true;
    }

    private void OnDestroy()
    {
        if (m_battleManager && m_battleManager.CurrentBattle != null)
        {
            m_battleManager.CurrentBattle.OnTurnStart.RemoveListener(SetCurrentTurnIndicator);
        }
    }

    /////////////////////////////////////
    // Battle Start function!          //
    // Call this to get things set up. //
    /////////////////////////////////////
    public void OnBattleStart()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }

        m_mainMenuParent.SetActive(false);
        m_partnerMenuParent.SetActive(false);
        HideTargetIndicators();
        HideSubmenu();

        m_battleManager.CurrentBattle.OnTurnStart.AddListener(SetCurrentTurnIndicator);
        m_battleManager.CurrentBattle.OnTurnEnd.AddListener(OnTurnEnd);

        foreach (Actor actor in m_battleManager.CurrentBattle.Actors)
        {
            if (m_battleManager.CurrentBattle.ActorInfo[actor].IsParticipating)
            {
                GameObject gameObj = null;
                HealthIndicator indicator = null;
                if (actor.Group.Name == "Player")
                {
                    gameObj = Instantiate(m_healthIndicatorPrefab, m_playerHealthIndicators);
                }
                else
                {
                    gameObj = Instantiate(m_enemyHealthIndicatorPrefab, m_enemyHealthIndicators);
                }
                indicator = gameObj.GetComponent<HealthIndicator>();
                indicator.Actor = actor;
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

    /////////////////////////////////
    /// Public UI event Listeners ///
    /////////////////////////////////

    // Updated the current turn indicator to show who's turn it is right now
    public void SetCurrentTurnIndicator(Actor actor)
    {
        if (m_currentTurnIndicator && actor)
        {
            m_currentTurnIndicator.transform.position = actor.gameObject.transform.position;
        }

        foreach (KeyValuePair<Actor, HealthIndicator> kvp in ActorToHealthIndicator)
        {
            kvp.Value.SetAsActive(kvp.Key == actor || actor.Group != kvp.Key.Group);
        }
    }

    // Something just to make sure things are cleaned up at the end of a turn.
    public void OnTurnEnd(Actor actor)
    {
        HideTargetIndicators();
        HideSubmenu();
        m_mainMenuParent.SetActive(false);
        m_partnerMenuParent.SetActive(false);
        
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Public functions to be called by other scripts to update/set data in the battle player menu. ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    // Called when an actor's turn starts - sets up their menu!
    public void StartTurn(Actor actor, ActionInput actionInput)
    {
        m_actionInput = actionInput;
        m_currentActor = actor;

        m_isKeeperTurn = (m_battleManager.ActorToData[m_currentActor] is PlayerBattleInstanceData);

        m_battleCamera.SetCameraOverShoulder(actor.transform.position, enemyCenter);
        ReturnToMainMenu();

        SetCurrentTurnIndicator(m_currentActor);
    }

    public void SetArrowIndicator(Actor actor = null)
    {
        if (m_currentTargetIndicator == null)
        {
            m_currentTargetIndicator = GetTargetIndicator();
        }

        if (actor == null)
        {
            m_currentTargetIndicator.SetActive(false);
        }
        else
        {
            m_currentTargetIndicator.SetActive(true);
            // TODO: Either set offset in prefab or in data
            m_currentTargetIndicator.transform.position = actor.transform.position + (Vector3.up * 1.5f);
        }
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
        if (m_isKeeperTurn)
        {
            submenuButton = AddSubmenuButton("Catch", "Catch that phantom!");
            submenuButton.ClickEvent.AddListener(CatchPhantomMenu);
            submenuButton.ButtonComponent.interactable = CanCatchPhantom();
        }

        // Check if swapping turns is an option first.
        if (CanSwapTurn())
        {
            submenuButton = AddSubmenuButton("Swap", "Swap turns with your partner");
            submenuButton.ClickEvent.AddListener(m_battleManager.SwapTurns);
            submenuButton.ButtonComponent.interactable = CanSwap;
        }

        if (CanSwitchPhantom())
        {
            submenuButton = AddSubmenuButton("Switch Phantom", "Switch out your current phantom partner");
            submenuButton.ClickEvent.AddListener(SwitchPhantomsMenu);
            submenuButton.ButtonComponent.interactable = CanSwitch;
        }

        // Just to skip a turn
        submenuButton = AddSubmenuButton("Skip", "Skip your turn");
        submenuButton.ClickEvent.AddListener(delegate { m_actionInput.SkipCallback(); });
        submenuButton.ButtonComponent.interactable = CanSkip;

        // Run Button
        // TODO: Have only chance to run, just always runs for now
        submenuButton = AddSubmenuButton("Run", "Run away from battle");
        submenuButton.ClickEvent.AddListener(m_battleManager.TryRun);
        submenuButton.ButtonComponent.interactable = CanRun;

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
        foreach (Ability ability in m_currentActor.Abilities)
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
        foreach (Actor actor in m_currentActor.Group.Actors)
        {
            if (!m_battleManager.CurrentBattle.ActorInfo[actor].IsParticipating && actor.HP > 0)
            {
                submenuButton = AddSubmenuButton(actor.DisplayName, "Switch to " + actor.DisplayName, actor.HP.ToString() + "/" + actor.MaxHP.ToString() + " HP");
                submenuButton.ClickEvent.AddListener(delegate
                {
                    m_battleManager.SwapPhantoms(actor);
                    m_actionInput.SkipCallback();
                });
            }
        }

        ShowSubmenu();
    }

    public void CatchPhantomMenu()
    {
        SetState(BattleMenuState.CATCHING);
        ClearSubmenu();
        m_phantomCatcher.StartCatch(m_currentActor, m_actionInput);
    }

    // Sets up the targeting menu for an ability.
    public void TargetMenuAbility(Ability ability)
    {
        m_currentAbility = ability;

        if (m_currentAbility.UseMinigame)
        {
            AbilityMinigameManager.ShowMinigameDescription(ability.Minigame);
        }

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
                else
                {
                    // Leave the current target indicator, get a new one.
                    m_currentTargetIndicator = GetTargetIndicator();
                    m_currentTargetIndicator.SetActive(false);
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
                else
                {
                    // Leave the current target indicator, get a new one.
                    m_currentTargetIndicator = GetTargetIndicator();
                    m_currentTargetIndicator.SetActive(false);
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
                HideTargetIndicators();

                switch (m_prevState)
                {
                    case BattleMenuState.ABILITIES:
                        AbilityMinigameManager.HideMinigameDescription();
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
            case BattleMenuState.CATCHING:
                m_phantomCatcher.StopCatching();
                TacticsMenu();
                return;
        }
    }

    // Returns to the main menu for the player.
    public void ReturnToMainMenu()
    {
        SetState(BattleMenuState.MAIN);

        ClearSubmenu();
        HideSubmenu();

        m_tacticsButton.interactable = CanUseTactics;
        m_partnerTacticsButton.interactable = CanUseTactics;
        if (!CanUseTactics && m_prevState == BattleMenuState.TACTICS)
        {
            m_prevState = BattleMenuState.ABILITIES;
        }
        m_itemsButton.interactable = CanUseItems;
        m_partnerItemsButton.interactable = CanUseItems;
        if (!CanUseItems && m_prevState == BattleMenuState.ITEMS)
        {
            m_prevState = BattleMenuState.ABILITIES;
        }
        m_abilitiesButton.interactable = CanUseAbilities;
        m_partnerAbilitiesButton.interactable = CanUseAbilities;
        if (!CanUseAbilities && (m_prevState != BattleMenuState.TACTICS || m_prevState != BattleMenuState.ITEMS))
        {
            m_prevState = BattleMenuState.TACTICS;
        }

        if (m_isKeeperTurn)
        {
            m_mainMenuParent.SetActive(true);
            m_partnerMenuParent.SetActive(false);
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
        else
        {
            m_partnerMenuParent.SetActive(true);
            m_mainMenuParent.SetActive(false);
            switch (m_prevState)
            {
                case BattleMenuState.TACTICS:
                    m_partnerTacticsButton.Select();
                    break;
                case BattleMenuState.ITEMS:
                    m_partnerItemsButton.Select();
                    break;
                default:
                    // Both if we were previously in the abilities menu, or from any other menu.
                    m_partnerAbilitiesButton.Select();
                    break;
            }
        }
    }

    public void ConfirmMove()
    {
        OnConfirm.Invoke();
    }


    ////////////////////////////////////////
    /// Private functions for Indicators ///
    ////////////////////////////////////////

    private GameObject GetTargetIndicator()
    {
        foreach (GameObject indicator in m_targetIndicators)
        {
            if (!indicator.activeSelf)
            {
                indicator.SetActive(true);
                return indicator;
            }
        }
        GameObject newIndicatorObject = Instantiate(m_targetIndicatorPrefab);
        m_targetIndicators.Add(newIndicatorObject);
        return newIndicatorObject;
    }

    private void HideTargetIndicators()
    {
        foreach (GameObject indicator in m_targetIndicators)
        {
            indicator.SetActive(false);
        }
        m_currentTargetIndicator = null;
    }

    ///////////////////////////////////////////////////////////
    /// Private helper functions to manage the battle menu. ///
    ///////////////////////////////////////////////////////////

    /// Functions to handle the states ///

    // Updates the battle menu state - should have any logic required for entering a new state here
    private void SetState(BattleMenuState newState)
    {
        m_prevState = m_currentState;
        m_currentState = newState;
    }

    /// Submenu-Related stuff ///

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
        if (!string.IsNullOrEmpty(description))
        {
            submenuButton.SelectEvent.AddListener(delegate { selectionObject.SetButtonSelection(submenuButton); });
        }
        return submenuButton;
    }

    private void ShowSubmenu()
    {
        m_mainMenuParent.SetActive(false);
        m_partnerMenuParent.SetActive(false);
        m_subMenuParent.SetActive(true);

        foreach (BattleSubmenuButton submenuButton in m_subMenuButtons)
        {
            if (submenuButton.gameObject.activeSelf)
            {
                Canvas.ForceUpdateCanvases();
                submenuButton.ButtonComponent.Select();
                return;
            }
        }

        m_backButton.Select();
    }

    private void HideSubmenu()
    {
        m_subMenuParent.SetActive(false);
        HideConfirmMenu();
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
        HideConfirmMenu();
    }

    /// Confirm Menu Functions ///
    private void ShowConfirmMenu()
    {
        m_confirmMenuParent.SetActive(true);
    }

    private void HideConfirmMenu()
    {
        m_confirmMenuParent.SetActive(false);
        OnConfirm.RemoveAllListeners();
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
                switch (targetGroup)
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

                foreach (BattleGroup group in m_actionGroupTargets)
                {
                    foreach (Actor actor in group.Actors)
                    {
                        GameObject targetIndicator = GetTargetIndicator();
                        targetIndicator.transform.position = actor.transform.position + (Vector3.up * 1.5f);
                    }
                }

                OnConfirm.AddListener(delegate
               {
                   callback();
                   m_battleCamera.ResetCamera();
                   HideConfirmMenu();
               });
                ShowConfirmMenu();
                break;
            case BattleInteractorData.TargetType.AllOtherActors:
            case BattleInteractorData.TargetType.AllActors:
                // Just add all valid targets then do the ability select callback.
                m_actionTargets.AddRange(validTargets);

                foreach (Actor actor in m_actionTargets)
                {
                    GameObject targetIndicator = GetTargetIndicator();
                    targetIndicator.transform.position = actor.transform.position + (Vector3.up * 1.5f);
                }

                OnConfirm.AddListener(delegate
               {
                   callback();
                   m_battleCamera.ResetCamera();
                   HideConfirmMenu();
               });
                ShowConfirmMenu();
                break;
        }
    }

    /////////////////////////////////////////////////////////////////////////
    /// Private functions to check if certain options should be available ///
    /////////////////////////////////////////////////////////////////////////

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
    private bool CanCatchPhantom()
    {
        if (!m_isKeeperTurn && m_currentActor.HP <= 1) { return false; }

        return m_battleManager.CanCatch() && CanCatch;
    }

    // Checks if the player can swap out their current phantom for another one
    private bool CanSwitchPhantom()
    {
        // Really, just check if there is at least 2 phantoms that the player owns...
        return (PlayerInventoryManager.Instance.Phantoms.Count > 1);
    }
}
