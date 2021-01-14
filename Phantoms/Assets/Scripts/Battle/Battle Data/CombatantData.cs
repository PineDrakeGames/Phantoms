using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ares;

public abstract class CombatantData : ScriptableObject
{
    [Header("Things needed for ARES Combat")]
    public GameObject BattlePrefab = null;
    public Ability[] Abilities;
}
