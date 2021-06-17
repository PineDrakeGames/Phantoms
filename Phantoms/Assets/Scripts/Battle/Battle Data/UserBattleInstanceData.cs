using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public abstract class UserBattleInstanceData : CombatantInstanceData
{
    public UserBattleData UserData
    {
        get
        {
            return (UserBattleData)Data;
        }
    }

    public List<RelicInstance> Relics = new List<RelicInstance>();

    public int CurrentRelicPoints = 0;

    // Levels and stats stuff
    public int Level = 0;
    public int Experience = 0;
    public LevelUpStats LevelUps = new LevelUpStats();

    // Events that can be invoked when stuff is changed/Updated
    public UnityEvent OnStatsUpdate = new UnityEvent();

    public virtual void SetCurrentStats()
    {
        OnStatsUpdate.Invoke();
    }

    // Public Functions to equip and unequip relics, making sure stuff is set up.
    public void EquipRelic(RelicInstance relic)
    {
        if (relic != null && !Relics.Contains(relic))
        {
            relic.Equipped = true;
            relic.User = this;
            Relics.Add(relic);
            CurrentRelicPoints += relic.Data.Points;
        }
    }

    public void UnequipRelic(RelicInstance relic)
    {
        if (relic != null && Relics.Contains(relic))
        {
            relic.Equipped = false;
            relic.User = null;
            Relics.Remove(relic);
            CurrentRelicPoints -= relic.Data.Points;
            SetCurrentStats();
        }
    }

    public virtual bool CanEquipRelic (RelicInstance relic)
    {
        if (relic == null) { return false; }
        return (CurrentRelicPoints + relic.Data.Points) <= CurrentStats.Relic;
    }

    // Public functions relating to level up stuff
    public virtual List<BattleStatType> LevelUpOptions()
    {
        List<BattleStatType> statTypes = new List<BattleStatType>();
        statTypes.AddRange(PhantomDataUtility.LevelUpStats);
        return statTypes;
    }


    // Protected helper functions that both the player and phantoms can use
    protected void ApplyLevelUps(ref BattleStats stats)
    {

        // Add in level ups!
        int totalLevel = 0;
        foreach(BattleStatType type in PhantomDataUtility.LevelUpStats)
        {
            int levels = LevelUps.GetStat(type);
            for (int i = 0; i < levels; i++)
            {
                stats.SetStat(type, stats.GetStat(type) + UserData.LevelUpAmounts.GetStat(type));
            }
            totalLevel += levels;
        }
        Level = totalLevel;
    }

    protected void ApplyRelics(ref BattleStats stats)
    {
        foreach(RelicInstance relic in Relics)
        {
            if (relic.Data is StatRelicData)
            {
                StatRelicData statRelic = relic.Data as StatRelicData;
                foreach(StatRelicData.StatBuffData buffData in statRelic.StatBuffs)
                {
                    stats.SetStat(buffData.Stat, stats.GetStat(buffData.Stat) + buffData.Amount);
                }
            }
        }
    }
}
