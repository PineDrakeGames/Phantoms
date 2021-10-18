// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Implements sequencer command: Animation(animation[, gameobject|speaker|listener[, animations...]])
    /// </summary>
    [AddComponentMenu("")] // Hide from menu.
    public class SequencerCommandSave : SequencerCommand
    {
        private float m_currentTime = 0f;
        private float minTime;
        private bool saveFinished = false;

        public void Start()
        {
            bool shouldHeal = GetParameterAsBool(0, false);
            minTime = GetParameterAsFloat(1, 1f);

            m_currentTime = 0f;
            saveFinished = false;
            PixelCrushers.SaveSystem.saveEnded += OnSaveEnded;

            PixelCrushers.SaveSystem.SaveToSlot(SaveDataManager.CurrentSaveSlot);

            if (shouldHeal)
            {
                DataManager.Instance.FullHeal();
            }

        }

        private void Update()
        {
            m_currentTime += Time.deltaTime;

            if (m_currentTime >= minTime && saveFinished)
            {
                Stop();
            }
        }

        public void OnSaveEnded()
        {
            Debug.Log("Save ended!");
            saveFinished = true;
        }

        private void OnDestroy()
        {
            PixelCrushers.SaveSystem.saveEnded -= OnSaveEnded;
        }
    }

}
