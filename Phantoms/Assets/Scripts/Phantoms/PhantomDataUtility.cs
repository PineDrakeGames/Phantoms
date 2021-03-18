using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ares;

/*
// OLD Backgrounds.
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
*/

public enum PhantomBackground
{
    None = 0,
    Soldier = 1,
    Athlete = 2,
    Scientist = 3,
    Theologist = 4,
    Thief = 5,
    Artist = 6,
}

public struct BackgroundStats
{
    public BattleStatType Increase;
    public BattleStatType Decrease;

    public BackgroundStats(BattleStatType increase, BattleStatType decrease)
    {
        Increase = increase;
        Decrease = decrease;
    }
}

public static class PhantomDataUtility
{
    /*
    // OLD background stuff.
    public static Dictionary<PhantomBackground, BackgroundStats> BackgroundToStats = new Dictionary<PhantomBackground, BackgroundStats>
    {
        { (PhantomBackground)0, new BackgroundStats(BattleStatType.MAXHP,  BattleStatType.MAXHP) },
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
    */

    public static Dictionary<PhantomBackground, BackgroundStats> BackgroundToStats = new Dictionary<PhantomBackground, BackgroundStats>
    {
        { (PhantomBackground)0, new BackgroundStats(BattleStatType.MAXHP,  BattleStatType.MAXHP) },
        { (PhantomBackground)1, new BackgroundStats(BattleStatType.MAXHP,  BattleStatType.MANA) },
        { (PhantomBackground)2, new BackgroundStats(BattleStatType.MAXHP,  BattleStatType.RELIC) },
        { (PhantomBackground)3, new BackgroundStats(BattleStatType.MANA,  BattleStatType.MAXHP) },
        { (PhantomBackground)4, new BackgroundStats(BattleStatType.MANA,  BattleStatType.RELIC) },
        { (PhantomBackground)5, new BackgroundStats(BattleStatType.RELIC,  BattleStatType.MAXHP) },
        { (PhantomBackground)6, new BackgroundStats(BattleStatType.RELIC,  BattleStatType.MANA) },
    };


    public static BattleStatType[] LevelUpStats = new BattleStatType[3]
    {
        BattleStatType.MAXHP,
        BattleStatType.MANA,
        BattleStatType.RELIC
    };

    public static void SetPhantomActor(Actor phantomActor, PhantomInstanceData instanceData)
    {
        Dictionary<string, int> stats = new Dictionary<string, int>();
        stats.Add("attack", instanceData.CurrentStats.Attack);
        stats.Add("defense", instanceData.CurrentStats.Defense);

        string displayName = instanceData.NickName;
        if (string.IsNullOrEmpty(displayName))
        {
            displayName = instanceData.PhanData.PhantomDisplayName;
        }

        phantomActor.Init(displayName, instanceData.CurrentHP, instanceData.CurrentStats.MaxHP, instanceData.CurrentMana, instanceData.CurrentStats.Mana, stats, instanceData.Data.Abilities, phantomActor.FallbackAbility, phantomActor.Afflictions, phantomActor.inventory);
        phantomActor.MainType = instanceData.PhanData.MainType;
        phantomActor.SecondType = instanceData.PhanData.SecondType;
    }

    public static PhantomInstanceData GenerateRandomPhantom(string phantomID, int level = 0)
    {
        return GenerateRandomPhantom(DataManager.Instance.TryGetPhantomData(phantomID), level);
    }

    public static PhantomInstanceData GenerateRandomPhantom(PhantomData phantomData, int level = 0)
    {
        if (phantomData == null)
        {
            return null;
        }


        PhantomInstanceData instanceData = new PhantomInstanceData();
        instanceData.Data = phantomData;

        // Get random background
        PhantomBackground[] valuesAsArray = System.Enum.GetValues(typeof(PhantomBackground)) as PhantomBackground[];
        int randomBackground = Random.Range(1, valuesAsArray.Length);
        instanceData.Background = valuesAsArray[randomBackground];

        // Get some level ups in there
        instanceData.Level = level;
        List<BattleStatType> randomPool = new List<BattleStatType>();
        BackgroundStats backStats = BackgroundToStats[instanceData.Background];
        for(int i = 0; i < level; i++)
        {
            List<BattleStatType> statOption = instanceData.LevelUpOptions();
            randomPool.Clear();
            foreach(BattleStatType statType in statOption)
            {
                if (statType == backStats.Increase)
                {
                    randomPool.Add(statType);
                    randomPool.Add(statType);
                    randomPool.Add(statType);
                    randomPool.Add(statType);
                }
                else if (statType == backStats.Decrease)
                {
                    randomPool.Add(statType);
                }
                else
                {
                    randomPool.Add(statType);
                    randomPool.Add(statType);
                }
            }

            int randomIndex = Random.Range(0, randomPool.Count);
            instanceData.LevelUps.SetStat(randomPool[randomIndex], instanceData.LevelUps.GetStat(randomPool[randomIndex]) + 1);
        }

        instanceData.SetCurrentStats();

        instanceData.CurrentHP = instanceData.CurrentStats.MaxHP;
        instanceData.CurrentMana = instanceData.CurrentStats.Mana;

        // Set initial nickname
        instanceData.NickName = phantomData.PhantomDisplayName;

        return instanceData;
    }
}
