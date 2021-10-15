// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Implements sequencer command: "MoveTo(target, [, subject[, duration]])", which matches the 
    /// subject to the target's position and rotation. If the subject has a rigidbody, uses
    /// Rigidbody.MovePosition/Rotation; otherwise sets the transform directly.
    /// 
    /// Arguments:
    /// -# The target. 
    /// -# (Optional) The subject; can be speaker, listener, or the name of a game object. 
    /// Default: speaker.
    /// -# (Optional) Duration in seconds.
    /// </summary>
    [AddComponentMenu("")] // Hide from menu.
    public class SequencerCommandMoveTo : SequencerCommand
    {

        private const float SmoothMoveCutoff = 0.05f;

        private Transform target;
        private Transform subject;
        private Rigidbody subjectRigidbody;
        // CJ NOTE: Adding in stuff for kinematic motors and specifically the player to move them how we want to
        private KinematicCharacterController.KinematicCharacterMotor subjectMotor;
        private PlayerController subjectController;
        private float duration;
        float startTime;
        float endTime;
        Vector3 originalPosition;
        Quaternion originalRotation;

        /// ADDED BY CJ ///
        // Adding in an enum for different types of movement!
        private enum MovementType
        {
            DEFAULT,
            EASEINOUT,
        }
        private MovementType movementType = MovementType.DEFAULT;

        public void Start()
        {
            // Get the values of the parameters:
            target = GetSubject(0);
            subject = GetSubject(1);
            duration = GetParameterAsFloat(2, 0);
            if (DialogueDebug.logInfo) Debug.Log(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}: Sequencer: MoveTo({1}, {2}, {3})", new System.Object[] { DialogueDebug.Prefix, target, subject, duration }));
            if ((target == null) && DialogueDebug.logWarnings) Debug.LogWarning(string.Format("{0}: Sequencer: MoveTo() target '{1}' wasn't found.", new System.Object[] { DialogueDebug.Prefix, GetParameter(0) }));
            if ((subject == null) && DialogueDebug.logWarnings) Debug.LogWarning(string.Format("{0}: Sequencer: MoveTo() subject '{1}' wasn't found.", new System.Object[] { DialogueDebug.Prefix, GetParameter(1) }));

            // Set up the move:
            if ((subject != null) && (target != null) && (subject != target))
            {
                subjectRigidbody = subject.GetComponent<Rigidbody>();
                subjectMotor = subject.GetComponentInChildren<KinematicCharacterController.KinematicCharacterMotor>();
                if (subjectMotor == null)
                {
                    subjectMotor = subject.GetComponentInParent<KinematicCharacterController.KinematicCharacterMotor>();
                }
                subjectController = subject.GetComponentInChildren<PlayerController>();
                if (subjectController == null)
                {
                    subjectController = subject.GetComponentInParent<PlayerController>();
                }

                // If duration is above the cutoff, smoothly move toward target:
                if (duration > SmoothMoveCutoff)
                {
                    startTime = DialogueTime.time;
                    endTime = startTime + duration;
                    originalPosition = subject.position;
                    originalRotation = subject.rotation;

                    if (subjectController)
                    {
                        if (subjectController.CurrentState is PlayerStateInteract)
                        {
                            PlayerStateInteract interactState = subjectController.CurrentState as PlayerStateInteract;
                            interactState.MoveToTarget = true;
                            interactState.TargetPosition = target.position;
                            interactState.LookAtTarget = true;
                            interactState.TargetRotation = target.rotation;
                        }
                    }
                }
                else
                {
                    Stop();
                }
            }
            else
            {
                Stop();
            }

            /// ADDED BY CJ ///
            // Check if there is a move type set
            string moveTypeString = GetParameter(3, "DEFAULT");
            moveTypeString = moveTypeString.ToUpper();
            moveTypeString = moveTypeString.Trim();
            if (moveTypeString == "EASE" || moveTypeString == "EASEIN" || moveTypeString == "EASEINOUT" || moveTypeString == "E")
            {
                movementType = MovementType.EASEINOUT;
            }
            else
            {
                movementType = MovementType.DEFAULT;
            }
        }

        private void SetPosition(Vector3 newPosition, Quaternion newRotation)
        {
            // For efficiency, doesn't warp NavMeshAgent.
            if (subjectMotor)
            {
                subjectMotor.SetPositionAndRotation(newPosition, newRotation);
            }
            else if (subjectRigidbody != null && !subjectRigidbody.isKinematic)
            {
                subjectRigidbody.MoveRotation(newRotation);
                subjectRigidbody.MovePosition(newPosition);
            }
            else
            {
                subject.rotation = newRotation;
                subject.position = newPosition;
            }
        }

        public void Update()
        {
            // Keep smoothing for the specified duration:
            if ((DialogueTime.time < endTime))
            {
                float elapsed = (DialogueTime.time - startTime) / duration;
                if (movementType == MovementType.EASEINOUT)
                {
                    elapsed = Mathf.SmoothStep(0f, 1f, elapsed);
                }
                if (!subjectController)
                {
                    SetPosition(Vector3.Lerp(originalPosition, target.position, elapsed), Quaternion.Lerp(originalRotation, target.rotation, elapsed));
                }
            }
            else
            {
                Stop();
            }
        }

        public void OnDestroy()
        {
            // Final position:
            if ((subject != null) && (target != null) && (subject != target))
            {
                //NOTE (CJ): If the target is something with a controller, and it's already moved to the position, we don't have to set it again at the end.
                if (subjectController)
                {
                    if (subjectController.CurrentState is PlayerStateInteract)
                    {
                        PlayerStateInteract interactState = subjectController.CurrentState as PlayerStateInteract;
                        if (!interactState.MoveToTarget && !interactState.LookAtTarget)
                        {
                            return;
                        }
                        else
                        {
                            interactState.MoveToTarget = false;
                            interactState.LookAtTarget = false;
                        }
                    }
                }
                SetPosition(target.position, target.rotation);
            }

        }

    }

}
