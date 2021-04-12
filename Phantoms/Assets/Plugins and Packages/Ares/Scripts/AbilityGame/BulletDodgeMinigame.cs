using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletDodgeMinigame : AbilityMinigame
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


    /// Private variables

    protected override void Restart()
    {

    }

    protected override void InitializingState()
    {
        m_state = MinigameState.RUNNING;
    }

    protected override void RunningState()
    {

    }

    protected override void FinishedState()
    {

    }
}
