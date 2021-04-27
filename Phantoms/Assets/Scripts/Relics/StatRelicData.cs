using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Phantoms/Relics/Stat Relic Data", order = 1)]
public class StatRelicData : RelicData
{
    [System.Serializable]
    public class StatBuffData
    {
        [SerializeField] private BattleStatType m_stat = BattleStatType.MAXHP;
        public BattleStatType Stat { get { return m_stat; } }

        // TODO: Make different ways of determining the amount, like making it scaled or something? For now, just static numbers
        [SerializeField] private int m_amount = 0;
        public int Amount { get { return m_amount; } }
    }

    [Header("Stats")]
    [SerializeField]
    private List<StatBuffData> m_statBuffs = new List<StatBuffData>();
    public List<StatBuffData> StatBuffs { get { return m_statBuffs; } }
}
