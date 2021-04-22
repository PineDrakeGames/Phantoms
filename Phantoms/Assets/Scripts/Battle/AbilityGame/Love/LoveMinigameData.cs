using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Phantoms/Ability Game/Love Minigame")]
[System.Serializable]
public class LoveMinigameData : AbilityMinigameData
{
    public override AbilityMinigameType type { get { return AbilityMinigameType.LOVE; } }

    [Header("Overall Settings")]
    public float TimerDuration = 7f;
    public int NumTargets = 3;

    [Header("Aiming Reticle Settings")]
    public Vector2 ReticlePosition = new Vector2(-1f, -1f);
    [MinMaxRange(-360f, 360f)] public RangedFloat AimAngleRange = new RangedFloat(0f, 90f);
    [Tooltip("Degrees per second")] public float AimSensitivity = 500f;

    [Header("Bullet Settings")]
    public LoveMinigameBullet.BulletMoveType BulletType = LoveMinigameBullet.BulletMoveType.LINEAR;
    public float BulletSpeed = 800f;
    public float BulletDuration = 5f;

    [Header("Target Settings")]
    public float MinTargetDistance = 200f;
}
