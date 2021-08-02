using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using KinematicCharacterController;

public class PlayerRespawnManager : MonoBehaviour
{

    /// Static Instance Stuff ///
    private static PlayerRespawnManager s_instance = null;
    public static PlayerRespawnManager Instance
    {
        get
        {
            if (!s_instance)
            {
                s_instance = FindObjectOfType<PlayerRespawnManager>();
            }
            return s_instance;
        }
    }

    // Variables that we're just gonna need a lot
    private PlayerController m_playerController = null;

    private float m_lastRespawnTime = 0f;

    // Variables for the safe respawn
    private Vector3 m_safeRespawnPoint = Vector3.zero;

    // Variables for the near respawn
    private Vector3 m_nearRespawnPoint = Vector3.zero;
    private float m_lastNearRespawnTime = 0f;
    private float m_timeToNextNearRespawnSet = 0f;

    private const float NEAR_RESPAWN_SET_INTERVAL = 0.5f;
    private const float NEAR_RESPAWN_MIN_TIME = 0.1f;

    // Public Unity Events
    public static UnityEvent<Vector3> OnSafeRespawnUpdate = new UnityEvent<Vector3>();
    public static UnityEvent OnRespawn = new UnityEvent();


    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Awake()
    {
        
    }

    private void Start()
    {
        m_playerController = OverworldManager.Instance.PlayerController;
    }

    private void Update()
    {
        m_timeToNextNearRespawnSet -= Time.deltaTime;
        if (m_timeToNextNearRespawnSet < 0f)
        {
            m_timeToNextNearRespawnSet = NEAR_RESPAWN_SET_INTERVAL;
            CheckForSafeRespawn();
        }
    }

    ////////////////////////////////////////////////////
    /// Public Functions to be called by other stuff ///
    ////////////////////////////////////////////////////
    public void SetSafeRespawn(Vector3 newPosition)
    {
        SetNearRespawn(newPosition);
        m_safeRespawnPoint = newPosition;
        // TODO: Maybe a small indicator?

        OnSafeRespawnUpdate.Invoke(newPosition);
    }

    public void RespawnSafe(int damageTaken = 0)
    {
        RespawnInternal(m_safeRespawnPoint, damageTaken);
    }

    public void SetNearRespawn(Vector3 newPosition)
    {
        m_nearRespawnPoint = newPosition;

        m_timeToNextNearRespawnSet = NEAR_RESPAWN_SET_INTERVAL;
    }

    public void RespawnNear(int damageTaken = 0)
    {
        // CHECK IF THIS RESPAWN IS SAFE.
        // TODO: Do other checks to make sure this is safe? For now, just checking how long it's been since we last tried to do the near respawn. If it's too soon, assume that this one is bad and just default to the safe respawn.
        // the safe respawn again.
        if (m_lastNearRespawnTime >= (Time.time - NEAR_RESPAWN_MIN_TIME) || m_lastRespawnTime >= (Time.time - NEAR_RESPAWN_MIN_TIME))
        {
            SetNearRespawn(m_safeRespawnPoint);
            RespawnSafe(damageTaken);
            return;
        }

        RespawnInternal(m_nearRespawnPoint, damageTaken);
        m_lastNearRespawnTime= Time.time;
    }


    ////////////////////////////////
    /// Private Helper Functions ///
    ////////////////////////////////
    private void CheckForSafeRespawn()
    {
        // Start with easy outs.
        // If we are loading or paused, don't bother.
        if (PauseMenu.Instance.Paused || LoadingManager.Loading)
        {
            return;
        }

        // If we respawned recently, then don't set a new one yet.
        if (m_lastRespawnTime >= Time.time - NEAR_RESPAWN_SET_INTERVAL)
        {
            return;
        }

        // If we are in the air, then no!
        if (!m_playerController.Motor.GroundingStatus.IsStableOnGround)
        {
            return;
        }

        // If we've made it this far, we should be good to go.
        // TODO: Do something to check the general area for certain no-nos, like spikes? IDK, we'll see!
        SetNearRespawn(m_playerController.transform.position);
    }

    private void RespawnInternal(Vector3 respawnPoint, int damageTaken)
    {
        if (damageTaken >= 0)
        {
            DataManager.Instance.GetPlayerBattleInstanceData().Damage(damageTaken);
        }
        PlayerStateHurt hurtState = new PlayerStateHurt();
        hurtState.TargetPosition = respawnPoint;
        m_playerController.SetState(hurtState);

        m_lastRespawnTime = Time.time;

        OnRespawn.Invoke();
    }
}
