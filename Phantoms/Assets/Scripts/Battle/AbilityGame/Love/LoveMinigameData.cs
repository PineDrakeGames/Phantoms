using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoveMinigameData : AbilityMinigameData
{
    public override AbilityMinigameType type { get { return AbilityMinigameType.LOVE; } }

    [Header("Overall Settings")]
    public float TimerDuration = 7f;
    public int NumTargets = 3;

    [Header("Aiming Reticle Settings")]
    public Vector2 ReticlePosition = new Vector2(-1f, -1f);

    public float AimAngleRange = 45f;

    [Header("Bullet Settings")]
    public float BulletSpeed = 800f;

    [Header("Target Settings")]
    public float MinTargetDistance = 200f;
}
