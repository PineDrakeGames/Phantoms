using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimingMeterMinigame : AbilityMinigame
{

    public class TimingMeterTarget
    {
        public TimingMeterTargetData Data;
        public MinigameResult result = MinigameResult.FAIL;
        public InputIndicator Indicator = null;

        public TimingMeterTarget(TimingMeterTargetData data)
        {
            Data = data;
            result = MinigameResult.FAIL;
        }
    }


    [Header("Minigame Variables")]
    public TimingMeterMinigameData Data;

    [Header("References to parts of the Meter")]
    [SerializeField]
    private GameObject m_meter = null;

    [SerializeField]
    private RectTransform m_fillArea = null;

    [SerializeField]
    private Image m_meterFillImage = null;

    [Header("Target Prefab")]
    [SerializeField]
    private GameObject m_inputIndicator;

    /// Public Getters
    public override string MinigameDescription
    {
        get
        {
            return "Press the indicated button when it lights up!";
        }
    }

    /// Private variables
    private float m_currentTime = 0f;
    private int m_currentTargetIndex = 0;

    private float m_targetPercentArea = 0f;

    private List<TimingMeterTarget> m_targetInstances = new List<TimingMeterTarget>();
    private List<GameObject> m_inputIndicators = new List<GameObject>();

    private bool m_initialized = false;
    private const float INITIALIZE_TIME = 0.5f;

    private void Start()
    {
        m_meter.SetActive(false);
    }

    protected override void Restart()
    {
        m_meterFillImage.fillAmount = 0f;
        m_initialized = false;
    }

    protected override void InitializingState()
    {
        if (!m_initialized)
        {
            m_meter.SetActive(true);

            // Calculate some stuff
            m_targetPercentArea = Data.TargetTimeWindow / Data.MeterDuration;

            float targetWidth = m_targetPercentArea * m_fillArea.rect.width;

            // Set up the targets so they are listed in order, and set them all to not being hit.
            foreach (TimingMeterTargetData data in Data.Targets)
            {
                m_targetInstances.Add(new TimingMeterTarget(data));
            }

            m_targetInstances.Sort((t1, t2) => t1.Data.TargetPercent.CompareTo(t2.Data.TargetPercent));

            foreach (TimingMeterTarget target in m_targetInstances)
            {
                // Instantiate a target
                GameObject indicator = GetInputIndicator();

                // Move it to the right position
                RectTransform indicatorTransform = indicator.GetComponent<RectTransform>();
                indicatorTransform.anchorMin = new Vector2(target.Data.TargetPercent, 0.5f);
                indicatorTransform.anchorMax = new Vector2(target.Data.TargetPercent, 0.5f);

                //Set the width based on the time window and size of the fill area.
                indicatorTransform.sizeDelta = new Vector2(targetWidth, targetWidth);

                // Set the reference
                target.Indicator = indicator.GetComponent<InputIndicator>();
                target.Indicator.SetState(InputIndicator.IndicatorState.WAITING);
                target.Indicator.SetIndicator(target.Data.InputRequired);
            }

            m_currentTargetIndex = 0;
            m_currentTime = 0f;
            m_initialized = true;
        }
        else
        {
            m_currentTime += Time.deltaTime;
            if (m_currentTime >= INITIALIZE_TIME)
            {
                m_currentTime = 0f;
                m_state = MinigameState.RUNNING;
            }
        }
    }

    protected override void RunningState()
    {

        float prevTime = m_currentTime;
        m_currentTime += Time.deltaTime;

        float prevProgress = prevTime / Data.MeterDuration;
        float progress = m_currentTime / Data.MeterDuration;

        TimingMeterTarget target = null;
        if (m_currentTargetIndex < m_targetInstances.Count) { target = m_targetInstances[m_currentTargetIndex]; }

        // Check if we hit the target (if there is any left)
        if (target != null)
        {
            float minProgress = target.Data.TargetPercent - (m_targetPercentArea * 0.5f);
            float maxProgress = target.Data.TargetPercent + (m_targetPercentArea * 0.5f);

            if (prevProgress < minProgress && progress >= minProgress)
            {
                // First entered target zone
                target.Indicator.SetState(InputIndicator.IndicatorState.READY_FOR_INPUT);
            }

            // Check if too late (Make sure we had at least one frame while in the target zone - unlikely but possible?)
            if (prevProgress >= minProgress && progress > maxProgress)
            {
                target.result = MinigameResult.FAIL;
                target.Indicator.SetState(InputIndicator.IndicatorState.FAILED);
                m_currentTargetIndex += 1;
            }
            // Otherwise, check input
            else if (MinigameInputManager.CheckInputDown(target.Data.InputRequired))
            {
                if (progress < minProgress)
                {
                    target.result = MinigameResult.FAIL;
                    target.Indicator.SetState(InputIndicator.IndicatorState.FAILED);
                }
                else
                {
                    target.result = MinigameResult.SUCCESS;
                    target.Indicator.SetState(InputIndicator.IndicatorState.INPUT_RECIEVED);
                }
                m_currentTargetIndex += 1;
            }
        }

        m_meterFillImage.fillAmount = Mathf.Clamp01(progress);

        if ((progress >= 1f) && (m_currentTargetIndex >= m_targetInstances.Count))
        {
            SetResult();
            m_state = MinigameState.FINISHED;
        }
    }

    private void SetResult()
    {
        int misses = 0;
        foreach (TimingMeterTarget target in m_targetInstances)
        {
            if (target.result == MinigameResult.FAIL)
            {
                misses += 1;
            }
        }

        if (misses > Data.AllowedMisses)
        {
            m_result = MinigameResult.FAIL;
        }
        else
        {
            m_result = MinigameResult.SUCCESS;
        }

        foreach (GameObject indicator in m_inputIndicators)
        {
            indicator.SetActive(false);
        }

        // Clear arrays
        m_targetInstances.Clear();

        m_meter.SetActive(false);
    }

    protected override void FinishedState()
    {

    }

    private GameObject GetInputIndicator()
    {
        foreach (GameObject indicator in m_inputIndicators)
        {
            if (!indicator.activeSelf)
            {
                indicator.SetActive(true);
                return indicator;
            }
        }

        GameObject newIndicator = Instantiate(m_inputIndicator, m_fillArea);
        m_inputIndicators.Add(newIndicator);
        return newIndicator;
    }
}
