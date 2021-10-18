using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlaySound : MonoBehaviour
{
    [SerializeField]
    private SoundEffectData[] m_audioClips = null;
    
    public void PlaySound(int soundToPlay = 0)
    {
        if (soundToPlay < m_audioClips.Length)
        {
            SoundEffectData audio = m_audioClips[soundToPlay];
            AudioManager.PlaySound(audio);
        }
    }
}
