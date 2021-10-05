using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ares;

[CreateAssetMenu(menuName = "Phantoms/Phantom Data")]
public class PhantomData : UserBattleData
{
    [Header("Stats stuff")]
    public PhantomType MainType = PhantomType.NONE;
    public PhantomType SecondType = PhantomType.NONE;

    [Header("Audio Clips")]
    public AudioClip AttackClip = null;
    public AudioClip BuffClip = null;
    public AudioClip PainClip = null;
    public AudioClip DeathClip = null;

}
