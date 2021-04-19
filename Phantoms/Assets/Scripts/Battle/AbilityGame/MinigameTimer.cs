using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MinigameTimer : MonoBehaviour
{
    ////////////////////////
    /// Serialize Fields ///
    ////////////////////////
    [SerializeField]
    private GameObject m_timerParent = null;

    [SerializeField]
    private Animator m_timerAnimator = null;

    ////////////////////////
    /// Public Variables ///
    ////////////////////////
    public UnityEvent OnTimerFinish = new UnityEvent();

    public bool TimerActive { get { return m_timerActive; } }

    /////////////////////////
    /// Private Variables ///
    /////////////////////////
    private bool m_timerActive = false;

    private float m_totalTime = 0f;
    private float m_currentTime = 0f;

    private const string TIMER_PARAMETER = "Time";

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Awake() 
    {
        StopTimer();
    }

    private void Update()
    {
        if (m_timerActive)
        {
            m_currentTime -= Time.deltaTime;

            float timerProgress = 1f - Mathf.Clamp01(m_currentTime / m_totalTime);
            m_timerAnimator.SetFloat(TIMER_PARAMETER, timerProgress);

            if (m_currentTime <= 0f)
            {
                m_timerActive = false;
                if (m_timerParent)
                {
                    m_timerParent.SetActive(false);
                }
                OnTimerFinish.Invoke();
            }
        }
    }

    ////////////////////////
    /// Public Functions ///
    ////////////////////////
    public void StartTimer(float time)
    {
        m_currentTime = time;
        m_totalTime = time;
        m_timerActive = true;

        if (m_timerParent)
        {
            m_timerParent.SetActive(true);
        }
    }

    public void StopTimer()
    {
        m_timerActive = false;
        if (m_timerParent)
        {
            m_timerParent.SetActive(false);
        }
    }
}
