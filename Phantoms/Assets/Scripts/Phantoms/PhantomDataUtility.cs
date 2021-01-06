using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PhantomBackground
{
    None = 0,
    Sailor = 1,
    Chef = 2,
    Carpenter = 3,
    Farmer = 4,
    Thief = 5,
    Soldier = 6,
    Miner = 7,
    Athlete = 8,
    Theologist = 9,
    Architect = 10,
    Teacher = 11,
    Guard = 12,
    Royalty = 13,
    Healer = 14,
    Leader = 15,
    Traveler = 16,
    Merchant = 17,
    Scientist = 18,
    Entertainer = 19,
    Artist = 20
}

public struct BackgroundStats
{
    BattleStatType Increase;
    BattleStatType Decrease;

    public BackgroundStats(BattleStatType increase, BattleStatType decrease)
    {
        Increase = increase;
        Decrease = decrease;
    }
}

public static class PhantomDataUtility
{
    public static Dictionary<PhantomBackground, BackgroundStats> BackgroundToStats = new Dictionary<PhantomBackground, BackgroundStats>
    {
        { (PhantomBackground)1, new BackgroundStats(BattleStatType.MAXHP,  BattleStatType.ATTACK) },
        { (PhantomBackground)2, new BackgroundStats(BattleStatType.MAXHP,  BattleStatType.DEFENSE) },
        { (PhantomBackground)3, new BackgroundStats(BattleStatType.MAXHP,  BattleStatType.MANA) },
        { (PhantomBackground)4, new BackgroundStats(BattleStatType.MAXHP,  BattleStatType.RELIC) },
        { (PhantomBackground)5, new BackgroundStats(BattleStatType.ATTACK,  BattleStatType.MAXHP) },
        { (PhantomBackground)6, new BackgroundStats(BattleStatType.ATTACK,  BattleStatType.DEFENSE) },
        { (PhantomBackground)7, new BackgroundStats(BattleStatType.ATTACK,  BattleStatType.MANA) },
        { (PhantomBackground)8, new BackgroundStats(BattleStatType.ATTACK,  BattleStatType.RELIC) },
        { (PhantomBackground)9, new BackgroundStats(BattleStatType.DEFENSE,  BattleStatType.MAXHP) },
        { (PhantomBackground)10, new BackgroundStats(BattleStatType.DEFENSE,  BattleStatType.ATTACK) },
        { (PhantomBackground)11, new BackgroundStats(BattleStatType.DEFENSE,  BattleStatType.MANA) },
        { (PhantomBackground)12, new BackgroundStats(BattleStatType.DEFENSE,  BattleStatType.RELIC) },
        { (PhantomBackground)13, new BackgroundStats(BattleStatType.MANA,  BattleStatType.MAXHP) },
        { (PhantomBackground)14, new BackgroundStats(BattleStatType.MANA,  BattleStatType.ATTACK) },
        { (PhantomBackground)15, new BackgroundStats(BattleStatType.MANA,  BattleStatType.DEFENSE) },
        { (PhantomBackground)16, new BackgroundStats(BattleStatType.MANA,  BattleStatType.RELIC) },
        { (PhantomBackground)17, new BackgroundStats(BattleStatType.RELIC,  BattleStatType.MAXHP) },
        { (PhantomBackground)18, new BackgroundStats(BattleStatType.RELIC,  BattleStatType.ATTACK) },
        { (PhantomBackground)19, new BackgroundStats(BattleStatType.RELIC,  BattleStatType.DEFENSE) },
        { (PhantomBackground)20, new BackgroundStats(BattleStatType.RELIC,  BattleStatType.MANA) }
    };
}
