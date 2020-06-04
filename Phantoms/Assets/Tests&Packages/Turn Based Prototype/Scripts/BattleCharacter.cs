using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacter : MonoBehaviour
{

    public int MaxHealth;
    public int CurrentHealth;

    public void Damage(int damage)
    {
        if (damage > 0)
        {
            CurrentHealth -= damage;
            if (CurrentHealth < 0)
            {
                CurrentHealth = 0;
            }
        }
    }

    public void Heal (int heal)
    {
        if (heal > 0)
        {
            CurrentHealth += heal;
            if (CurrentHealth > MaxHealth)
            {
                CurrentHealth = MaxHealth;
            }
        }
    }
}
