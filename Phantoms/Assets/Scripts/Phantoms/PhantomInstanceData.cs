using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PhantomInstanceData : CombatantInstanceData
{
    public string PhantomID = null;

    public string NickName = null;

    public int Level = 0;

    public PhantomBackground Background = PhantomBackground.None;

    public LevelUpStats LevelUps = new LevelUpStats();
    
    public PhantomData PhanData
    {
        get
        { 
            if (Data == null)
            {
                Data = DataManager.Instance.TryGetPhantomData(PhantomID);
            }
            return (PhantomData)Data;
        }
    }

    public PhantomInstanceData()
    {
        PhantomID = string.Empty;
        NickName = string.Empty;
        Level = 0;
        Background = PhantomBackground.None;
        LevelUps = new LevelUpStats();
        LevelUps.SetToZero();
        CurrentStats = new BattleStats();
    }

    public void SetCurrentStats()
    {
        BattleStats startingStats = new BattleStats(PhanData.StartingStats);

        // Setting up background stats - for now, just subtract the decrease and add the increase
        BackgroundStats backStats = PhantomDataUtility.BackgroundToStats[Background];
        BattleStatType increase = backStats.Increase;
        BattleStatType decrease = backStats.Decrease;
        startingStats.SetStat(increase, startingStats.GetStat(increase) + (PhanData.LevelUpAmounts.GetStat(increase)/2));
        startingStats.SetStat(decrease, startingStats.GetStat(decrease) - (PhanData.LevelUpAmounts.GetStat(decrease)/2));

        // Add in level ups!
        foreach(BattleStatType type in PhantomDataUtility.LevelUpStats)
        {
            for (int i = 0; i < LevelUps.GetStat(type); i++)
            {
                startingStats.SetStat(type, startingStats.GetStat(type) + PhanData.LevelUpAmounts.GetStat(type));
            }
        }

        CurrentStats = startingStats;
    }

    public List<BattleStatType> LevelUpOptions()
    {
        List<BattleStatType> levelUpOptions = new List<BattleStatType>();
        BackgroundStats backStats = PhantomDataUtility.BackgroundToStats[Background];

        foreach(BattleStatType statType in (System.Enum.GetValues(typeof(BattleStatType)) as BattleStatType[]))
        {
            int statLevels = LevelUps.GetStat(statType);
            if (statType == backStats.Decrease)
            {
                if (statLevels < 2)
                {
                    levelUpOptions.Add(statType);
                }
            }
            else if (statType == backStats.Increase)
            {
                if (statLevels < 7)
                {
                    levelUpOptions.Add(statType);
                }
            }
            else
            {
                if (statLevels < 3)
                {
                    levelUpOptions.Add(statType);
                }
            }
        }

        return levelUpOptions;
    }
}
