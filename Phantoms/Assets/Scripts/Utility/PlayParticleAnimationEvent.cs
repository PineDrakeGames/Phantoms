using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleAnimationEvent : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem m_particleSystem = null;

    public void PlayParticles()
    {
        if (m_particleSystem)
        {
            m_particleSystem.Play();
        }
    }
}
