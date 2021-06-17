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
        BattleStats startingStats = new BattleStats(PlayerData.StartingStats);

        ApplyLevelUps(ref startingStats);
        ApplyRelics(ref startingStats);

        CurrentStats = startingStats;

        base.SetCurrentStats();
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

    // Overriding the health change functions to make sure they invoke the HP change event
    public override void RestoreHealth()
    {
        base.RestoreHealth();
        DataManager.PlayerHPChange.Invoke(CurrentHP);
    }

    public override void RestoreHealth(int amount)
    {
        base.RestoreHealth(amount);
        DataManager.PlayerHPChange.Invoke(CurrentHP);
    }

    public override void Damage(int amount)
    {
        base.Damage(amount);
        DataManager.PlayerHPChange.Invoke(CurrentHP);
    }
}
