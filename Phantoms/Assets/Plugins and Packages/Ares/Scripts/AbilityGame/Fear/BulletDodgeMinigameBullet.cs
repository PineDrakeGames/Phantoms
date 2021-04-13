using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDodgeMinigameBullet : MonoBehaviour
{
    public BulletDodgeMinigame m_minigame = null;

    public enum BulletMoveType
    {
        LINEAR,
        HOMING
    }

    private RectTransform m_bulletTransform = null;
    private BulletMoveType m_moveType;

    private Vector2 m_currentDirection = Vector2.zero;
    private float m_currentSpeed = 0f;
    private RectTransform m_target = null;

    /////////////////////////////////////////
    /// Public Functions for the movement ///
    /////////////////////////////////////////
    public void SetLinearMovemet(Vector2 direction, float speed)
    {
        m_currentDirection = direction.normalized;
        m_currentSpeed = speed;
        m_moveType = BulletMoveType.LINEAR;
    }

    public void SetHomingMovement(Vector2 startDirection, float startSpeed, RectTransform target)
    {
        m_currentDirection = startDirection.normalized;
        m_currentSpeed = startSpeed;
        m_target = target;
        m_moveType = BulletMoveType.HOMING;
    }


    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Awake()
    {
        m_target = GetComponent<RectTransform>();    
    }

    private void Update()
    {
        Vector2 moveVector = Vector2.zero;
        switch(m_moveType)
        {
            case BulletMoveType.LINEAR:
                moveVector = LinearMovement();
                break;
            case BulletMoveType.HOMING:
                moveVector = HomingMovement();
                break;
        }

        m_target.Translate(moveVector);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BulletDodgeMinigamePlayer>() != null)
        {
            // Hit Player
            if (m_minigame)
            {
                m_minigame.HitPlayer(this);
            }
            Destroy(this.gameObject);
        }
    }

    ///////////////////////////////////////////////////////////////
    /// Private functions for the different bullet moving types ///
    ///////////////////////////////////////////////////////////////
    private Vector2 LinearMovement()
    {
        Vector2 moveVector = m_currentDirection.normalized * m_currentSpeed * Time.deltaTime;
        return moveVector;
    }

    private Vector2 HomingMovement()
    {
        Vector2 moveVector = Vector2.zero;

        // TODO

        return moveVector;
    }
}
