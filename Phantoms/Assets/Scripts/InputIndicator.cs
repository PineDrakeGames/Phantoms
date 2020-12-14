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

    public void SetIndicator(MinigameInput inputType)
    {
        // Set the image based on the button?
    }

    public void SetState(IndicatorState newState)
    {


        m_currentState = newState;
    }
}
