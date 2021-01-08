using UnityEngine;

public abstract class AbilityMinigame : MonoBehaviour
{
    /// Enums, used to indicate the state and result of the minigame. ///
    public enum MinigameState
    {
        INACTIVE,
        INITIALIZING,
        RUNNING,
        FINISHED
    }
    public enum MinigameResult
    {
        FAIL,
        SUCCESS,
        PERFECT
    }
    protected MinigameState m_state = MinigameState.INACTIVE;
    public MinigameState State
    {
        get { return m_state; }
    }
    protected MinigameResult m_result = MinigameResult.SUCCESS;
    public MinigameResult Result
    {
        get { return m_result; }
    }

    /// Editor fields for general items to all minigames ///
    [Header("General Serialize fields")]
    [SerializeField]
    protected bool m_startOnEnable = false;


    protected void Awake()
    {
        m_state = MinigameState.INACTIVE;
        m_result = MinigameResult.SUCCESS; // Not sure what default should be?
    }

    protected void Update()
    {
        switch (m_state)
        {
            case MinigameState.INACTIVE:
                break;
            case MinigameState.INITIALIZING:
                InitializingState();
                break;
            case MinigameState.RUNNING:
                RunningState();
                break;
            case MinigameState.FINISHED:
                FinishedState();
                break;
        }
    }

    public void StartMinigame()
    {
        m_state = MinigameState.INITIALIZING;
        Restart();
    }

    public void ResetMinigame()
    {
        m_state = MinigameState.INACTIVE;
    }

    protected abstract void Restart();

    /// For any intro parts to the minigame - usually animations to bring it in?
    protected abstract void InitializingState();

    protected abstract void RunningState();

    protected abstract void FinishedState();
}