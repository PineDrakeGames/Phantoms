using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Ares;
using Ares.ActorComponents;

public class BattleInitializer : MonoBehaviour
{
    [Header("Music? Idk why not here")]
    [SerializeField]
    private string m_battleMusicIntro = null;
    [SerializeField]
    private string m_battleMusicLoop = null;

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

    [Header("Camera Positions")]
    [SerializeField]
    private Transform m_battleStartCamera = null;
    [SerializeField]
    private Transform m_enemyViewCamera = null;

    List<Actor> PlayerActors = null;
    List<Actor> EnemyActors = null;

    public static UnityEvent BattleInitializationFinished = new UnityEvent();


    // Start is called before the first frame update
    public void InitializeBattle(List<CombatantInstanceData> playerCombatants, List<CombatantInstanceData> inactivePlayerCombatants, EnemyEncounterData enemies)
    {
        AudioManager.PlayMusic(m_battleMusicLoop, m_battleMusicIntro);

        PlayerActors = SpawnActorsInLine(true, playerCombatants, m_playerSpawnPoint1.position, m_playerSpawnPoint2.position, m_maxDistanceBetweenPlayers);

        List<Actor> InactivePlayerActors = SpawnActors(true, inactivePlayerCombatants, PlayerActors[PlayerActors.Count - 1].transform.position);
        foreach (Actor actor in InactivePlayerActors)
        {
            actor.gameObject.SetActive(false);
        }
        PlayerActors.AddRange(InactivePlayerActors);

        Dictionary<CombatantInstanceData, Actor> Enemies = enemies.GetEnemies();
        EnemyActors = new List<Actor>();


        Vector3[] positions = GetSpawnPointsInLine(Enemies.Count, m_enemySpawnPoint1.position, m_enemySpawnPoint2.position, m_maxDistanceBetweenEnemies);
        int index = 0;
        foreach (CombatantInstanceData combatant in Enemies.Keys)
        {
            Actor actorComponent = Enemies[combatant];
            m_battleManager.ActorToData[actorComponent] = combatant;
            actorComponent.transform.position = positions[index];
            EnemyActors.Add(actorComponent);
            index++;
        }

        SetInitialRotations(PlayerActors, EnemyActors);

        StartCoroutine(BattleStartSequence());
    }

    /// Used to spawn actors in a straight line, with a given start and end point to the line, and a max distance between the actors. ///
    private List<Actor> SpawnActorsInLine(bool isPlayer, List<CombatantInstanceData> combatants, Vector3 point1, Vector3 point2, float maxDistance)
    {
        // First, create a list of positions to spawn things
        int numToSpawn = combatants.Count;
        Vector3[] spawnPoints = GetSpawnPointsInLine(numToSpawn, point1, point2, maxDistance);

        // Now, spawn each actor using our list of positions.
        return SpawnActors(isPlayer, combatants, spawnPoints);
    }

    private Vector3[] GetSpawnPointsInLine(int numToSpawn, Vector3 point1, Vector3 point2, float maxDistance)
    {
        Vector3[] spawnPoints = new Vector3[numToSpawn];
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

        return spawnPoints;
    }

    // To just spawn a bunch of combatants in a single place.
    private List<Actor> SpawnActors(bool isPlayer, List<CombatantInstanceData> combatants, Vector3 spawnPoint)
    {
        Vector3[] spawnPoints = new Vector3[combatants.Count];
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnPoints[i] = spawnPoint;
        }

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

