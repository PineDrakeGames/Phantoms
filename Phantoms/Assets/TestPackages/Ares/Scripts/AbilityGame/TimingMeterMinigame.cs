using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimingMeterMinigame : AbilityMinigame
{
    public class TimingMeterTarget
    {
        [Range(0f,1f)]
        public float TargetPercent = 1f; 

        public MinigameInput InputRequired = MinigameInput.DOWN;

        public bool hit = false;
    }

    [Header("Minigame Variables")]
    public float MeterDuration = 1.5f;

    public float TargetTimeWindow = 0.25f;

    public List<TimingMeterTarget> Targets = new List<TimingMeterTarget>();

    [Header("References to parts of the Meter")]
    [SerializeField]
    private Transform m_fillArea = null;

    [SerializeField]
    private Image m_meterFillImage = null;


    /// Private variables
    private float m_currentTime = 0f;
    private int m_currentTargetIndex = 0;

    protected override void Restart()
    {
        m_meterFillImage.fillAmount = 0f;
    }

    protected override void InitializingState()
    {
        // Set up the targets so they are listed in order, and set them all to not being hit.
        Targets.Sort((t1, Texture2D)=>t1.TargetPercent.CompareTo(Texture2D.TargetPercent));
        foreach(TimingMeterTarget target in Targets)
        {
            target.hit = false;
        }

        // Instantiate the targets

        m_currentTargetIndex = 0;
        m_currentTime = 0f;

        m_state = MinigameState.RUNNING;
    }

    protected override void RunningState()
    {

        float prevTime = m_currentTime;
        m_currentTime += Time.deltaTime;

        float progress = m_currentTime / MeterDuration;
        progress = Mathf.Clamp01(progress);

        m_meterFillImage.fillAmount = progress;


    }

    protected override void FinishedState()
    {

    }
}
