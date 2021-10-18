using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPickup : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The amount that the player collects when they get this drop (most will be 1, but can be larger for bigger ones)")]
    private int m_dropAmount = 1;

    [SerializeField]
    [MinMaxRange(-3f, 3f)]
    private RangedFloat m_dropPitch = new RangedFloat(0.7f, 1.3f);
    [SerializeField]
    private float m_dropVolume = 0.3f;
    [SerializeField]
    private AudioClip m_dropPickupSoundEffect;

    [Header("Collection animation stuff")]
    [SerializeField]
    private float m_dropCollectDistance = 0.5f;

    [SerializeField]
    private float m_acceleration = 10f;

    [SerializeField]
    private float m_maxSpeed = 30f;


    private bool m_triggered = false;
    private float m_currentSpeed = 0f;
    private Transform m_player = null;
    
    private void Update() {
        if (m_triggered)
        {
            if (m_currentSpeed < m_maxSpeed)
            {
                m_currentSpeed += Time.deltaTime * m_acceleration;
                m_currentSpeed = Mathf.Clamp(m_currentSpeed, 0f, m_maxSpeed);
            }

            Vector3 direction = (m_player.position - transform.position).normalized;

            transform.Translate(direction * m_currentSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, m_player.position) <= m_dropCollectDistance)
            {
                Collect();
                m_triggered = false;
            }
        }
    }

    // Called when the drop should be picked up
    public void Collect()
    {
        DataManager.CurrentDrops += m_dropAmount;

        AudioManager.PlaySound(m_dropPickupSoundEffect, m_dropVolume, Random.Range(m_dropPitch.minValue, m_dropPitch.maxValue));

        // TODO: Play an animation instead
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player" && collider.GetComponent<PlayerController>())
        {
            m_triggered = true;
            // TODO: Get reference to this
            m_player = collider.GetComponent<PlayerController>().PlayerCenter;
        }
    }
}
