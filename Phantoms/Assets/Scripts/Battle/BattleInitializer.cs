using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ares;
using Ares.ActorComponents;

public class BattleInitializer : MonoBehaviour
{
    [Header("References to other scripts")]
    [SerializeField]
    private BattleManager m_battleManager;

    [Header("Data for where and how to spawn the combatants")]
    [SerializeField]
    private Transform m_playerSpawnPoint1;
    [SerializeField]
    private Transform m_playerSpawnPoint2;
    [SerializeField]
    private float m_maxDistanceBetweenPlayers = 3f;

    [SerializeField]
    private Transform m_enemySpawnPoint1;
    [SerializeField]
    private Transform m_enemySpawnPoint2;
    [SerializeField]
    private float m_maxDistanceBetweenEnemies = 3f;


    // Start is called before the first frame update
    public void InitializeBattle(List<CombatantInstanceData> playerCombatants, List<CombatantInstanceData> enemyCombatants)
    {
        List<Actor> PlayerActors = SpawnActorsInLine(true, playerCombatants, m_playerSpawnPoint1.position, m_playerSpawnPoint2.position, m_maxDistanceBetweenPlayers);
        List<Actor> EnemyActors = SpawnActorsInLine(false, enemyCombatants, m_enemySpawnPoint1.position, m_enemySpawnPoint2.position, m_maxDistanceBetweenEnemies);

        SetInitialRotations(PlayerActors, EnemyActors);

        m_battleManager.StartBattle(PlayerActors.ToArray(), EnemyActors.ToArray());
    }

    /// Used to spawn actors in a straight line, with a given start and end point to the line, and a max distance between the actors. ///
    private List<Actor> SpawnActorsInLine(bool isPlayer, List<CombatantInstanceData> combatants, Vector3 point1, Vector3 point2, float maxDistance)
    {
        // First, create a list of positions to spawn things
        int numToSpawn = combatants.Count;
        Vector3[] spawnPoints = new Vector3[numToSpawn];

        // For each combatant, figure out where we are going to spawn things - this method spreads them evenly between point 1 and point 2, with max distance between them.
        float totalDistance = Vector3.Distance(point1, point2);
        float actorDistance = totalDistance / (float)numToSpawn;
        if (actorDistance > maxDistance)
        {
            actorDistance = maxDistance;
        }
        float lerpChange = actorDistance / totalDistance;

        for (int i = 0; i < numToSpawn; i++)
        {
            float unitsAway = ((float)i + 0.5f) - ((float)numToSpawn * 0.5f);

            float lerpAmount = 0.5f + (unitsAway * lerpChange);
            spawnPoints[i] = Vector3.Lerp(point1, point2, lerpAmount);
        }

        // Now, spawn each actor using our list of positions.
        return SpawnActors(isPlayer, combatants, spawnPoints);
    }

    /// Given a list of positions and a list of combatants, spawn them all.
    private List<Actor> SpawnActors(bool isPlayer, List<CombatantInstanceData> combatants, Vector3[] spawnPoints)
    {
        List<Actor> spawnedActors = new List<Actor>();
        int numToSpawn = Mathf.Min(combatants.Count, spawnPoints.Length);

        // spawn each actor using our list of positions.
        for (int i = 0; i < numToSpawn; i++)
        {
            GameObject spawnedObject = Instantiate(combatants[i].Data.BattlePrefab, spawnPoints[i], Quaternion.identity);
            Actor actorComponent = spawnedObject.GetComponent<Actor>();
            if (actorComponent == null)
            {
                if (isPlayer)
                {
                    actorComponent = spawnedObject.AddComponent<PlayerActor>();
                }
                else
                {
                    actorComponent = spawnedObject.AddComponent<AIActor>();
                }
            }

            // Set up the actor component
            if (combatants[i] is PlayerBattleInstanceData)
            {
                // Do player setup
                CombatantInstanceData data = combatants[i];
                Dictionary<string, int> stats = new Dictionary<string, int>();
                stats.Add("attack", data.CurrentStats.Attack);
                stats.Add("defense", data.CurrentStats.Defense);

                string displayName = "Player";

                actorComponent.Init(displayName, data.CurrentStats.MaxHP, data.CurrentStats.MaxHP, stats, data.Data.Abilities, actorComponent.FallbackAbility, actorComponent.Afflictions, actorComponent.inventory);
                actorComponent.MainType = PhantomType.NONE;
                actorComponent.SecondType = PhantomType.NONE;
            }
            else if (combatants[i] is PhantomInstanceData)
            {
                // Do phantom setup
                PhantomInstanceData data = combatants[i] as PhantomInstanceData;
                data.SetCurrentStats();
                PhantomDataUtility.SetPhantomActor(actorComponent, data);
                ActorAnimation actorAnimation = spawnedObject.GetComponent<ActorAnimation>();
                if (actorAnimation == null)
                {
                    actorAnimation = spawnedObject.AddComponent<ActorAnimation>();
                    SetActorAnimations(actorAnimation);
                }
            }
            else
            {
                // Default fighter setup
                CombatantInstanceData data = combatants[i];
                Dictionary<string, int> stats = new Dictionary<string, int>();
                stats.Add("attack", data.CurrentStats.Attack);
                stats.Add("defense", data.CurrentStats.Defense);

                // TODO: ACtor name?
                actorComponent.Init("TODO REPLACE THIS", data.CurrentStats.MaxHP, data.CurrentStats.MaxHP, stats, data.Data.Abilities, actorComponent.FallbackAbility, actorComponent.Afflictions, actorComponent.inventory);
                actorComponent.MainType = PhantomType.NONE;
                actorComponent.SecondType = PhantomType.NONE;
            }

            spawnedActors.Add(actorComponent);
        }

        return spawnedActors;
    }


    private void SetInitialRotations(List<Actor> players, List<Actor> enemies)
    {
        Vector3 playerCenter = Vector3.zero;
        foreach (Actor player in players)
        {
            playerCenter += player.transform.position;
        }
        playerCenter /= (float)(players.Count);
        playerCenter.y = 0;

        Vector3 enemyCenter = Vector3.zero;
        foreach (Actor enemy in enemies)
        {
            enemyCenter += enemy.transform.position;
        }
        enemyCenter /= (float)(enemies.Count);
        enemyCenter.y = 0;

        Quaternion playerLookDirection = Quaternion.LookRotation(enemyCenter - playerCenter, Vector3.up);
        Quaternion enemyLookDirection = Quaternion.LookRotation(playerCenter - enemyCenter, Vector3.up);

        foreach (Actor player in players)
        {
            player.transform.rotation = playerLookDirection;
        }
        foreach (Actor enemy in enemies)
        {
            enemy.transform.rotation = enemyLookDirection;
        }
    }

    private void SetActorAnimations(ActorAnimation actorAnimation)
    {
        actorAnimation.Init(false, false, false, true, ActorAnimation.ParamaterResetType.DefaultValue, 0, ActorAnimation.ParamaterResetType.DefaultValue, 0f, ActorAnimation.ParamaterResetType.DefaultValue, false);
        ActorAnimationEventElement takeDamageCallback = actorAnimation.GetEventCallback(EventCallbackType.TakeDamage);
        takeDamageCallback.Effect.SetAsTrigger("TakeDamage");
        takeDamageCallback.Enabled = true;

        ActorAnimationEventElement deathCallback = actorAnimation.GetEventCallback(EventCallbackType.Die);
        deathCallback.Effect.SetAsTrigger("Dead");
        deathCallback.Enabled = true;
    }
}
