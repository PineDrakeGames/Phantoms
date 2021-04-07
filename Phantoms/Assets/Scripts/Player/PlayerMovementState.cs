using UnityEngine;

public abstract class PlayerMovementState
{
    public PlayerController Controller;

    public abstract void StateEnter();
    public abstract void TickInput(PlayerCharacterInputs input);
    public abstract void TickRotation(ref Quaternion currentRotation, float deltaTime);
    public abstract void TickVelocity(ref Vector3 currentVelocity, float deltaTime);
    public abstract void StateExit();

    ////////////////////////////////////////////////////////////////////////////////////
    /// Protected function for calculations that are common to many different states ///
    ////////////////////////////////////////////////////////////////////////////////////

    /// Input Checking functions ///
    protected void CheckForJumpInput()
    {
        if (Controller.CanJump())
        {
            Controller.SetState(new PlayerStateJump());
        }
    }

    protected void CheckForAttackInput()
    {
        if (Controller.CanAttack())
        {
            Controller.SetState(new PlayerStateAttack());
        }
    }

    protected void CheckForDoubleJump()
    {
        if (Controller.CanDoubleJump())
        {
            Controller.SetState(new PlayerStateDoubleJump());
        }
    }

    /// Movement Functions ///

    // Rotates the player towards the controller's look input vector with the given sharpness.
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


    // Handles movement while on the ground.
    protected void GroundMovement(ref Vector3 currentVelocity, Vector3 moveVector, float deltaTime)
    {
        float currentVelocityMagnitude = currentVelocity.magnitude;

        Vector3 effectiveGroundNormal = Controller.Motor.GroundingStatus.GroundNormal;
        if (currentVelocityMagnitude > 0f && Controller.Motor.GroundingStatus.SnappingPrevented)
        {
            // Take the normal from where we're coming from
            Vector3 groundPointToCharacter = Controller.Motor.TransientPosition - Controller.Motor.GroundingStatus.GroundPoint;
            if (Vector3.Dot(currentVelocity, groundPointToCharacter) >= 0f)
            {
                effectiveGroundNormal = Controller.Motor.GroundingStatus.OuterGroundNormal;
            }
            else
            {
                effectiveGroundNormal = Controller.Motor.GroundingStatus.InnerGroundNormal;
            }
        }

        // Reorient velocity on slope
        currentVelocity = Controller.Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

        // Calculate target velocity
        if (moveVector != Vector3.zero)
        {
            Vector3 inputRight = Vector3.Cross(moveVector, Controller.Motor.CharacterUp);
            Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * moveVector.magnitude;
            Vector3 targetMovementVelocity = reorientedInput * Controller.MaxStableMoveSpeed;

            // Smooth movement Velocity
            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-Controller.StableMovementSharpness * deltaTime));
        }
        else
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, 1f - Mathf.Exp(-Controller.StableMovementSharpness * deltaTime));
        }
    }


    // Handles movement while in the air.
    protected void AirStrafeMovement(ref Vector3 currentVelocity, Vector3 moveVector, float deltaTime)
    {
        if (moveVector.sqrMagnitude > 0f)
        {
            Vector3 addedVelocity = moveVector * Controller.AirAccelerationSpeed * deltaTime;

            Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, Controller.Motor.CharacterUp);

            // Limit air velocity from inputs
            if (currentVelocityOnInputsPlane.magnitude < Controller.MaxAirMoveSpeed)
            {
                // clamp addedVel to make total vel not exceed max vel on inputs plane
                Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, Controller.MaxAirMoveSpeed);
                addedVelocity = newTotal - currentVelocityOnInputsPlane;
            }
            else
            {
                // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                {
                    addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                }
            }

            // Prevent air-climbing sloped walls
            if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
            {
                Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Controller.Motor.CharacterUp, Controller.Motor.GroundingStatus.GroundNormal), Controller.Motor.CharacterUp).normalized;
                addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
            }

            // Apply added velocity
            currentVelocity += addedVelocity;
        }
    }


    // Applies gravity.
    protected void ApplyGravity(ref Vector3 currentVelocity, float deltaTime)
    {
        currentVelocity += Controller.Gravity * deltaTime;
    }


    // Applies drag.
    protected void ApplyDrag(ref Vector3 currentVelocity, float deltaTime)
    {
        currentVelocity *= (1f / (1f + (Controller.Drag * deltaTime)));
    }
}
