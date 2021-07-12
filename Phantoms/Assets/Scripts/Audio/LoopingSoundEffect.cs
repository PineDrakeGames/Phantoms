using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingSoundEffect : MonoBehaviour
{
    [SerializeField]
    private AudioClip m_defaultClip = null;


    private AudioSource m_aud;

    private AudioClip m_currentClip = null;

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    void Awake()
    {
        m_aud = gameObject.AddComponent<AudioSource>();
        m_aud.loop = true;
        m_aud.playOnAwake = false;
        Reset();
        SetClip(m_defaultClip);
    }

    ////////////////////////
    /// Public Functions ///
    ////////////////////////

    // A lot of these are just calling audio source things directly, but basically we want all looping sound effects to go through this script - so we can change how they all work later easier.

    // Just in case we want to set things back to blank.
    public void Reset()
    {
        SetClip(null);
        SetVolume(1f);
        SetPitch(1f);
    }

    public void SetClip(AudioClip clip)
    {
        if (m_currentClip != clip)
        {
            if (m_aud.isPlaying) { m_aud.Stop(); }
            m_currentClip = clip;
            m_aud.clip = clip;
        }
    }

    public void Play(AudioClip clip = null)
    {
        if (clip)
        {
            SetClip(clip);
            m_aud.Play();
        }
        else
        {
            if (m_currentClip != null && !m_aud.isPlaying)
            {
                m_aud.Play();
            }
        }
    }

    public void Pause()
    {
        if (m_aud.isPlaying)
        {
            m_aud.Pause();
        }
    }

    public void Stop()
    {
        if (m_aud.isPlaying)
        {
            m_aud.Stop();
        }
    }

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        m_aud.volume = volume * DataManager.Instance.GetSoundVolume();
    }

    public void SetPitch(float pitch)
    {
        pitch = Mathf.Clamp(pitch, -3f, 3f);
        m_aud.pitch = pitch;
    }
}
