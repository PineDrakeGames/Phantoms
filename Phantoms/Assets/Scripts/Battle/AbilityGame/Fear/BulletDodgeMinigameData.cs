using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Phantoms/Ability Game/Bullet Dodge Minigame")]
[System.Serializable]
public class BulletDodgeMinigameData : AbilityMinigameData
{
    public override AbilityMinigameType type { get { return AbilityMinigameType.FEAR; } }

    public float TimerDuration = 7f;

    public List<BulletSpawnerData> BulletSpawners = new List<BulletSpawnerData>();

    [Header("Player Variables")]
    [Tooltip("Units are canvas units/second")]
    public float PlayerSpeed = 500f;

    public int Health = 3;
}


[System.Serializable]
public class BulletSpawnerData
{
    public enum SpawnPoint
    {
        TOP,
        BOTTOM,
        LEFT,
        RIGHT
    }

    public SpawnPoint Spawn = SpawnPoint.RIGHT;

    public BulletDodgeMinigameBullet.BulletMoveType BulletType = BulletDodgeMinigameBullet.BulletMoveType.LINEAR;

    public float StartDelay = 1f;

    public float BulletSpawnInterval = 0.5f;
    public float BulletSpawnIntervalVariance = 0f;

    public float BulletLifetime = 5f;
    public float BulletLifetimeVariance = 0f;

    public float BulletStartSpeed = 500f;
    public float BulletStartSpeedVariance = 0f;

    public float BulletStartAngle = 0f;
    public float BulletStartAngleVariance = 0f;
}

public class BulletSpawner
{
    public BulletSpawnerData Data;

    public float TimeToNextSpawn = 0f;
}