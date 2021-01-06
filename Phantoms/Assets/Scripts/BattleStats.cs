using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleStatType
{
    MAXHP,
    ATTACK,
    DEFENSE,
    MANA,
    RELIC
}

[System.Serializable]
public class BattleStats
{
    public int MaxHP = 8;
    public int Attack = 1;
    public int Defense = 0;
    public int Mana = 5;
    public int Relic = 2;

    public BattleStats() { }

    public BattleStats(BattleStats other)
    {
        MaxHP = other.MaxHP;
        Attack = other.Attack;
        Defense = other.Defense;
        Mana = other.Mana;
        Relic = other.Relic;
    }

    public int GetStat(BattleStatType type)
    {
        switch (type)
        {
            case BattleStatType.MAXHP:
                return MaxHP;
            case BattleStatType.ATTACK:
                return Attack;
            case BattleStatType.DEFENSE:
                return Defense;
            case BattleStatType.MANA:
                return Mana;
            case BattleStatType.RELIC:
                return Relic;
            default:
                return 0;
        }
    }

    public void SetStat(BattleStatType type, int newValue)
    {
        switch (type)
        {
            case BattleStatType.MAXHP:
                MaxHP = newValue;
                break;
            case BattleStatType.ATTACK:
                Attack = newValue;
                break;
            case BattleStatType.DEFENSE:
                Defense = newValue;
                break;
            case BattleStatType.MANA:
                Mana = newValue;
                break;
            case BattleStatType.RELIC:
                Relic = newValue;
                break;
            default:
                break;
        }
    }

    public void SetToZero()
    {
        MaxHP = 0;
        Attack = 0;
        Defense = 0;
        Mana = 0;
        Relic = 0;
    }
}