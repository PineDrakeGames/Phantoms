/* A basic template for a manager script to set up and manage
 * a battle. It is entirely possible to split this script
 * up into multiple ones, seperating battle and actor creation, UI, etc.
 */

using UnityEngine;
using Ares;

public class BattleManager : MonoBehaviour
{
    [SerializeField, Header("Battle")] BattleRules rules = null; // The rules and settings that the battle will adhere to.

    [Header("Actors")]
    [SerializeField]
    private Actor[] playerTeam = null;
    [SerializeField]
    private Actor[] enemies = null;

    [Header("UI Items")]
    [SerializeField]
    private BattlePlayerMenu m_playerMenu = null;

    [Header("Test stuff")]
    [SerializeField]
    [Scene]
    private string m_testReturnScene = null;

    private Battle battle; // A reference to the actual Battle object
    public Battle CurrentBattle
    {
        get { return battle; }
    }

    private static BattleManager s_instance = null;
    public static BattleManager Instance
    {
        get 
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<BattleManager>();
                if (s_instance)
                {
                    GameObject managerObject = Instantiate(new GameObject());
                    s_instance = managerObject.AddComponent<BattleManager>();
                }
            }
            return s_instance;
        }
    }

    // Other private variables
    BattleGroup playerGroup = null;
    BattleGroup enemyGroup = null;

    void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }
    }

    public void StartBattle(Actor[] playerActors, Actor[] enemyActors)
    {
        playerTeam = playerActors;
        enemies = enemyActors;
        StartBattle();
    }

    public void StartBattle()
    {
        // Set up battle
        battle = new Battle(rules);

        // Set up the required battle delegates and events
        battle.OnActorNeedsActionInput.AddListener(ShowActionInput);
        battle.OnActorNeedsSingleTargetInput.AddListener(ShowTargetInput);
        battle.OnActorNeedsActorsTargetInput.AddListener(ShowTargetInput);
        battle.OnActorNeedsGroupTargetInput.AddListener(ShowTargetInput);
        battle.OnActorHasGivenAllNeededInput.AddListener(HideInput);
        battle.OnBattleEnd.AddListener(OnBattleEnd);
        
        // Set up battle Camera manager's events
        battle.OnTurnEnd.AddListener(BattleCameraManager.Instance.OnTurnEnd);

        // Set up groups and win conditions
        playerGroup = battle.AddGroup("Player", PlayerInventoryManager.Instance.CreateBattleInventory());
        enemyGroup = battle.AddGroup("Enemies");

        playerGroup.OnDefeat.AddListener(() => EndBattle(false));
        enemyGroup.OnDefeat.AddListener(() => EndBattle(true));

        // Add all actors to their respective groups
        foreach (Actor actor in playerTeam)
        {
            playerGroup.AddActor(actor, true);
            if (!actor.gameObject.activeSelf)
            {
                battle.SetParticipation(actor, false);
            }
            else
            {
                battle.SetParticipation(actor, true);
            }
        }
        foreach (Actor actor in enemies)
        {
            enemyGroup.AddActor(actor, true);
            if (!actor.gameObject.activeSelf)
            {
                battle.SetParticipation(actor, false);
            }
            else
            {
                battle.SetParticipation(actor, true);
            }
        }


        // Start the battle and get it initialized
        battle.Start(true);

        m_playerMenu.OnBattleStart();

        // If we'd started the battle with `progressAutomatically = false`, we could wait a while here to open menus etc.
        // before manually progressing to the first round by calling `battle.ProgressBattle()`.
    }

    void ShowActionInput(Actor actor, ActionInput actionInput)
    {
        // Set up and show the UI for selecting an actor's item or ability.

        // `actionInput.ValidAbilities` and `.ValidItems` are filtered lists of all abilities and items that can be used
        // given the current state of the battle.

        // `actor.Abilities` can be used to access all abilities.

        // The actor's full inventory can be accessed from either `actor.inventory`, `actor.Group.Inventory`
        // or both, depending on how your game works and which items you wish to show when.
        // These inventories can be filtered based on the `rules.ItemComsumptionMoment`.
        // Typically `OnRoundStart` and `OnTurn` moments would use the `Inventory.Filter.All` filter,
        // and `OnTurnButMarkPendingOnSelect` would use `Inventory.Filter.ExcludePending`.

        // To select an item or ability, call the respective callback method inside `actionInput`.
        // These callbacks will return a `success` bool.

        m_playerMenu.SetupMenu(actor, actionInput);
    }

    void ShowItemInput(Actor actor, StackedItem[] items, ActionInput actionInput)
    {
        // Set up and show the UI for selecting an item for the current actor to use.
        // When a target is selected, call `actionInput.ItemSelectCallback(chosenItem)`
    }

    void ShowTargetInput(Actor actor, TargetInputSingleActor targetInput)
    {
        if (m_playerMenu.ActionTargets.Count > 0)
        {
            targetInput.TargetSelectCallback(m_playerMenu.ActionTargets[0]);
        }
    }

    void ShowTargetInput(Actor actor, TargetInputNumActors targetInput)
    {
        targetInput.TargetSelectCallback(m_playerMenu.ActionTargets.ToArray());
    }

    void ShowTargetInput(Actor actor, TargetInputGroup targetInput)
    {
        // Set up and show the UI for selecting the chosen action's target group.
        // When a target is selected, call `actionInput.TargetSelectCallback(chosenBattleGroup)`.
        // This callback will return a `success` bool.
        
        targetInput.TargetSelectCallback(m_playerMenu.ActionGroupTargets[0]);
    }

    void HideInput(Actor actor)
    {
        // Hide the UI now that the actor has received all needed input.
    }

    void EndBattle(bool playerWon)
    {
        // A win condition has been met; end the battle.
        battle.EndBattle(Battle.EndReason.WinLoseConditionMet);

        // Show victory/ defeat animations and UI
    }

    void OnBattleEnd(Battle.EndReason endReason)
    {
        if (endReason == Battle.EndReason.OutOfTurns)
        {
            // Show tie screen or determine winner
        }

        PlayerInventoryManager.Instance.SaveBattleInventory(playerGroup.Inventory as StackedInventory);

        // For now, just loading back to the test scene
        LoadingManager.LoadSceneByPath(m_testReturnScene);
    }

    /// Public functions ///

    public void SwapTurns()
    {
        battle.SwapTurn();
    }

    
}