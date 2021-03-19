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

    public void RestoreHealth()
    {
        CurrentHP = CurrentStats.MaxHP;
    }

    public void RestoreMana()
    {
        CurrentMana = CurrentStats.Mana;
    }

    public void RestoreHealth(int amount)
    {
        if (amount > 0)
        {
            CurrentHP += amount;
            CurrentHP = Mathf.Clamp(CurrentHP, 0, CurrentStats.MaxHP);
        }
    }

    public void RestoreMana(int amount)
    {
        if (amount > 0)
        {
            CurrentMana += amount;
            CurrentMana = Mathf.Clamp(CurrentMana, 0, CurrentStats.Mana);
        }
    }
}