                actorComponent.Init(displayName, data.CurrentHP, data.CurrentStats.MaxHP, data.CurrentMana, data.CurrentStats.Mana, stats, data.Data.Abilities, actorComponent.FallbackAbility, actorComponent.Afflictions, actorComponent.inventory);
                actorComponent.MainType = PhantomType.NONE;
                actorComponent.SecondType = PhantomType.NONE;
                ActorAnimation actorAnimation = spawnedObject.GetComponent<ActorAnimation>();
                if (actorAnimation == null)
                {
                    actorAnimation = spawnedObject.AddComponent<ActorAnimation>();
                    SetActorAnimations(actorAnimation);
                }
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
                actorComponent.Init("TODO REPLACE THIS", data.CurrentHP, data.CurrentStats.MaxHP, data.CurrentMana, data.CurrentStats.Mana, stats, data.Data.Abilities, actorComponent.FallbackAbility, actorComponent.Afflictions, actorComponent.inventory);
                actorComponent.MainType = PhantomType.NONE;
                actorComponent.SecondType = PhantomType.NONE;
            }

            spawnedActors.Add(actorComponent);

            BattleManager.Instance.ActorToData[actorComponent] = combatants[i];
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

        foreach (ActorAnimationAbilityElement abilityElement in actorAnimation.AbilityCallbacks)
        {
            abilityElement.Enabled = true;
        }
    }

    private IEnumerator BattleStartSequence()
    {
        BattleCameraManager.Instance.SetCamera(m_battleStartCamera.position, m_battleStartCamera.rotation, 0f);
        BattleCameraManager.Instance.SetCamera(m_enemyViewCamera.position, m_enemyViewCamera.rotation, 1f);
        yield return new WaitForSeconds(0.75f); // Wait for cam movement
        // Give 0.3 seconds to stagger enemy spawn in animations - spreading them out over that time.
        float timePerPhantom = 0.5f;
        if (EnemyActors.Count > 1)
        {
            timePerPhantom = timePerPhantom / (EnemyActors.Count - 1);
        }

        Dictionary<string, int> EncounterNames = new Dictionary<string, int>();
        List<string> keys = new List<string>();

        foreach (Actor enemy in EnemyActors)
        {
            foreach (Animator anim in enemy.GetComponentsInChildren<Animator>())
            {
                anim.SetTrigger("Spawn");
            }
            string name = BattleManager.Instance.ActorToData[enemy].GetDisplayName();
            if (EncounterNames.ContainsKey(name))
            {
                EncounterNames[name] += 1;
            }
            else
            {
                EncounterNames.Add(name, 1);
                keys.Add(name);
            }
            yield return new WaitForSeconds(timePerPhantom);
        }

        // Build string.
        string battleStartText = "You encountered";
        for (int i = 0; i < keys.Count; i++)
        {
            battleStartText += " ";
            string name = keys[i];
            int num = EncounterNames[name];
            if (keys.Count > 1 && i == (keys.Count - 1))
            {
                battleStartText += "and ";
            }
            if (num > 1)
            {
                battleStartText += num.ToString();
            }
            else
            {
                battleStartText += "a";
            }
            battleStartText += " wild " + name;

            if (keys.Count > 2 && i < (keys.Count - 1))
            {
                battleStartText += ",";
            }
        }
        battleStartText += "!";

        BattleText.SetText(battleStartText, false);

        foreach (Actor enemy in EnemyActors)
        {
            foreach (Animator anim in enemy.GetComponentsInChildren<Animator>())
            {
                anim.SetTrigger("Special");
            }
            yield return new WaitForSeconds(timePerPhantom);
        }

        yield return new WaitForSeconds(1.3f); // Give a bit of time for the spawn animations to play

        Vector3 enemyCenter = Vector3.zero;
        int totalEnemies = 0;
        foreach (Actor enemy in EnemyActors)
        {
            enemyCenter += enemy.transform.position;
            totalEnemies += 1;
        }
        enemyCenter /= totalEnemies;
        BattleCameraManager.Instance.SetCameraOverShoulder(PlayerActors[0].transform.position, enemyCenter, 1f);
        yield return new WaitForSeconds(.8f);
        BattleText.HideText();

        BattleInitializationFinished.Invoke();
        m_battleManager.StartBattle(PlayerActors.ToArray(), EnemyActors.ToArray());
    }

}
