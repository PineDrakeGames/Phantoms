using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMinigame : AbilityMinigame
{
    [Header("UI References")]
    [SerializeField]
    private Animator m_minigameAnim = null;

    [Header("Minigame Variables")]
    [SerializeField]
    private float m_revealTime = 0.8f;

    [SerializeField]
    private float m_maxErrorTime = 0.3f;

    [SerializeField]
    private float m_minErrorTime = 0.05f;

    [SerializeField]
    private int m_numSteps = 3;


    private float m_currentTime = 0f;
    private AnimatorStateInfo m_animatorState;
    private float m_timePerStep = 0f;
    private int m_currentStep = 0;

    private void Start()
    {
        m_timePerStep = m_revealTime / (float)(m_numSteps - 1);
    }

    protected override void InitializingState()
    {
        m_animatorState = m_minigameAnim.GetCurrentAnimatorStateInfo(0);
        if (m_animatorState.IsName("Default"))
        {
            m_minigameAnim.SetTrigger("Start");
        }
        else if (m_animatorState.IsName("Enter") && m_animatorState.normalizedTime >= 0.95)
        {
            m_state = MinigameState.RUNNING;
        }
    }

    protected override void RunningState()
    {
        if (m_currentStep == 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_currentStep += 1;
                m_minigameAnim.SetInteger("Step", m_currentStep);
                m_currentTime = 0f;
            }
            return;
        }

        float prevTime = m_currentTime;
        m_currentTime += Time.deltaTime;

        if (m_currentTime >= (m_timePerStep * (float)m_currentStep))
        {
            m_currentStep += 1;
            m_minigameAnim.SetInteger("Step", m_currentStep);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if ((m_currentTime < m_revealTime) || (m_currentTime > (m_revealTime + m_maxErrorTime)))
            {
                m_result = MinigameResult.FAIL;
            }
            // If either we are within the first reveal time or this is the first frame of the reveal, it's perfect
            else if ((m_currentTime <= (m_revealTime + m_minErrorTime)) || (prevTime < m_revealTime))
            {
                m_result = MinigameResult.PERFECT;
            }
            else
            {
                m_result = MinigameResult.SUCCESS;
            }

            Debug.Log("Minigame Finished with result: " + m_result + "\nRelease Time: " + m_currentTime);
            m_state = MinigameState.FINISHED;
        }
    }

    protected override void FinishedState()
    {

    }
}
