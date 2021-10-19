using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffIndicator : MonoBehaviour
{
    [System.Serializable]
    private class BuffIndicatorData
    {
        public string Stat;
        public Material StarsColor;
        public Color LightsColor;
    }

    [SerializeField]
    private ParticleSystem m_starsParticles = null;

    [SerializeField]
    private ParticleSystem m_lightsParticles = null;

    [Header("Buff specifics")]
    [SerializeField]
    private Transform m_buffTransform = null;
    [SerializeField]
    private Transform m_debuffTransform = null;
    [SerializeField]
    private Transform m_starsTransform = null;

    [SerializeField]
    private List<BuffIndicatorData> m_buffsData = new List<BuffIndicatorData>();

    private bool m_readyToUse = true;
    public bool Ready { get { return m_readyToUse; } }

    private const float BUFF_INDICATOR_DURATION = 1f;
    private float m_currentTime = 0f;

    // Particle System parts
    ParticleSystem.EmissionModule m_starsParticlesEmission;
    ParticleSystemRenderer m_starsParticlesRenderer;
    Material m_starsParticlesMaterial = null;

    ParticleSystem.EmissionModule m_lightsParticlesEmission;
    ParticleSystem.MainModule m_lightsParticlesMain;

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Awake()
    {
        m_starsParticlesEmission = m_starsParticles.emission;
        m_starsParticlesRenderer = m_starsParticles.GetComponent<ParticleSystemRenderer>();
        m_starsParticlesMaterial = new Material(m_starsParticlesRenderer.material);
        m_starsParticlesRenderer.material = m_starsParticlesMaterial;

        m_lightsParticlesEmission = m_lightsParticles.emission;
        m_lightsParticlesMain = m_lightsParticles.main;
    }

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
        StartBuffEffect(stat, stages);

        m_readyToUse = false;
        m_currentTime = BUFF_INDICATOR_DURATION;
    }

    private void StartBuffEffect(Ares.Stat stat, int stages)
    {
        BuffIndicatorData data = GetStatData(stat);

        if (data != null)
        {
            m_starsParticlesRenderer.material = data.StarsColor;
            m_lightsParticlesMain.startColor = data.LightsColor;
        }
        else
        {
            m_starsParticlesRenderer.material = m_starsParticlesMaterial;
            m_lightsParticlesMain.startColor = Color.white;
        }

        Transform targetTransform = m_buffTransform;
        if (stages < 0) { targetTransform = m_debuffTransform; }
        m_starsTransform.position = targetTransform.position;
        m_starsTransform.rotation = targetTransform.rotation;

        m_starsParticlesEmission.enabled = true;
        m_lightsParticlesEmission.enabled = true;
    }

    private void EndBuffEffect()
    {
        m_starsParticlesEmission.enabled = false;
        m_lightsParticlesEmission.enabled = false;
    }

    private BuffIndicatorData GetStatData( Ares.Stat stat)
    {
        foreach(BuffIndicatorData buffData in m_buffsData)
        {
            if (buffData.Stat == stat.Data.name)
            {
                return buffData;
            }
        }

        return null;
    }
}
