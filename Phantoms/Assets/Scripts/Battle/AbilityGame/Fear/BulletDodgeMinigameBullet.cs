using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDodgeMinigameBullet : MonoBehaviour
{
    public BulletDodgeMinigame BulletMinigame = null;

    public enum BulletMoveType
    {
        LINEAR,
        HOMING
    }

    private RectTransform m_bulletTransform = null;
    private BulletMoveType m_moveType;

    private Vector2 m_currentDirection = Vector2.zero;
    private float m_currentSpeed = 0f;
    private float m_remainingLifeTime = 10f;
    private RectTransform m_target = null;

    private const float DEFAULT_SPEED = 500f;
    private const float DEFAULT_LIFETIME = 4f;

    /////////////////////////////////////////
    /// Public Functions for the movement ///
    /////////////////////////////////////////
    public void SetPosition(Vector2 position)
    {
        m_bulletTransform.anchoredPosition = position;
    }

    public void SetLinearMovement(Vector2 direction, float speed = DEFAULT_SPEED, float lifetime = DEFAULT_LIFETIME)
    {
        m_currentDirection = direction.normalized;
        m_currentSpeed = speed;
        m_remainingLifeTime = lifetime;
        m_moveType = BulletMoveType.LINEAR;
    }

    public void SetHomingMovement(Vector2 startDirection, float startSpeed, RectTransform target, float lifetime = DEFAULT_LIFETIME)
    {
        m_currentDirection = startDirection.normalized;
        m_currentSpeed = startSpeed;
        m_remainingLifeTime = lifetime;
        m_target = target;
        m_moveType = BulletMoveType.HOMING;
    }


    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Awake()
    {
        m_bulletTransform = GetComponent<RectTransform>();    
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

        m_bulletTransform.Translate(moveVector);

        m_remainingLifeTime -= Time.deltaTime;
        if (m_remainingLifeTime <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BulletDodgeMinigamePlayer>() != null)
        {
            // Hit Player
            if (BulletMinigame)
            {
                BulletMinigame.HitPlayer(this);
            }
            this.gameObject.SetActive(false);
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
