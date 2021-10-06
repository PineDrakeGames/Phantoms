using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    private static AudioManager s_instance = null;
    public static AudioManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                GameObject audioObject = Instantiate(new GameObject("Audio Manager"));
                s_instance = audioObject.AddComponent<AudioManager>();
            }
            return s_instance;
        }
    }

    // Private variables for Ambience
    private AudioSource m_ambienceSource = null;
    private AudioClip m_ambienceClip = null;
    private AudioClip m_queuedAmbienceClip = null;

    private float m_ambienceCurrentVolumeMultiplier = 0f;
    private float m_ambiencePrevVolumeMultiplier = 0f;
    private float m_ambienceTargetVolumeMultiplier = 0f;

    private const float DEFAULT_AMBIENCE_VOLUME = 0.05f;
    private const float DEFAULT_AMBIENCE_FADE_TIME = 2f;
    private bool m_fadingAmbience = false;
    private bool m_queuedAmbience = false;
    private float m_currentAmbienceFadeTime = 0f;
    private float m_ambienceFadeTime = 0f;

    // Private variables for Music
    private AudioSource m_musicSource = null;
    private AudioSource m_musicIntroSource = null;

    private AudioClip m_musicClip = null;
    private AudioClip m_musicIntroClip = null;

    private AudioClip m_queuedMusicClip = null;
    private AudioClip m_queuedMusicIntroClip = null;

    private const float DEFAULT_MUSIC_FADE_OUT_TIME = 0.5f;
    private float m_musicFadeOutTime = 0.5f;
    private float m_currentMusicFadeOutTime = 0.5f;
    private bool m_fadingOutMusic = false;


    // Private variables for Sound Effects
    private AudioSource m_oneShotSoundSource = null;
    private List<AudioSource> m_oneShotSoundSources = new List<AudioSource>();
    private List<AudioSource> m_loopingSoundSources = new List<AudioSource>();

    // Public Getters for stuff!
    public static AudioClip CurrentMusicClip { get { return Instance.m_musicClip; } }
    public static AudioClip CurrentMusicIntroClip { get { return Instance.m_musicIntroClip; } }


    ///////////////////////
    /// Unity Functions ///
    ///////////////////////

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
            Initialize();
        }
        else if (this != s_instance)
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (m_fadingOutMusic)
        {
            FadeOutMusic();
        }
        if (m_fadingAmbience)
        {
            FadeAmbience();
        }
    }

    ////////////////////////
    /// Public Functions ///
    ////////////////////////

    /// Ambience Functions ///
    public static void PlayAmbience(string ambienceID, float volumeMultiplier = 1f, float fadeTime = DEFAULT_AMBIENCE_FADE_TIME)
    {
        Instance.PlayAmbienceInternal(DataManager.AmbienceData.GetClip(ambienceID), volumeMultiplier, fadeTime);
    }

    public static void PlayAmbience(AudioClip ambience, float volumeMultiplier = 1f, float fadeTime = DEFAULT_AMBIENCE_FADE_TIME)
    {
        Instance.PlayAmbienceInternal(ambience, volumeMultiplier, fadeTime);
    }

    public static void SetAmbienceVolume(float volumeMultiplier = 1f, float fadeTime = DEFAULT_AMBIENCE_FADE_TIME)
    {
        Instance.PlayAmbienceInternal(Instance.m_ambienceClip, volumeMultiplier, fadeTime);
    }

    public static void StopAmbience(float fadeTime = DEFAULT_AMBIENCE_FADE_TIME)
    {
        Instance.StopAmbienceInternal(fadeTime);
    }

    /// Music Functions ///

    public static void PlayMusic(string musicID, string introID = "", float fadeOutTime = DEFAULT_MUSIC_FADE_OUT_TIME)
    {
        AudioClip clip = DataManager.MusicData.GetClip(musicID);
        if (clip != null)
        {
            Instance.PlayMusicInternal(clip, DataManager.MusicData.GetClip(introID), fadeOutTime);
        }
    }

    public static void PlayMusic(AudioClip music, AudioClip intro = null, float fadeOutTime = DEFAULT_MUSIC_FADE_OUT_TIME)
    {
        Instance.PlayMusicInternal(music, intro, fadeOutTime);
    }

    public static void StopMusic(float fadeOutTime = DEFAULT_MUSIC_FADE_OUT_TIME)
    {
        Instance.StopMusicInternal(fadeOutTime);
    }

    /// Sound Functions ///

    public static void PlaySound(AudioClip sound, float volumeScale = 1f, float pitch = 1f)
    {
        Instance.PlayOneShotInternal(sound, volumeScale, pitch);
    }
    public static void PlaySound(string soundClipID, float volumeScale = 1f, float pitch = 1f)
    {
        Instance.PlayOneShotInternal(DataManager.SoundEffectData.GetClip(soundClipID), volumeScale, pitch);
    }

    public static void PlayLoopingSound(AudioClip sound, float volumeScale = 1f)
    {
        // TODO!
    }

    /// Misc Functions ///

    public static void UpdateVolume()
    {
        Instance.m_musicSource.volume = DataManager.Instance.GetMusicVolume();
        Instance.m_musicIntroSource.volume = DataManager.Instance.GetMusicVolume();
        Instance.m_oneShotSoundSource.volume = DataManager.Instance.GetSoundVolume();
        Instance.m_ambienceSource.volume = Instance.GetAmbienceVolume();
    }

    /////////////////////////
    /// Private Functions ///
    /////////////////////////
    private void Initialize()
    {
        DontDestroyOnLoad(s_instance);

        m_ambienceSource = gameObject.AddComponent<AudioSource>();
        m_musicSource = gameObject.AddComponent<AudioSource>();
        m_musicIntroSource = gameObject.AddComponent<AudioSource>();
        m_oneShotSoundSource = gameObject.AddComponent<AudioSource>();
        m_ambienceSource.loop = true;
        m_musicSource.loop = true;
    }

    /// Ambience Functions ///

    private void PlayAmbienceInternal(AudioClip ambience, float volumeMultiplier = 1f, float fadeTime = DEFAULT_AMBIENCE_FADE_TIME)
    {
        m_ambiencePrevVolumeMultiplier = m_ambienceCurrentVolumeMultiplier;

        // if the new clip is nothing, or the volume is 0, just stop the clip instead
        if (ambience == null || volumeMultiplier <= 0f)
        {
            StopAmbienceInternal(fadeTime);
            return;
        }

        // if fade time is less than 0, then just instantly start playing the ambience.
        if (fadeTime <= 0)
        {
            m_ambienceClip = ambience;
            m_ambienceSource.clip = ambience;
            m_ambienceCurrentVolumeMultiplier = volumeMultiplier;
            m_ambienceSource.volume = GetAmbienceVolume();
            m_ambienceSource.Play();

            m_fadingAmbience = false;
            m_queuedAmbience = false;
        }
        else
        {
            // At this point, we know that the ambience clip is not null, the volume is greater than 0, as is the fade time -
            // so just check if we need to fade something out, then start fading this in.

            m_ambienceFadeTime = fadeTime;
            m_currentAmbienceFadeTime = 0f;
            m_ambienceTargetVolumeMultiplier = volumeMultiplier;
            m_fadingAmbience = true;

            // If no clip is currently playing, set it up to fade in.
            if (!m_ambienceSource.isPlaying)
            {
                m_ambienceClip = ambience;
                m_ambienceSource.clip = ambience;
                m_ambienceSource.volume = 0f;
                m_ambienceSource.Play();
                m_ambiencePrevVolumeMultiplier = 0f;

                m_queuedAmbience = false;
            }
            else
            {
                if (ambience == m_ambienceClip)
                {
                    m_queuedAmbience = false;
                }
                else
                {
                    m_queuedAmbienceClip = ambience;
                    m_queuedAmbience = true;
                }
            }
        }

    }

    private void StopAmbienceInternal(float fadeOutTime = DEFAULT_AMBIENCE_FADE_TIME)
    {
        if (!m_ambienceSource.isPlaying) { return; }

        m_queuedAmbience = false;
        m_queuedAmbienceClip = null;
        m_ambiencePrevVolumeMultiplier = m_ambienceCurrentVolumeMultiplier;

        if (fadeOutTime <= 0)
        {
            m_ambienceSource.Stop();
            m_ambienceClip = null;
            m_fadingAmbience = false;
        }
        else
        {
            m_ambienceFadeTime = fadeOutTime;
            m_currentAmbienceFadeTime = 0f;
            m_ambienceTargetVolumeMultiplier = 0f;
            m_fadingAmbience = false;
        }
    }

    private void FadeAmbience()
    {
        m_currentAmbienceFadeTime += Time.deltaTime;

        // If we are fading the ambience, we are either switching ambiences, or changing the volume. Check that first to get the 'real' target volume.
        float targetVolumeMultiplier;
        if (m_queuedAmbience)
        {
            targetVolumeMultiplier = 0f;
        }
        else
        {
            targetVolumeMultiplier = m_ambienceTargetVolumeMultiplier;
        }

        float progress = Mathf.Clamp01(m_currentAmbienceFadeTime / m_ambienceFadeTime);
        m_ambienceCurrentVolumeMultiplier = Mathf.Lerp(m_ambiencePrevVolumeMultiplier, targetVolumeMultiplier, progress);

        m_ambienceSource.volume = GetAmbienceVolume();

        // Check if we are finished fading.
        if (progress >= 1f)
        {
            // If we want to fade in a new clip, set that up now.
            if (m_queuedAmbience)
            {
                m_ambienceSource.Stop();
                m_ambienceSource.clip = m_queuedAmbienceClip;
                m_ambienceClip = m_queuedAmbienceClip;

                if (m_queuedAmbienceClip == null)
                {
                    // If the clip we queued is nothing, then just stop - done fading stuff
                    m_fadingAmbience = false;
                }
                else
                {
                    // Otherwise, start up the new clip and set needed values.
                    m_ambienceSource.volume = 0f;
                    m_ambienceSource.Play();
                    m_ambiencePrevVolumeMultiplier = 0f;
                    m_currentAmbienceFadeTime = 0f;

                    m_queuedAmbience = false;
                }
            }
            else
            {
                m_fadingAmbience = false;
            }
        }
    }

    private float GetAmbienceVolume()
    {
        return Mathf.Clamp01(DEFAULT_AMBIENCE_VOLUME * m_ambienceCurrentVolumeMultiplier) * DataManager.Instance.GetMusicVolume();
    }

    /// Music Functions ///

    private void PlayMusicInternal(AudioClip music, AudioClip intro = null, float fadeOutTime = DEFAULT_MUSIC_FADE_OUT_TIME)
    {
        if (music == m_queuedMusicClip || (music == m_musicClip && !m_fadingOutMusic))
        {
            // Already playing the music, or the music is already queued up! Skip this.
            return;
        }

        m_queuedMusicClip = music;
        m_queuedMusicIntroClip = intro;

        if (m_musicClip == null)
        {
            StartMusic();
        }
        else
        {
            StopMusicInternal(fadeOutTime);
        }
    }

    private void StopMusicInternal(float fadeOutTime = DEFAULT_MUSIC_FADE_OUT_TIME)
    {
        if (fadeOutTime <= 0)
        {
            m_musicSource.Stop();
            m_musicIntroSource.Stop();
            m_fadingOutMusic = false;
        }
        else
        {
            // Really more of an edge case, but switch to the fade out time of the next song.
            if (m_fadingOutMusic)
            {
                float fadeOutProgress = m_currentMusicFadeOutTime / m_musicFadeOutTime;
                m_musicFadeOutTime = fadeOutTime;
                m_currentMusicFadeOutTime = fadeOutTime * fadeOutProgress;
            }
            else
            {
                m_musicFadeOutTime = fadeOutTime;
                m_currentMusicFadeOutTime = fadeOutTime;
            }
            m_fadingOutMusic = true;
        }
    }

    private void FadeOutMusic()
    {
        m_currentMusicFadeOutTime -= Time.deltaTime;

        float newVolume = Mathf.Clamp01(m_currentMusicFadeOutTime / m_musicFadeOutTime) * DataManager.Instance.GetMusicVolume(); ;
        m_musicSource.volume = newVolume;
        m_musicIntroSource.volume = newVolume;

        if (m_currentMusicFadeOutTime <= 0f)
        {
            m_musicSource.Stop();
            m_musicIntroSource.Stop();
            m_fadingOutMusic = false;

            StartMusic();
        }
    }

    private void StartMusic()
    {
        if (m_queuedMusicClip == null) { return; }

        m_musicSource.volume = DataManager.Instance.GetMusicVolume();
        m_musicIntroSource.volume = DataManager.Instance.GetMusicVolume();

        if (m_queuedMusicIntroClip != null)
        {
            m_musicSource.clip = m_queuedMusicClip;
            m_musicIntroSource.clip = m_queuedMusicIntroClip;
            m_musicIntroSource.Play();
            m_musicSource.PlayScheduled(AudioSettings.dspTime + (double)(m_queuedMusicIntroClip.length));
        }
        else
        {
            // TODO: Set volyme!
            m_musicSource.clip = m_queuedMusicClip;
            m_musicSource.Play();
        }

        m_musicClip = m_queuedMusicClip;
        m_musicIntroClip = m_queuedMusicIntroClip;
        m_queuedMusicClip = null;
        m_queuedMusicIntroClip = null;
    }

    /// Internal Sound Effect Functions ///

    private void PlayOneShotInternal(AudioClip clip, float volumeScale = 1f, float pitch = 1f)
    {
        // If just using the default pitch, can use the same basic audio source.
        if (pitch == 1f)
        {
            m_oneShotSoundSource.PlayOneShot(clip, volumeScale * DataManager.Instance.GetSoundVolume());
        }
        else
        {
            AudioSource source = GetFreeOneShotSource();
            source.pitch = pitch;
            source.volume = volumeScale * DataManager.Instance.GetSoundVolume();
            source.clip = clip;
            source.Play();
        }
    }

    private AudioSource GetFreeOneShotSource()
    {
        AudioSource source = null;
        foreach (AudioSource src in m_oneShotSoundSources)
        {
            if (!src.isPlaying)
            {
                source = src;
                source.pitch = 1f;
                source.volume = 1f;
                source.clip = null;
            }
        }

        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
            m_oneShotSoundSources.Add(source);
        }

        return source;
    }
}
