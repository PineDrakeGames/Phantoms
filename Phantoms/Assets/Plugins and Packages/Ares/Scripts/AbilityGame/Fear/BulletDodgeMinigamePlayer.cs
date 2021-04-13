using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDodgeMinigamePlayer : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_playerTransform = null;

    [SerializeField]
    private Rigidbody m_playerRigidbody = null;

    public bool AllowMovement = false;

    public float PlayerSpeed = 0f;

    private float widthRadius = 0f;
    private float heightRadius = 0f;

    private void Awake()
    {
        widthRadius = m_playerTransform.rect.width / 2f;
        heightRadius = m_playerTransform.rect.width / 2f;
    }

    private void Update() 
    {
        if (AllowMovement)
        {
            PlayerMovement();
        }
    }

    public void PlayerMovement()
    {
        // TODO: Make input stuff generic here
        float verticalMove = Input.GetAxisRaw("Vertical");
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(horizontalMove, verticalMove), 1f);

        Vector3 moveVector = moveInputVector * PlayerSpeed * Time.deltaTime;

        // Raycasting in the directions we are going to make sure we can go there.
        RaycastHit hit;
        float distance = 0f;
        Vector3 direction = Vector3.right;

        // First check horizontal movement
        if (moveVector.x < 0)
        {
            direction = Vector3.left;
        }
        else
        {
            direction = Vector3.right;
        }
        
        distance = Mathf.Abs(moveVector.x) + widthRadius;

        if (Physics.Raycast(m_playerRigidbody.position, direction, out hit, distance))
        {
            moveVector.x = (direction.x * hit.distance) - (direction.x * widthRadius);
        }

        // then check vertical movement.
        if (moveVector.y < 0)
        {
            direction = Vector3.down;
        }
        else
        {
            direction = Vector3.up;
        }
        distance = Mathf.Abs(moveVector.y) + heightRadius;
        if (Physics.Raycast(m_playerRigidbody.position, direction, out hit, distance))
        {
            moveVector.y = (direction.y * hit.distance) - (direction.y * heightRadius);
        }

        m_playerRigidbody.MovePosition(m_playerRigidbody.position + moveVector);
    }
}
