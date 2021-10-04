// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Command added by CJ!
    /// Allows us to play ambience from the dialogue system into our audio backend stuff.
    /// First parameter is what to play - if set to "NONE" or "STOP", it'll just stop it, if set to "CURRENT" It just changes the volume.
    /// Second parameter is the volume multiplier to play it at.
    /// Third is the fade time for the ambience - halve it if fading out another ambience and fading in a new one..
    /// </summary>
    [AddComponentMenu("")] // Hide from menu.
    public class SequencerCommandSetAmbience : SequencerCommand
    {

        public void Start()
        {
            // Get the parameters, and parse them.
            string ambienceID = GetParameter(0);
            string volumeMultiplierString = GetParameter(1);
            string fadeTimeString = GetParameter(2);

            float volumeMultiplier = 1f;
            if (!string.IsNullOrEmpty(volumeMultiplierString))
            {
                volumeMultiplier = float.Parse(volumeMultiplierString);
            }

            float fadeTime = 2f;
            if (!string.IsNullOrEmpty(fadeTimeString))
            {
                fadeTime = float.Parse(fadeTimeString);
            }

            ambienceID = ambienceID.Trim().ToUpper();

            // Check what the ambience ID is.
            if (string.IsNullOrEmpty(ambienceID) || ambienceID == "NONE" || ambienceID == "STOP")
            {
                AudioManager.StopAmbience();
            }
            else if (ambienceID == "CURRENT")
            {
                AudioManager.SetAmbienceVolume(volumeMultiplier, fadeTime);
            }
            else
            {
                AudioManager.PlayAmbience(ambienceID, volumeMultiplier, fadeTime);
            }
            Stop();
        }
    }
}
