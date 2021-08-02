using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSafeRespawnTrigger : MonoBehaviour
{
    // Serialize Fields
    [SerializeField]
    private Transform m_respawnPoint = null;

    [Header("Options")]
    [SerializeField]
    private bool m_requireTouchingGround = true;

    [SerializeField]
    private bool m_triggerOnlyOnce = false;


    // Private non-serialized stuff
    private const string PLAYER_TAG = "Player";


    // Unity Functions
    private void Awake() 
    {
        if (m_respawnPoint == null) { m_respawnPoint = transform; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            TrySetRespawn();
        }
    }

    public void TrySetRespawn()
    {

    }
}
