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

    public Sprite IconFill = null;
    public Sprite IconLines = null;

    [Header("Things needed for ARES Combat")]
    public GameObject BattlePrefab = null;
    public Ability[] Abilities;
}
