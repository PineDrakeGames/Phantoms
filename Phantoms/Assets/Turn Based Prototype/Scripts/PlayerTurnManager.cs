using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnManager : MonoBehaviour
{
    [SerializeField]
    private BattleManager m_battleManager = null;


    private enum PlayerTurnState
    {
        DEFAULT,
        BEGINNING,
        PRE_TURN,
        TURN,
        POST_TURN,
        END
    }
    private PlayerTurnState m_playerState = PlayerTurnState.DEFAULT;

    private bool m_isTurnComplete = false;

    public void StartTurn()
    {
        m_playerState = PlayerTurnState.BEGINNING;
        m_isTurnComplete = false;
    }

    void Update()
    {
        switch (m_playerState)
        {
            case PlayerTurnState.BEGINNING:
                // Initialize anything needed for the player's turn.
                m_playerState = PlayerTurnState.PRE_TURN;
                break;
            case PlayerTurnState.PRE_TURN:
                // Tick any effects that should happen at the start of the player's turn
                m_playerState = PlayerTurnState.TURN;
                break;
            case PlayerTurnState.TURN:
                // Handle the player's turn
                if (m_isTurnComplete)
                {
                    m_playerState = PlayerTurnState.POST_TURN;
                }
                break;
            case PlayerTurnState.POST_TURN:
                // Tick any effects that should happen at the end of the player's turn
                m_battleManager.UpdateUI();
                m_battleManager.m_playerTurnComplete = true;
                m_playerState = PlayerTurnState.END;
                break;
            case PlayerTurnState.END:
                // Used to signify that the player's turn is over - nothing left to do.
                break;
            default:
                // Do nothing
                break;
        }
    }

    public void Damage(int amount)
    {
        if (m_playerState == PlayerTurnState.TURN)
        m_battleManager.Enemy.Damage(amount);
        m_isTurnComplete = true;
    }

    public void Heal(int amount)
    {
        if (m_playerState == PlayerTurnState.TURN)
        m_battleManager.Player.Heal(amount);
        m_isTurnComplete = true;
    }
}
