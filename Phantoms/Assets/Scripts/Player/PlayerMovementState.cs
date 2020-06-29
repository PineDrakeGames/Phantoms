using UnityEngine;

public abstract class PlayerMovementState
{
    public PlayerController Controller;

    public abstract void StateEnter();
    public abstract void TickInput(PlayerCharacterInputs input);
    public abstract void TickRotation(ref Quaternion currentRotation, float deltaTime);
    public abstract void TickVelocity(ref Vector3 currentVelocity, float deltaTime);
    public abstract void StateExit();

    /// Protected function for calculations that are common to many different states
    protected void RotateTowardsMovement(ref Quaternion currentRotation, float sharpness, float deltaTime)
    {
        if (Controller.LookInputVector.sqrMagnitude > 0f && sharpness > 0f)
        {
            // Smoothly interpolate from current to target look direction
            Vector3 smoothedLookInputDirection = Vector3.Slerp(Controller.Motor.CharacterForward, Controller.LookInputVector, 1 - Mathf.Exp(-sharpness * deltaTime)).normalized;

            // Set the current rotation (which will be used by the KinematicCharacterMotor)
            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Controller.Motor.CharacterUp);
        }

        Vector3 currentUp = (currentRotation * Vector3.up);
        Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-Controller.BonusOrientationSharpness * deltaTime));
        currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
    }
}
