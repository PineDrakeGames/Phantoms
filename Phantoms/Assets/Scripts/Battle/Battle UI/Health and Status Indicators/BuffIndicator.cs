using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffIndicator : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem m_starsParticles = null;

    [SerializeField]
    private ParticleSystem m_lightsParticles = null;

    private bool m_readyToUse = true;
    public bool Ready { get { return m_readyToUse; } }

    private const float BUFF_INDICATOR_DURATION = 1f;
    private float m_currentTime = 0f;

    private void Update() 
    {
        if (!m_readyToUse)
        {
            m_currentTime -= Time.deltaTime;
            if (m_currentTime <= 0)
            {
                EndBuffEffect();
                m_readyToUse = true;
            }
        }
    }

    public void SetBuffIndicator(Vector3 position, Ares.Stat stat, int stages)
    {
        transform.position = position;
        StartBuffEffect();

        m_readyToUse = false;
        m_currentTime = BUFF_INDICATOR_DURATION;
    }

    private void StartBuffEffect()
    {
        ParticleSystem.EmissionModule starsEmission = m_starsParticles.emission;
        starsEmission.enabled = true;
        ParticleSystem.EmissionModule lightsEmission = m_lightsParticles.emission;
        lightsEmission.enabled = true;
    }

    private void EndBuffEffect()
    {
        ParticleSystem.EmissionModule starsEmission = m_starsParticles.emission;
        starsEmission.enabled = false;
        ParticleSystem.EmissionModule lightsEmission = m_lightsParticles.emission;
        lightsEmission.enabled = false;
    }
}
