/* A basic template for a manager script to set up and manage
 * a battle. It is entirely possible to split this script
 * up into multiple ones, seperating battle and actor creation, UI, etc.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
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

    [Header("Results stuff")]
    [SerializeField]
    private BattleResultsManager m_resultsManager = null;
    public BattleResultsManager ResultsManager { get { return m_resultsManager; } }

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
            }
            return s_instance;
        }
    }

    // Other private variables
    BattleGroup playerGroup = null;
    BattleGroup enemyGroup = null;

    public Dictionary<Actor, CombatantInstanceData> ActorToData = new Dictionary<Actor, CombatantInstanceData>();
    [HideInInspector]
    public UnityEvent OnBattleStart = new UnityEvent();

    public Dictionary<UserBattleInstanceData, int> ExperienceReward = new Dictionary<UserBattleInstanceData, int>();

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

        // For exp stuff
        enemyGroup.ActorDefeated.AddListener(OnEnemyDefeat);

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
                Debug.Log(actor.gameObject.name);
                if (ActorToData[actor] is PlayerBattleInstanceData)
                {
                    actor.OnHPDeplete.AddListener(() => EndBattle(false));
                }
            }

            if (ActorToData.ContainsKey(actor) && ActorToData[actor] is UserBattleInstanceData)
            {
                ExperienceReward[ActorToData[actor] as UserBattleInstanceData] = 0;
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

        BattleLog.Instance.AddBattleListeners(battle);

        OnBattleStart.Invoke();

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

        m_playerMenu.StartTurn(actor, actionInput);
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

    public void OnEnemyDefeat(Actor enemy)
    {
        CombatantInstanceData enemyData = null;
        if (ActorToData.TryGetValue(enemy, out enemyData))
        {
            // TODO: Handle other cases - just phantoms for now
            if (enemyData is PhantomInstanceData)
            {
                int enemyLevel = (enemyData as PhantomInstanceData).Level;
                foreach (Actor actor in playerTeam)
                {
                    if (ActorToData.ContainsKey(actor) && ActorToData[actor] is UserBattleInstanceData)
                    {
                        UserBattleInstanceData playerData = ActorToData[actor] as UserBattleInstanceData;
                        int levelDifference = enemyLevel - playerData.Level;

                        // Magic number time - should eventually store these as constants elsewhere.
                        // 0.55 is to make sure things tend to round up instead of down
                        int xpToGain = 5 + Mathf.RoundToInt(levelDifference * 0.55f);
                        if (!battle.IsParticipating(actor))
                        {
                            xpToGain = Mathf.RoundToInt((float)xpToGain * 0.55f);
                        }
                        xpToGain = Mathf.Clamp(xpToGain, 0, 99);
                        if (ExperienceReward.ContainsKey(playerData))
                        {
                            ExperienceReward[playerData] += xpToGain;
                        }
                        else
                        {
                            ExperienceReward[playerData] = xpToGain;
                        }

                        Debug.Log(playerData.GetDisplayName() + " got " + xpToGain + " xp.");
                    }
                }
            }
        }
    }

    void EndBattle(bool playerWon)
    {
        // A win condition has been met; end the battle.
        if (playerWon)
        {
            battle.EndBattle(Battle.EndReason.PlayerWin);
        }
        else
        {
            battle.EndBattle(Battle.EndReason.EnemyWin);
        }

        // Show victory/ defeat animations and UI
    }

    void OnBattleEnd(Battle.EndReason endReason)
    {
        // UPDATE ALL THE DATA BASED ON THE RESULTS OF THE BATTLE
        foreach (Actor actor in playerTeam)
        {
            UserBattleInstanceData data = ActorToData[actor] as UserBattleInstanceData;
            data.CurrentHP = actor.HP;
            data.CurrentMana = actor.Mana;

            if (data is PlayerBattleInstanceData && data.CurrentHP <= 0)
            {
                data.CurrentHP = 1;
            }

            ExperienceReward[data] = Mathf.Clamp(ExperienceReward[data], 1, 99);
            //ExperienceReward[data] = Random.Range(5, 500);
            Debug.Log(data.GetDisplayName() + " got " + ExperienceReward[data] + " xp TOTAL.");
        }

        // Update inventory with any used items!
        PlayerInventoryManager.Instance.SaveBattleInventory(playerGroup.Inventory as StackedInventory);

        // Reward XP and determine level ups!
        // TODO: In the future, enemies other than phantoms should reward some amount of exp special to them. For now, just assume that they are a phantom,
        // and reward Exp based on the level difference.

        // Show them results screen

        BattleText.HideText();
        m_resultsManager.ShowResults(endReason);
    }

    ////////////////////////
    /// Public functions ///
    ////////////////////////

    public void TryRun()
    {
        // For now, just always run and end the battle
        CurrentBattle.EndBattle(Battle.EndReason.Ran);
    }

    public void SwapPhantoms(Actor newPhantom)
    {
        foreach (KeyValuePair<Actor, CombatantInstanceData> keyValuePair in ActorToData)
        {
            if (keyValuePair.Value == PlayerInventoryManager.Instance.GetCurrentPhantom())
            {
                CurrentBattle.SwapParticipant(keyValuePair.Key, newPhantom);
                PlayerInventoryManager.Instance.SetCurrentPhantom(ActorToData[newPhantom] as PhantomInstanceData);
                HealthIndicator prevIndicator = m_playerMenu.ActorToHealthIndicator[keyValuePair.Key];
                m_playerMenu.ActorToHealthIndicator.Remove(keyValuePair.Key);
                prevIndicator.Actor = newPhantom;
                m_playerMenu.ActorToHealthIndicator.Add(newPhantom, prevIndicator);

                return;
            }
        }
    }

    public void SwapTurns()
    {
        battle.SwapTurn();
    }

    public bool CanCatch()
    {
        int remainingEnemies = 0;
        foreach (Actor actor in enemyGroup.Actors)
        {
            if (actor.HP > 0)
            {
                remainingEnemies += 1;
            }
        }
        return (remainingEnemies == 1);
    }

    public Actor CatchablePhantom()
    {
        Actor phantomToCatch = null;
        foreach (Actor actor in enemyGroup.Actors)
        {
            if (actor.HP > 0)
            {
                phantomToCatch = actor;
                break;
            }
        }

        return phantomToCatch;
    }
}