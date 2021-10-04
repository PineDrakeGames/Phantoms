using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldPhantomEnemy : MonoBehaviour
{
    [Header("Data Stuff")]
    [Scene]
    [SerializeField]
    private string m_battleScene = null;
    [SerializeField]
    private EnemyEncounterData m_encounterData = null;

    [Header("Trigger Settings")]
    [SerializeField]
    private float m_triggerDistance = 9f;
    [SerializeField]
    private float m_chaseDistance = 11f;
    [SerializeField]
    private LayerMask m_LineOfSightBlockers;
    [SerializeField]
    private float m_chaseDelay = 0.3f;
    [SerializeField]
    private float m_lingerTimeAfterChase = 1.5f;
    [SerializeField]
    private float m_approachSpeed = 5f;
    [SerializeField]
    private float m_turnRate = 360f;
    [SerializeField]
    private float m_returnSpeed = 1f;


    private enum OverworldEnemyState
    {
        INACTIVE,
        IDLE,
        CHASING,
        RETURNING
    }

    private OverworldEnemyState m_state = OverworldEnemyState.IDLE;
    private Vector3 m_startPosition = Vector3.zero;
    private Transform m_player = null;

    private float m_lastStateChangeTime = 0f;
    private Vector3 m_currentVelocity = Vector3.zero;

    private RaycastHit m_raycastHit;
    private const string PLAYER_TAG = "Player";

    private bool m_alreadyTriggered = false;

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////

    private void Start()
    {
        // TODO: Make an empty object on the player, like 'player center' or 'player face' or something
        m_player = OverworldManager.Instance.PlayerController.PlayerCenter;
        m_startPosition = transform.position;
    }

    private void OnEnable()
    {
        if (m_alreadyTriggered)
        {
            SetState(OverworldEnemyState.INACTIVE);
            this.gameObject.SetActive(false);
        }
        else
        {
            SetState(OverworldEnemyState.IDLE);
        }
    }

    void Update()
    {
        bool playerDetected = PlayerDetected();

        // Doing a series of If statements incase the state changes as we go through them - so the change happens this frame.
        if (m_state == OverworldEnemyState.IDLE)
        {
            if (playerDetected)
            {
                // Detected the player - start chasing
                SetState(OverworldEnemyState.CHASING);
            }
            else
            {
                // Just idle around
            }
        }

        if (m_state == OverworldEnemyState.CHASING)
        {
            if (!playerDetected)
            {
                // Lost the player - start returning
                SetState(OverworldEnemyState.RETURNING);
            }
            else
            {
                if (Time.time - m_lastStateChangeTime >= m_chaseDelay)
                {
                    Chase(m_player.position);
                }
            }
        }

        if (m_state == OverworldEnemyState.RETURNING)
        {
            if (playerDetected)
            {
                // Detected the player again - start chasing again
                SetState(OverworldEnemyState.CHASING);
            }
            else
            {
                if (Time.time - m_lastStateChangeTime >= m_lingerTimeAfterChase)
                {
                    if (Wander(m_startPosition))
                    {
                        SetState(OverworldEnemyState.IDLE);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && m_encounterData != null)
        {
            StartBattle();
        }
    }

    #if UNITY_EDITOR
 void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_chaseDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_triggerDistance);
    }
    #endif

    //////////////////////////////////////
    /// Private helper function stuff! ///
    //////////////////////////////////////

    private bool PlayerDetected()
    {
        float checkDistance = m_triggerDistance;
        if (m_state == OverworldEnemyState.CHASING) { checkDistance = m_chaseDistance; }

        // Check if the player is close enough to where the phantom started
        if (Vector3.Distance(m_player.position, m_startPosition) <= checkDistance)
        {
            // Try and raycast towards the player. if we hit nothing, then the player is too far.
#if UNITY_EDITOR
            Debug.DrawRay(transform.position, m_player.position - transform.position, Color.red);
#endif
            if (Physics.Raycast(transform.position, m_player.position - transform.position, out m_raycastHit, checkDistance * 2f, m_LineOfSightBlockers))
            {
                Debug.Log(m_raycastHit.collider.name);
                // If we hit something, check if it's the player. If not, something is blocking line of sight.
                if (m_raycastHit.collider.CompareTag(PLAYER_TAG))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void SetState(OverworldEnemyState newState)
    {
        m_lastStateChangeTime = Time.time;
        m_currentVelocity = Vector3.zero;
        m_state = newState;
    }

    private void Chase(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position);

        if (m_currentVelocity == Vector3.zero)
        {
            m_currentVelocity = (direction.normalized * m_approachSpeed);
        }
        else
        {
            float turnRate = m_turnRate * Mathf.Deg2Rad * Time.deltaTime;
            m_currentVelocity = Vector3.RotateTowards(m_currentVelocity, direction.normalized, turnRate, 0f);
        }


        transform.Translate(m_currentVelocity * Time.deltaTime);
    }

    // Returns true if the destination has been reached
    private bool Wander(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position);

        Vector3 moveVector = direction.normalized * m_returnSpeed * Time.deltaTime;

        if (moveVector.magnitude >= direction.magnitude)
        {
            transform.position = targetPosition;
            return true;
        }
        else
        {
            transform.Translate(moveVector);
            return false;
        }
    }

    private void StartBattle()
    {
        BattleStartManager.Enemies = m_encounterData;
        m_alreadyTriggered = true;
        SetState(OverworldEnemyState.INACTIVE);
        LoadingManager.LoadBattle(m_battleScene);
    }
}
