using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerBattleInstanceData : UserBattleInstanceData
{
    

    public PlayerBattleData PlayerData
    {
        get
        { 
            if (Data == null)
            {
                Data = DataManager.Instance.GetPlayerBattleData();
            }
            return (PlayerBattleData)Data;
        }
    }

    public PlayerBattleInstanceData(PlayerBattleData playerData)
    {
        Data = playerData;
    }

    public override void SetCurrentStats()
    {
        BattleStats startingStats = new BattleStats(PlayerData.DefaultStats);

        foreach(RelicInstance relic in Relics)
        {
            if (relic.Data is StatRelicData)
            {
                StatRelicData statRelic = relic.Data as StatRelicData;
                foreach(StatRelicData.StatBuffData buffData in statRelic.StatBuffs)
                {
                    startingStats.SetStat(buffData.Stat, startingStats.GetStat(buffData.Stat) + buffData.Amount);
                }
            }
        }

        CurrentStats = startingStats;
    }

    public override string GetDisplayName()
    {
        return Data.DisplayName;
    }

    public override bool CanEquipRelic (RelicInstance relic)
    {
        if (base.CanEquipRelic(relic) && relic.Data.EquipType != RelicEquipType.PHANTOM_ONLY)
        {
            return true;
        }
        return false;
    }
}
