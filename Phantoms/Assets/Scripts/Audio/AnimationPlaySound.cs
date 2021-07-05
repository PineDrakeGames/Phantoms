using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlaySound : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] m_audioClips = null;
    

    public void PlaySound(int soundToPlay = 0)
    {
        if (soundToPlay < m_audioClips.Length)
        {
            AudioManager.PlaySound(m_audioClips[soundToPlay]);
        }
    }
}
