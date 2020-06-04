using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    START,
    PLAYERTURN,
    ENEMYTURN,
    WIN,
    LOSE
}

public class BattleManager : MonoBehaviour
{
    [SerializeField]
    private BattleUI m_battleUI = null;

    [SerializeField]
    private PlayerTurnManager m_playerTurnManager = null;

    /// Serialize fields ///
    [SerializeField]
    private BattleCharacter m_player = null;
    public BattleCharacter Player
    {
        get { return m_player; }
    }

    [SerializeField]
    private BattleCharacter m_enemy = null;
    public BattleCharacter Enemy
    {
        get { return m_enemy; }
    }

    /// Private Variables ///
    private BattleState m_state = BattleState.START;
    public BattleState State
    {
        get { return m_state; }
    }

    public bool m_playerTurnComplete = false;
    public bool m_enemyTurnComplete = false;


    /// Initialization ///
    private void Start()
    {
        // Set up UI
        m_battleUI.SetPlayerHealth(m_player.CurrentHealth, m_player.MaxHealth);
        m_battleUI.SetEnemyHealth(m_enemy.CurrentHealth, m_enemy.MaxHealth);

        SetState(BattleState.START);
    }

    private void Update()
    {
        switch (m_state)
        {
            case BattleState.START:
                // Wait until any initialization is complete - probably intro animation?
                // Once complete, set state to player turn and call any functions to initialize that.
                SetState(BattleState.PLAYERTURN);
                m_playerTurnComplete = false;
                break;
            case BattleState.PLAYERTURN:
                // Call to update player turn. Involves picking options, and executing attacks
                // Once player turn is done, set to enemy turn
                if (m_playerTurnComplete)
                {
                    m_battleUI.SetPlayerHealth(m_player.CurrentHealth, m_player.MaxHealth);
                    m_battleUI.SetEnemyHealth(m_enemy.CurrentHealth, m_enemy.MaxHealth);
                    if (!CheckForBattleComplete())
                    {
                        SetState(BattleState.ENEMYTURN);
                        m_enemyTurnComplete = false;
                        StartCoroutine(TestEnemyTurn());
                    }
                }
                break;
            case BattleState.ENEMYTURN:
                if (m_playerTurnComplete)
                {
                    if (!CheckForBattleComplete())
                    {
                        SetState(BattleState.PLAYERTURN);
                        m_playerTurnComplete = false;
                    }
                }
                break;
            case BattleState.WIN:
                break;
            case BattleState.LOSE:
                break;
        }
    }

    private void SetState(BattleState state)
    {
        switch (state)
        {
            case BattleState.START:
                break;
            case BattleState.PLAYERTURN:
                m_playerTurnManager.StartTurn();
                break;
            case BattleState.ENEMYTURN:
                break;
            case BattleState.WIN:
                break;
            case BattleState.LOSE:
                break;
        }

        m_state = state;
    }

    public void UpdateUI()
    {
        m_battleUI.SetPlayerHealth(m_player.CurrentHealth);
        m_battleUI.SetEnemyHealth(m_enemy.CurrentHealth);
    }

    public bool CheckForBattleComplete()
    {
        if (m_player.CurrentHealth <= 0)
        {
            SetState(BattleState.LOSE);
            return true;
        }
        else if (m_enemy.CurrentHealth <= 0)
        {
            SetState(BattleState.WIN);
            return true;
        }
        return false;
    }

    private IEnumerator TestEnemyTurn()
    {
        yield return new WaitForSeconds(1f);
        m_player.Damage(5);
        UpdateUI();
        m_enemyTurnComplete = true;
    }


}
