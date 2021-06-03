using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class UserBattleInstanceData : CombatantInstanceData
{
    public List<RelicInstance> Relics = new List<RelicInstance>();

    public int CurrentRelicPoints = 0;

    // Levels and stats stuff
    public int Level = 0;
    public int Experience = 0;
    public LevelUpStats LevelUps = new LevelUpStats();

    public abstract void SetCurrentStats();

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
}
