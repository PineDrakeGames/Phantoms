using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    public class SequencerCommandStarterQuestion : SequencerCommand
    {

        private bool answerSelected = false;

        public void Awake()
        {
            // Let the starter quiz manager know that we've finished showing the question, and reveal the answers!
            answerSelected = false;
            StarterQuizManager.Instance.OnAnswerSelect.AddListener(AnswerSelected);
            StarterQuizManager.Instance.ShowAnswers();
        }

        public void Update()
        {
            if (answerSelected)
            {
                Stop();
            }
        }

        public void OnDestroy()
        {
            StarterQuizManager.Instance.OnAnswerSelect.RemoveListener(AnswerSelected);
        }

        public void AnswerSelected()
        {
            answerSelected = true;
        }

    }

}

