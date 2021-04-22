using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoveMinigameBullet : MonoBehaviour
{
    public enum BulletMoveType
    {
        LINEAR,
        GRAVITY
    }

    private RectTransform m_bulletTransform = null;
    private BulletMoveType m_moveType = BulletMoveType.LINEAR;

    private Vector2 m_currentDirection = Vector2.zero;
    private float m_currentSpeed = 0f;
    private float m_remainingLifeTime = 10f;
    private float m_gravity = 0f;

    private const float DEFAULT_SPEED = 500f;
    private const float DEFAULT_LIFETIME = 4f;

    /////////////////////////////////////////
    /// Public Functions to set up bullet ///
    /////////////////////////////////////////
    public void SetPosition(Vector2 position)
    {
        m_bulletTransform.anchoredPosition = position;
    }

    public void SetMovement(Vector2 direction, BulletMoveType moveType = BulletMoveType.LINEAR, float speed = DEFAULT_SPEED, float lifetime = DEFAULT_LIFETIME, float gravity = 0f)
    {
        m_currentDirection = direction.normalized;
        m_currentSpeed = speed;
        m_remainingLifeTime = lifetime;
        m_moveType = moveType;
        m_gravity = gravity;
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
            case BulletMoveType.GRAVITY:
                moveVector = GravityMovement();
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
        LoveMinigameTarget target  = other.GetComponent<LoveMinigameTarget>();
        if (target)
        {
            target.Hit();
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

    private Vector2 GravityMovement()
    {
        Vector2 moveVector = m_currentDirection.normalized * m_currentSpeed;
        moveVector += (Vector2.down * m_gravity * Time.deltaTime);
        m_currentDirection = moveVector.normalized;
        m_currentSpeed = moveVector.magnitude;

        return moveVector * Time.deltaTime;
    }
}
