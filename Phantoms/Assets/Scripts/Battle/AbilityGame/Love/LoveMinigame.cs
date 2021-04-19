using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoveMinigame : AbilityMinigame
{

    [Header("Minigame Variables")]
    public LoveMinigameData Data;

    [Header("Scene References")]
    [SerializeField]
    private RectTransform m_bulletLauncher = null;

    public override string MinigameDescription
    {
        get
        {
            return "Aim at the targets!";
        }
    }

    ////////////////////////////////////
    /// Protected override functions ///
    ////////////////////////////////////
    protected override void Restart()
    {

    }

    protected override void InitializingState()
    {
        AbilityMinigameManager.Timer.StartTimer(Data.TimerDuration);
        m_state = MinigameState.RUNNING;
    }

    protected override void RunningState()
    {
        ControlLauncher();
    }

    protected override void FinishedState()
    {

    }

    private void ControlLauncher()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
    }

    private void FireBullet()
    {

    }
}
