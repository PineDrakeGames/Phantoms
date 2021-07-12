using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMashMinigame : AbilityMinigame
{
    public class ButtonMashInstance
    {
        public MinigameInput Input;
        public InputIndicator Indicator = null;

        public ButtonMashInstance(MinigameInput input)
        {
            Input = input;
        }
    }

    [Header("Minigame Variables")]
    public ButtonMashMinigameData Data;

    [Header("References to parts of the Meter")]
    [SerializeField]
    private GameObject m_meter = null;

    [SerializeField]
    private RectTransform m_inputIndicatorsParent = null;

    [SerializeField]
    private Image m_meterFillImage = null;

    [Header("Target Prefab")]
    [SerializeField]
    private GameObject m_inputIndicator;

    [Header("Sounds")]
    [SerializeField]
    private LoopingSoundEffect m_meterFillSound = null;

    [SerializeField]
    [MinMaxRange(-3f, 3f)]
    private RangedFloat m_minToMaxPitch = new RangedFloat(0.5f, 1.5f);

    /// Public Getters
    public override string MinigameDescription
    {
        get
        {
            return "Mash the buttons above repeatedly!";
        }
    }


    /// Private variables
    private float m_currentFill = 0f;

    private int m_currentTargetIndex = 0;

    private List<ButtonMashInstance> m_buttonsRequired = new List<ButtonMashInstance>();
    private List<GameObject> m_inputIndicators = new List<GameObject>();

    private void Start()
    {
        m_meter.SetActive(false);
    }

    protected override void Restart()
    {
        m_meterFillImage.fillAmount = 0f;
    }

    protected override void InitializingState()
    {
        m_meter.SetActive(true);

        // Calculate some stuff
        m_buttonsRequired.Clear();

        // Set up the targets so they are listed in order, and set them all to not being hit.
        foreach (MinigameInput input in Data.Inputs)
        {
            m_buttonsRequired.Add(new ButtonMashInstance(input));
        }

        foreach (ButtonMashInstance instance in m_buttonsRequired)
        {
            // Instantiate a target
            GameObject indicator = GetInputIndicator();

            // Set the reference
            instance.Indicator = indicator.GetComponent<InputIndicator>();
            instance.Indicator.SetState(InputIndicator.IndicatorState.WAITING);
            instance.Indicator.SetIndicator(instance.Input);
        }

        m_buttonsRequired[0].Indicator.SetState(InputIndicator.IndicatorState.READY_FOR_INPUT);

        m_currentTargetIndex = 0;
        m_currentFill = 0f;

        m_meterFillSound.SetPitch(m_minToMaxPitch.minValue);
        m_meterFillSound.Play();

        AbilityMinigameManager.Timer.StartTimer(Data.TimerDuration);

        m_state = MinigameState.RUNNING;
    }

    protected override void RunningState()
    {
        m_currentFill -= Time.deltaTime / Data.DrainRate;
        if (m_currentFill < 0)
        {
            m_currentFill = 0;
        }

        float progress = m_currentFill / Data.MeterTotal;

        ButtonMashInstance instance = null;
        if (m_currentTargetIndex < m_buttonsRequired.Count) { instance = m_buttonsRequired[m_currentTargetIndex]; }

        // Check if we hit the target (if there is any left)
        if (instance != null)
        {
            if (MinigameInputManager.CheckInputDown(instance.Input))
            {
                m_currentFill += Data.AmountPerButton;
                m_currentTargetIndex += 1;
                if (m_currentTargetIndex >= m_buttonsRequired.Count)
                {
                    m_currentTargetIndex = 0;
                }

                instance.Indicator.SetState(InputIndicator.IndicatorState.WAITING);
                m_buttonsRequired[m_currentTargetIndex].Indicator.SetState(InputIndicator.IndicatorState.READY_FOR_INPUT);
            }
        }

        m_meterFillImage.fillAmount = Mathf.Clamp01(progress);
        m_meterFillSound.SetPitch(Mathf.Lerp(m_minToMaxPitch.minValue, m_minToMaxPitch.maxValue, progress));

        if (progress >= 1f)
        {
            m_result = MinigameResult.SUCCESS;
            Finish();
        }
        else if (!AbilityMinigameManager.Timer.TimerActive)
        {
            m_result = MinigameResult.FAIL;
            Finish();
        }
    }

    private void Finish()
    {
        foreach (GameObject indicator in m_inputIndicators)
        {
            indicator.SetActive(false);
        }

        m_meterFillSound.Stop();

        m_meter.SetActive(false);

        AbilityMinigameManager.Timer.StopTimer();

        m_state = MinigameState.FINISHED;
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

        GameObject newIndicator = Instantiate(m_inputIndicator, m_inputIndicatorsParent);
        m_inputIndicators.Add(newIndicator);
        return newIndicator;
    }
}
