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
    }

    ////////////////////////
    /// Public Functions ///
    ////////////////////////

    /// Music Functions ///

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
    }

    /////////////////////////
    /// Private Functions ///
    /////////////////////////
    private void Initialize()
    {
        DontDestroyOnLoad(s_instance);

        m_musicSource = gameObject.AddComponent<AudioSource>();
        m_musicIntroSource = gameObject.AddComponent<AudioSource>();
        m_oneShotSoundSource = gameObject.AddComponent<AudioSource>();
        m_musicSource.loop = true;
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

        float newVolume = Mathf.Clamp01(m_currentMusicFadeOutTime / m_musicFadeOutTime) * DataManager.Instance.GetMusicVolume();;
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
        foreach(AudioSource src in m_oneShotSoundSources)
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
