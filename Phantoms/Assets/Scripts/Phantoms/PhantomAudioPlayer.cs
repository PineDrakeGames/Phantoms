using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomAudioPlayer : MonoBehaviour
{
    public PhantomData Data;

    public enum SoundType
    {
        ATTACK,
        BUFF,
        PAIN,
        DEATH
    }

    /// Public Functions for all the different sound effects - so animation events are easier?
    public void PlayAttackSound()
    {
        PlaySound(SoundType.ATTACK);
    }

    public void PlayBuffSound()
    {
        PlaySound(SoundType.BUFF);
    }

    public void PlayPainSound()
    {
        PlaySound(SoundType.PAIN);
    }

    public void PlayDeathSound()
    {
        PlaySound(SoundType.DEATH);
    }


    // TODO: Play from their own audiosource with some spatialization and stuff!

    public void PlaySound(SoundType type)
    {

        if (Data == null)
        {
            return;
        }

        AudioClip clip = null;
        switch(type)
        {
            case SoundType.ATTACK:
                clip = Data.AttackClip;
                break;
            case SoundType.BUFF:
                clip = Data.BuffClip;
                break;
            case SoundType.PAIN:
                clip = Data.PainClip;
                break;
            case SoundType.DEATH:
                clip = Data.DeathClip;
                break;
        }

        if (clip != null)
        {
            AudioManager.PlaySound(clip);
        }
    }
}
