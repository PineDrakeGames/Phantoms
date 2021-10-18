using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    [SerializeField]
    private float m_masterVolume = 0.5f;
    [SerializeField]
    private float m_soundsVolume = 0.5f;
    [SerializeField]
    private float m_musicVolume = 0.5f;

    public int LastSaveSlotPlayed = -1;

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

    public SettingsData()
    {
    }

    public SettingsData(SettingsData settings)
    {
        m_masterVolume = settings.MasterVolume;
        m_soundsVolume = settings.SoundsVolume;
        m_musicVolume = settings.MusicVolume;
        LastSaveSlotPlayed = settings.LastSaveSlotPlayed;
    }
}
