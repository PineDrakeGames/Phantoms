using UnityEngine;

[CreateAssetMenu(menuName = "Phantoms/Ability Game/Bullet Dodge Minigame")]
[System.Serializable]
public class BulletDodgeMinigameData : AbilityMinigameData
{
    public override AbilityMinigameType type { get { return AbilityMinigameType.FEAR; } }

    public float TimerDuration = 7f;

    [Header("Player Variables")]
    [Tooltip("Units are canvas units/second")]
    public float PlayerSpeed = 500f;
}


[System.Serializable]
public class BulletSpawner
{
    
}