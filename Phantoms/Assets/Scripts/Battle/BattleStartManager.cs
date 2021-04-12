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
    public static List<PhantomInstanceData> EnemyPhantoms = null;
    
    #if UNITY_EDITOR
    private void Awake()
    {
        if (EnemyPhantoms == null)
        {
            List<PhantomInstanceData> encounterPhantoms = m_enemyEncounterData.GetEnemyPhantoms();
            if (encounterPhantoms.Count > 0)
            {
                BattleStartManager.EnemyPhantoms = encounterPhantoms;
            }
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
            playerInactiveCombatants.RemoveAt(PlayerInventoryManager.Instance.CurrentActivePhantom);
        }


        List<CombatantInstanceData> enemyCombatants = new List<CombatantInstanceData>();
        foreach(PhantomInstanceData phantomData in EnemyPhantoms)
        {
            phantomData.SetCurrentStats();
            phantomData.CurrentHP = phantomData.CurrentStats.MaxHP;
            phantomData.CurrentMana = phantomData.CurrentStats.Mana;
        }
        enemyCombatants.AddRange(EnemyPhantoms);

        m_battleInitializer.InitializeBattle(playerCombatants, playerInactiveCombatants, enemyCombatants);
    }
}
