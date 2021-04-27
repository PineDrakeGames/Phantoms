using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ares;

[CreateAssetMenu(menuName = "Phantoms/Phantom Data")]
public class PhantomData : UserBattleData
{
    [Header("Stats stuff")]
    public PhantomType MainType = PhantomType.NONE;
    public PhantomType SecondType = PhantomType.NONE;

    [Tooltip("Initial values, before any level ups applied.")]
    public BattleStats StartingStats;

    [Tooltip("The amount that each stat increases when leveled for this phantom")]
    public LevelUpStats LevelUpAmounts;
}
