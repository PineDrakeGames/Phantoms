using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDodgeMinigamePlayer : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_playerTransform = null;

    public bool AllowMovement = false;

    public float PlayerSpeed = 0f;

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
        Vector2 moveInputVector = Vector2.ClampMagnitude(new Vector2(horizontalMove, verticalMove), 1f);

        Vector2 moveVector = moveInputVector * PlayerSpeed * Time.deltaTime;
        m_playerTransform.Translate(moveVector);
    }
}
