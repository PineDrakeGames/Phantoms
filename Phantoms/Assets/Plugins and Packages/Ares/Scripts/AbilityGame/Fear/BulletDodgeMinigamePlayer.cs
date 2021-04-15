using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDodgeMinigamePlayer : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_playerTransform = null;

    [SerializeField]
    private Rigidbody m_playerRigidbody = null;

    [SerializeField]
    private RectTransform m_playerArea = null;

    [SerializeField]
    private float m_playerAreaSpacing = 10f;

    public bool AllowMovement = false;

    public float PlayerSpeed = 0f;

    private float widthRadius = 0f;
    private float heightRadius = 0f;

    private void Awake()
    {
        widthRadius = (m_playerTransform.rect.width / 2f) + m_playerAreaSpacing;
        heightRadius = (m_playerTransform.rect.width / 2f) + m_playerAreaSpacing;
    }

    private void Update() 
    {
        if (AllowMovement)
        {
            PlayerMovement();
        }
    }

    public void ResetPosition()
    {
        // for now, just assume we want to recenter
        m_playerTransform.anchoredPosition = Vector2.zero;
    }

    public void PlayerMovement()
    {
        // TODO: Make input stuff generic here
        float verticalMove = Input.GetAxisRaw("Vertical");
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(horizontalMove, verticalMove), 1f);

        Vector3 moveVector = moveInputVector * PlayerSpeed * Time.deltaTime;

        m_playerTransform.Translate(moveVector);

        float xPos = Mathf.Clamp(m_playerTransform.anchoredPosition.x, m_playerArea.rect.xMin + widthRadius, m_playerArea.rect.xMax - widthRadius);
        float yPos = Mathf.Clamp(m_playerTransform.anchoredPosition.y, m_playerArea.rect.yMin + heightRadius, m_playerArea.rect.yMax - heightRadius);

        m_playerTransform.anchoredPosition = new Vector2(xPos, yPos);
    }
}
