using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PhantomInstanceData : UserBattleInstanceData
{
    public string PhantomID = null;

    public string NickName = null;

    public PhantomBackground Background = PhantomBackground.None;
    
    public PhantomData PhanData
    {
        get
        { 
            if (Data == null)
            {
                Data = DataManager.Instance.TryGetPhantomData(PhantomID);
            }
            if (string.IsNullOrEmpty(PhantomID))
            {
                PhantomID = Data.ID;
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

    public PhantomInstanceData(PhantomData data)
    {
        Data = data;
        PhantomID = data.ID;
        NickName = string.Empty;
        Level = 0;
        Background = PhantomBackground.None;
        LevelUps = new LevelUpStats();
        LevelUps.SetToZero();
        CurrentStats = new BattleStats();
    }

    public PhantomInstanceData(PhantomInstanceData phantomData)
    {
        PhantomID = phantomData.PhantomID;
        NickName = phantomData.NickName;
        Level = phantomData.Level;
        Background = phantomData.Background;
        LevelUps = phantomData.LevelUps;
        CurrentStats = phantomData.CurrentStats;
        Data = phantomData.Data;
        CurrentHP = phantomData.CurrentHP;
        CurrentMana = phantomData.CurrentMana;
        Experience = phantomData.Experience;
    }

    public override void SetCurrentStats()
    {
        BattleStats startingStats = new BattleStats(PhanData.StartingStats);

        // Setting up background stats - for now, just subtract the decrease and add the increase
        BackgroundStats backStats = PhantomDataUtility.BackgroundToStats[Background];
        BattleStatType increase = backStats.Increase;
        BattleStatType decrease = backStats.Decrease;
        startingStats.SetStat(increase, startingStats.GetStat(increase) + (PhanData.LevelUpAmounts.GetStat(increase)/2));
        startingStats.SetStat(decrease, startingStats.GetStat(decrease) - (PhanData.LevelUpAmounts.GetStat(decrease)/2));

        ApplyLevelUps(ref startingStats);
        ApplyRelics(ref startingStats);

        CurrentStats = startingStats;

        base.SetCurrentStats();
    }

    public override List<BattleStatType> LevelUpOptions()
    {
        List<BattleStatType> levelUpOptions = new List<BattleStatType>();
        BackgroundStats backStats = PhantomDataUtility.BackgroundToStats[Background];

        foreach(BattleStatType statType in (PhantomDataUtility.LevelUpStats))
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

    public override string GetDisplayName()
    {
        if (!string.IsNullOrEmpty(NickName))
        {
            return NickName;
        }
        else
        {
            return Data.DisplayName;
        }
    }

    public override bool CanEquipRelic (RelicInstance relic)
    {
        if (base.CanEquipRelic(relic) && relic.Data.EquipType != RelicEquipType.PLAYER_ONLY)
        {
            return true;
        }
        return false;
    }
}
