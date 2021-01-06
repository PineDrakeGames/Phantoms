using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomInstanceData
{
    public string PhantomID = null;

    public string NickName = null;

    public int Level = 0;

    public PhantomBackground Background = PhantomBackground.None;

    public BattleStats LevelUps = new BattleStats();

    public BattleStats CurrentStats = new BattleStats();

    private PhantomData m_data = null;
    public PhantomData Data
    {
        get
        { 
            if (m_data == null)
            {
                m_data = DataManager.PhantomData.IdToData[PhantomID];
            }
            return m_data;
        }
    }

    public PhantomInstanceData()
    {
        PhantomID = string.Empty;
        NickName = string.Empty;
        Level = 0;
        Background = PhantomBackground.None;
        LevelUps = new BattleStats();
        LevelUps.SetToZero();
        CurrentStats = new BattleStats();
    }

    public void SetCurrentStats()
    {
        BattleStats startingStats = new BattleStats(Data.StartingStats);

        // Setting up background stats - for now, just subtract the decrease and add the increase
        BackgroundStats backStats = PhantomDataUtility.BackgroundToStats[Background];
        BattleStatType increase = backStats.Increase;
        BattleStatType decrease = backStats.Decrease;
        startingStats.SetStat(increase, startingStats.GetStat(increase) + Data.LevelUpAmounts.GetStat(increase));
        startingStats.SetStat(decrease, startingStats.GetStat(decrease) - Data.LevelUpAmounts.GetStat(decrease));

        // Add in level ups!
        foreach(BattleStatType type in PhantomDataUtility.LevelUpStats)
        {
            for (int i = 0; i < LevelUps.GetStat(type); i++)
            {
                startingStats.SetStat(type, startingStats.GetStat(type) + Data.LevelUpAmounts.GetStat(type));
            }
        }

        CurrentStats = startingStats;
    }
}
