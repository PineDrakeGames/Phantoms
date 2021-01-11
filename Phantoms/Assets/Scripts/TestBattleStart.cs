using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ares;

public class TestBattleStart : MonoBehaviour
{
    [SerializeField]
    private BattleManager m_battleManager = null;

    [SerializeField]
    private Actor m_playerActor = null;

    [SerializeField]
    private PlayerBattleInstanceData m_playerData = null;

    [SerializeField]
    private Actor[] m_phantomActors = null;

    [SerializeField]
    private PhantomInstanceData[] m_phantomData = null;
    // Start is called before the first frame update
    void Start()
    {
        int Count = Mathf.Min(m_phantomActors.Length, m_phantomData.Length);

        for (int i = 0; i < Count; i++)
        {
            m_phantomData[i].SetCurrentStats();
            PhantomDataUtility.SetPhantomActor(m_phantomActors[i], m_phantomData[i]);
        }

        if (m_playerActor != null && m_playerData != null)
        {
            Dictionary<string, int> stats = new Dictionary<string, int>();
            stats.Add("attack", m_playerData.CurrentStats.Attack);
            stats.Add("defense", m_playerData.CurrentStats.Defense);

            string displayName = "Player";

            m_playerActor.Init(displayName, m_playerData.CurrentStats.MaxHP, m_playerData.CurrentStats.MaxHP, stats, m_playerData.Data.Abilities, m_playerActor.FallbackAbility, m_playerActor.Afflictions, m_playerActor.inventory);
            m_playerActor.MainType = PhantomType.NONE;
            m_playerActor.SecondType = PhantomType.NONE;
        }

        m_battleManager.StartBattle();
    }
}
