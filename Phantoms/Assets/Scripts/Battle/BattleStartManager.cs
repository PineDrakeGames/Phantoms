using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ares;

public class BattleStartManager : MonoBehaviour
{
    [SerializeField]
    private BattleInitializer m_battleInitializer = null;

    [Header("Editor Only things to help out")]
    [SerializeField]
    private EnemyEncounterData m_enemyEncounterData = null;

    [SerializeField]
    private PhantomInstanceData[] m_playerPhantoms = null;


    // List of enemies to fight in 
    public static EnemyEncounterData Enemies = null;
    
    #if UNITY_EDITOR
    private void Awake()
    {
        if (Enemies == null)
        {
            Enemies = m_enemyEncounterData;
        }
        if (PlayerInventoryManager.Instance.Phantoms.Count == 0)
        {
            foreach(PhantomInstanceData phantom in m_playerPhantoms)
            {
                phantom.SetCurrentStats();
                phantom.FullRestore();
            }
            PlayerInventoryManager.Instance.Phantoms.AddRange(m_playerPhantoms);
        }
    }

    #endif

    void Start()
    {
        List<CombatantInstanceData> playerCombatants = new List<CombatantInstanceData>();
        playerCombatants.Add(DataManager.Instance.GetPlayerBattleInstanceData());

        // For now, just assume that the first in the list is the current phantom.
        if (PlayerInventoryManager.Instance.Phantoms.Count >= 1)
        {
            playerCombatants.Add(PlayerInventoryManager.Instance.GetCurrentPhantom());
        }

        List<CombatantInstanceData> playerInactiveCombatants = new List<CombatantInstanceData>();
        playerInactiveCombatants.AddRange(PlayerInventoryManager.Instance.Phantoms);
        if (playerInactiveCombatants.Count > 0)
        {
            playerInactiveCombatants.RemoveAt(PlayerInventoryManager.CurrentActivePhantom);
        }

        m_battleInitializer.InitializeBattle(playerCombatants, playerInactiveCombatants, Enemies);
    }
}
