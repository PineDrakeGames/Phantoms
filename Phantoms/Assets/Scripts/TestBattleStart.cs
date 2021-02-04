using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ares;

public class TestBattleStart : MonoBehaviour
{
    [SerializeField]
    private BattleInitializer m_battleInitializer = null;

    [SerializeField]
    private PlayerBattleInstanceData m_playerData = null;

    [SerializeField]
    private PhantomInstanceData[] m_enemyPhantoms = null;
    // Start is called before the first frame update
    void Start()
    {
        List<CombatantInstanceData> playerCombatants = new List<CombatantInstanceData>();
        playerCombatants.Add(m_playerData);

        // For now, just assume that the first in the list is the current phantom.
        if (PlayerInventoryManager.Instance.Phantoms.Count >= 1)
        playerCombatants.Add(PlayerInventoryManager.Instance.GetCurrentPhantom());

        List<CombatantInstanceData> playerInactiveCombatants = new List<CombatantInstanceData>();
        playerInactiveCombatants.AddRange(PlayerInventoryManager.Instance.Phantoms);
        if (playerInactiveCombatants.Count > 0)
        {
            playerInactiveCombatants.RemoveAt(PlayerInventoryManager.Instance.CurrentActivePhantom);
        }


        List<CombatantInstanceData> enemyCombatants = new List<CombatantInstanceData>();
        foreach(PhantomInstanceData phantomData in m_enemyPhantoms)
        {
            phantomData.SetCurrentStats();
            phantomData.CurrentHP = phantomData.CurrentStats.MaxHP;
            phantomData.CurrentMana = phantomData.CurrentStats.Mana;
        }
        enemyCombatants.AddRange(m_enemyPhantoms);

        m_battleInitializer.InitializeBattle(playerCombatants, playerInactiveCombatants, enemyCombatants);
    }
}
