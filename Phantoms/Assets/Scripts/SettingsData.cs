using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    private float m_masterVolume = 0.7f;
    private float m_soundsVolume = 0.5f;
    private float m_musicVolume = 0.3f;

    public float MasterVolume
    {
        get { return m_masterVolume; }
        set { m_masterVolume = Mathf.Clamp01(value); }
    }

    public float SoundsVolume
    {
        get { return m_soundsVolume; }
        set { m_soundsVolume = Mathf.Clamp01(value); }
    }

    public float MusicVolume
    {
        get { return m_musicVolume; }
        set { m_musicVolume = Mathf.Clamp01(value); }
    }
}
