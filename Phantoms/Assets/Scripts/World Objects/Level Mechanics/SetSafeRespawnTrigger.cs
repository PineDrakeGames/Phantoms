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

    private bool m_isCurrentCheckpoint = false;
    private bool m_triggered = false;


    // Unity Functions
    private void Awake() 
    {
        if (m_respawnPoint == null) { m_respawnPoint = transform; }
        PlayerRespawnManager.OnSafeRespawnUpdate.AddListener(OnSafeRespawnUpdate);
    }

    private void OnDestroy()
    {
        PlayerRespawnManager.OnSafeRespawnUpdate.RemoveListener(OnSafeRespawnUpdate);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            TrySetRespawn();
        }
    }

    // Public Functions 
    public void TrySetRespawn()
    {
        if (m_isCurrentCheckpoint) { return; }
        if (m_triggered && m_triggerOnlyOnce) { return; }

        PlayerRespawnManager.Instance.SetSafeRespawn(m_respawnPoint.position);

        if (m_triggerOnlyOnce) { m_triggered = true; }
    }

    // private functions
    private void OnSafeRespawnUpdate(Vector3 newPosition)
    {
        if (Vector3.Distance(m_respawnPoint.transform.position, newPosition) < 0.1f)
        {
            m_isCurrentCheckpoint = true;
            if (m_triggerOnlyOnce) { m_triggered = true; }
        }
        else
        {
            m_isCurrentCheckpoint = false;
        }
    }
}
