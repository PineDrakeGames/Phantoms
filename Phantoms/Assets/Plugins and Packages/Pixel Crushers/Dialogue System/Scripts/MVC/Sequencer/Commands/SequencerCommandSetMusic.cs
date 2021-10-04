// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Command added by CJ!
    /// Allows us to play music from the dialogue system into our audio backend stuff.
    /// First parameter is what to play, but if set to 'none' or 'stop' it'll just stop the music.
    /// Second parameter is the intro for the song, if one is needed
    /// Third is the fade out time for the old song, if none is provided it'll just do the default.
    /// </summary>
    [AddComponentMenu("")] // Hide from menu.
    public class SequencerCommandSetMusic : SequencerCommand
    {

        public void Start()
        {
            string songID = GetParameter(0);
            string songIDIntro = GetParameter(1);
            string fadeTimeString = GetParameter(2);

            songID = songID.Trim().ToUpper();
            if (string.IsNullOrEmpty(songID) || songID == "NONE" || songID == "STOP")
            {
                AudioManager.StopMusic();
            }
            else
            {
                float fadeTime = 0f;
                if (float.TryParse(fadeTimeString, out fadeTime))
                {
                    AudioManager.PlayMusic(songID, songIDIntro, fadeTime);
                }
                else
                {
                    AudioManager.PlayMusic(songID, songIDIntro);
                }
            }
            Stop();
        }
    }
}
