using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlaySound : MonoBehaviour
{
    [System.Serializable]
    private class AnimationAudioTrigger
    {
        public AudioClip Clip;
        public string ClipID;
        [Range(0f, 1f)]
        public float VolumeScale = 1f;
        [MinMaxRange(-3f, 3f)]
        public RangedFloat PitchRange = new RangedFloat(1f, 1f);
    }
    [SerializeField]
    private AnimationAudioTrigger[] m_audioClips = null;
    

    public void PlaySound(int soundToPlay = 0)
    {
        if (soundToPlay < m_audioClips.Length)
        {
            AnimationAudioTrigger audio = m_audioClips[soundToPlay];
            float pitch = Random.Range(audio.PitchRange.minValue, audio.PitchRange.maxValue);
            if (audio.Clip != null)
            {
                AudioManager.PlaySound(audio.Clip, audio.VolumeScale, pitch);
            }
            else
            {
                AudioManager.PlaySound(audio.ClipID, audio.VolumeScale, pitch);
            }
        }
    }
}
