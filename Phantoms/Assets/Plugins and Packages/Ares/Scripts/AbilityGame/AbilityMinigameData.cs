using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum AbilityMinigameType
{
    LOVE,
    FEAR,
    ANGER,
    SADNESS,
    CALM,
    HOPE,
    JOY_LIGHTS,
    JOY_METER,
    SHAME
}

public abstract class AbilityMinigameData : ScriptableObject
{
    [HideInInspector]
    public abstract AbilityMinigameType type { get; }
}