using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputIndicator : MonoBehaviour
{
    
    public enum IndicatorState
    {
        DISABLED,
        WAITING,
        READY_FOR_INPUT,
        INPUT_RECIEVED,
        FAILED
    }

    private IndicatorState m_currentState = IndicatorState.DISABLED;
    public IndicatorState CurrentState
    {
        get { return m_currentState; }
        set { SetState(value); }
    }

    [SerializeField]
    private Image m_indicatorImage = null;

    public void SetIndicator(MinigameInput inputType)
    {
        // Set the image based on the button?
    }

    public void SetState(IndicatorState newState)
    {
        /// For now, just set the color based on the indicator state.
        switch (newState)
        {
            case IndicatorState.DISABLED:
                m_indicatorImage.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
                break;
            case IndicatorState.WAITING:
                m_indicatorImage.color = new Color(0.5f, 0.5f, 0.5f, 0.75f);
                break;
            case IndicatorState.READY_FOR_INPUT:
                m_indicatorImage.color = new Color(1f, 1f, 1f, 1f);
                break;
            case IndicatorState.INPUT_RECIEVED:
                m_indicatorImage.color = new Color(0f, 1f, 0f, 1f);
                break;
            case IndicatorState.FAILED:
                m_indicatorImage.color = new Color(1f, 0f, 0f, 1f);
                break;
        }

        m_currentState = newState;
    }
}
