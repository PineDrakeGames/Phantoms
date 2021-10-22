// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// </summary>
    [AddComponentMenu("")] // Hide from menu.
    public class SequencerCommandHeal : SequencerCommand
    {
        public void Start()
        {
            // TODO: Add parameters if we need heal options (heal just the player, a specific phantom, etc?
            DataManager.Instance.FullHeal();
            Stop();
        }
    }

}
