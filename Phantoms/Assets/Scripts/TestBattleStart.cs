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
    private PhantomInstanceData[] m_playerPhantoms = null;

    [SerializeField]
    private PhantomInstanceData[] m_enemyPhantoms = null;
    // Start is called before the first frame update
    void Start()
    {
        List<CombatantInstanceData> playerCombatants = new List<CombatantInstanceData>();
        playerCombatants.Add(m_playerData);
        playerCombatants.AddRange(m_playerPhantoms);

        List<CombatantInstanceData> enemyCombatants = new List<CombatantInstanceData>();
        enemyCombatants.AddRange(m_enemyPhantoms);
        m_battleInitializer.InitializeBattle(playerCombatants, enemyCombatants);
    }
}
