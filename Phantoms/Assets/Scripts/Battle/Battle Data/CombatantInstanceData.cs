using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CombatantInstanceData
{
    public CombatantData Data;

    public BattleStats CurrentStats = new BattleStats();
}
