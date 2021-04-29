using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ares;

public abstract class UserBattleData : CombatantData
{
    [Tooltip("Initial values, before any level ups applied.")]
    public BattleStats StartingStats;

    [Tooltip("The amount that each stat increases when leveled for this phantom")]
    public LevelUpStats LevelUpAmounts;
}
