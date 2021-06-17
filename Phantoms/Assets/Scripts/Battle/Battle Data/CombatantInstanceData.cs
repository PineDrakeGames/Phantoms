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

    public void FullRestore()
    {
        RestoreHealth();
        RestoreMana();
    }

    public virtual void RestoreHealth()
    {
        CurrentHP = CurrentStats.MaxHP;
    }

    public virtual void RestoreMana()
    {
        CurrentMana = CurrentStats.Mana;
    }

    public virtual void RestoreHealth(int amount)
    {
        if (amount > 0)
        {
            CurrentHP += amount;
            CurrentHP = Mathf.Clamp(CurrentHP, 0, CurrentStats.MaxHP);
        }
    }

    public virtual void RestoreMana(int amount)
    {
        if (amount > 0)
        {
            CurrentMana += amount;
            CurrentMana = Mathf.Clamp(CurrentMana, 0, CurrentStats.Mana);
        }
    }

    public virtual void Damage(int amount)
    {
        if (amount > 0)
        {
            CurrentHP -= amount;
        }
        // TODO: Do something when you reach 0?
        // for now, just floor it to 1.
        if (CurrentHP < 1) { CurrentHP = 1; }
    }

    public virtual void UseMana(int amount)
    {
        if (amount > 0)
        {
            CurrentMana -= amount;
        }
        // TODO: Do something when you reach 0?
        if (CurrentMana < 0) { CurrentMana = 0; }
    }

    /// Public Data getter functions, to be overriden
    public virtual string GetDisplayName()
    {
        return Data.DisplayName;
    }
}
