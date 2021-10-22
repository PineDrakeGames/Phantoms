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

    public void AddStats(BattleStats otherStats)
    {
        MaxHP += otherStats.MaxHP;
        Attack += otherStats.Attack;
        Defense += otherStats.Defense;
        Mana += otherStats.Mana;
        Relic += otherStats.Relic;
    }

    public void AddStats(BattleStatsFloat otherStats)
    {
        MaxHP += Mathf.FloorToInt(otherStats.MaxHP);
        Attack += Mathf.FloorToInt(otherStats.Attack);
        Defense += Mathf.FloorToInt(otherStats.Defense);
        Mana += Mathf.FloorToInt(otherStats.Mana);
        Relic += Mathf.FloorToInt(otherStats.Relic);
    }
}

//////////////////////
/// Level Up Stats ///
//////////////////////

// Very similar to battle stats, but JUST the stats that can be leveled up.
public enum LevelUpStatType
{
    MAXHP,
    MANA,
    RELIC
}

[System.Serializable]
public class LevelUpStats
{
    public int MaxHP = 8;
    public int Mana = 5;
    public int Relic = 2;

    public LevelUpStats() { }

    public LevelUpStats(LevelUpStats other)
    {
        MaxHP = other.MaxHP;
        Mana = other.Mana;
        Relic = other.Relic;
    }

    public int GetStat(BattleStatType type)
    {
        switch (type)
        {
            case BattleStatType.MAXHP:
                return MaxHP;
            case BattleStatType.MANA:
                return Mana;
            case BattleStatType.RELIC:
                return Relic;
            default:
                return 0;
        }
    }

    public int GetStat(LevelUpStatType type)
    {
        switch (type)
        {
            case LevelUpStatType.MAXHP:
                return MaxHP;
            case LevelUpStatType.MANA:
                return Mana;
            case LevelUpStatType.RELIC:
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

    public void SetStat(LevelUpStatType type, int newValue)
    {
        switch (type)
        {
            case LevelUpStatType.MAXHP:
                MaxHP = newValue;
                break;
            case LevelUpStatType.MANA:
                Mana = newValue;
                break;
            case LevelUpStatType.RELIC:
                Relic = newValue;
                break;
            default:
                break;
        }
    }

    public void SetToZero()
    {
        MaxHP = 0;
        Mana = 0;
        Relic = 0;
    }
}

[System.Serializable]
public class BattleStatsFloat
{
    public float MaxHP = 0f;
    public float Attack = 0f;
    public float Defense = 0f;
    public float Mana = 0f;
    public float Relic = 0f;

    public BattleStatsFloat() { }

    public BattleStatsFloat(BattleStatsFloat other)
    {
        MaxHP = other.MaxHP;
        Attack = other.Attack;
        Defense = other.Defense;
        Mana = other.Mana;
        Relic = other.Relic;
    }

    public float GetStat(BattleStatType type)
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

    public void SetStat(BattleStatType type, float newValue)
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

    public void AddStats(BattleStats otherStats)
    {
        MaxHP += otherStats.MaxHP;
        Attack += otherStats.Attack;
        Defense += otherStats.Defense;
        Mana += otherStats.Mana;
        Relic += otherStats.Relic;
    }

    public void ScaleStats(float scaleAmount)
    {
        MaxHP *= scaleAmount;
        Attack *= scaleAmount;
        Defense *= scaleAmount;
        Mana *= scaleAmount;
        Relic *= scaleAmount;
    }
}