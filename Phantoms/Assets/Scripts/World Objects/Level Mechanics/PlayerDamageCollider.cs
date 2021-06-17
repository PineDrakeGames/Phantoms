using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageCollider : MonoBehaviour
{
    [SerializeField]
    private int m_damageAmount = 1;
    [SerializeField]
    private bool m_safeRespawn = true;

    private const string PLAYER_TAG = "Player";

    public void HurtPlayer()
    {
        // If the player is already in the hurt state, then no worries - skip it
        if (OverworldManager.Instance.PlayerController.CurrentState is PlayerStateHurt) { return; }

        if (m_safeRespawn)
        {
            PlayerRespawnManager.Instance.RespawnSafe(m_damageAmount);
        }
        else
        {
            PlayerRespawnManager.Instance.RespawnNear(m_damageAmount);
        }
    }

    // Setting this up to work with either triggers or colliders, in case we want a solid wall the player can't go through and is hurt by.
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(PLAYER_TAG))
        {
            HurtPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            HurtPlayer();
        }
    }
}
