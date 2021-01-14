using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerBattleInstanceData : CombatantInstanceData
{
    public PlayerBattleData PhanData
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
}
