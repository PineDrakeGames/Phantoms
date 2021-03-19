using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ares;

public abstract class CombatantData : ScriptableObject
{
    [Header("Info for Every Combatant")]
    public string ID = string.Empty;
    public string DisplayName;
    public string Description = string.Empty;

    [Header("Things needed for ARES Combat")]
    public GameObject BattlePrefab = null;
    public Ability[] Abilities;
}
