using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerBattleInstanceData : CombatantInstanceData
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

    public void SetCurrentStats()
    {
        CurrentStats = new BattleStats(PlayerData.DefaultStats);
    }
}
