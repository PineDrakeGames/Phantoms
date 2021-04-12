using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Phantoms/Ability Game/Button Mash Minigame")]
[System.Serializable]
public class ButtonMashMinigameData : AbilityMinigameData
{
    public override AbilityMinigameType type { get { return AbilityMinigameType.ANGER; } }

    public float MeterTotal = 1f;
    public float AmountPerButton = 0.1f;
    public float DrainRate = 1f;
    public float TimerDuration = 7f;

    public List<MinigameInput> Inputs = new List<MinigameInput>();

    public ButtonMashMinigameData()
    {
        Inputs.Add(MinigameInput.JUMP);
    }
}