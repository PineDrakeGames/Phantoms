// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Implements sequencer command: "AnimatorAllTrigger(animatorParameter[,  gameobject|speaker|listener])",
    /// which smoothly changes a float parameter on a subject's Animator.
    /// 
    /// Arguments:
    /// -# Name of a Mecanim animator parameter.
    /// -# (Optional) Float value. Default: <c>1f</c>.
    /// -# (Optional) The subject; can be speaker, listener, or the name of a game object. Default: speaker.
    /// -# (Optional) Duration in seconds to smooth to the value.
    /// </summary>
    [AddComponentMenu("")] // Hide from menu.
    public class SequencerCommandAnimatorAllTrigger : SequencerCommand
    {
        private int animatorParameterHash = -1;
        private Transform subject = null;
        private Animator[] animators = null;


        public void Start()
        {
            // Get the values of the parameters:
            string animatorParameter = GetParameter(0);
            animatorParameterHash = Animator.StringToHash(animatorParameter);
            subject = GetSubject(1);

            // Check the parameters:
            if (subject == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning(string.Format("{0}: Sequencer: AnimatorAllTrigger(): subject '{1}' wasn't found.", new System.Object[] { DialogueDebug.Prefix, GetParameter(2) }));
                Stop();
            }
            else
            {
                animators = subject.GetComponentsInChildren<Animator>();
                if (animators == null)
                {
                    if (DialogueDebug.logWarnings) Debug.LogWarning(string.Format("{0}: Sequencer: AnimatorAllTrigger(): no Animators found on '{1}'.", new System.Object[] { DialogueDebug.Prefix, subject.name }));
                    Stop();
                }
                else
                {
                    foreach(Animator anim in animators)
                    {
                        anim.SetTrigger(animatorParameter);
                    }
                }
            }
        }
    }
}
