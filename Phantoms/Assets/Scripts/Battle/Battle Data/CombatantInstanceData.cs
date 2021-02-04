using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CombatantInstanceData
{
    public CombatantData Data;

    public int CurrentHP = 0;
    public int CurrentMana = 0;

    public BattleStats CurrentStats = new BattleStats();
}
