using UnityEngine;

public abstract class AbilityMinigame : MonoBehaviour
{
    /// Enums, used to indicate the state and result of the minigame. ///
    public enum MinigameState
    {
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
    protected MinigameState m_state = MinigameState.INITIALIZING;
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
        m_state = MinigameState.INITIALIZING;
        m_result = MinigameResult.SUCCESS; // Not sure what default should be?
    }

    protected void Update()
    {
        switch (m_state)
        {
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

    protected abstract void InitializingState();

    protected abstract void RunningState();

    protected abstract void FinishedState();
}
