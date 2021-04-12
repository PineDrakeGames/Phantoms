using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Phantoms/Ability Game/Timing Meter Minigame")]
[System.Serializable]
public class TimingMeterMinigameData : AbilityMinigameData
{
    public override AbilityMinigameType type { get { return AbilityMinigameType.JOY_METER; } }

    public float MeterDuration = 1.5f;

    public float TargetTimeWindow = 0.35f;
    public float PerfectTimeWindow = 0.05f;

    public int AllowedMisses = 0;

    public List<TimingMeterTargetData> Targets = new List<TimingMeterTargetData>();
}

[System.Serializable]
public class TimingMeterTargetData
{
    [Range(0f, 1f)]
    public float TargetPercent = 1f;
    public MinigameInput InputRequired = MinigameInput.DOWN;
}