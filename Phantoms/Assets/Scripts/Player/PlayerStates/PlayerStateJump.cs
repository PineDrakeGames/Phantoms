using UnityEngine;

public class PlayerStateJump : PlayerMovementState
{
    private bool m_holdingJump = true;

    public override void StateEnter()
    {
        m_holdingJump = true;

    }

    public override void TickInput(PlayerCharacterInputs input)
    {
        m_holdingJump = input.JumpHeld;
    }

    public override void TickRotation(ref Quaternion currentRotation, float deltaTime)
    {
        RotateTowardsMovement(ref currentRotation, Controller.OrientationSharpness, deltaTime);
    }

    public override void TickVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (Controller.Motor.GroundingStatus.IsStableOnGround)
        {
            if (Controller.MoveInputVector.sqrMagnitude > 0f)
            {
                Controller.SetState(new PlayerStateMove());
            }
            else
            {
                Controller.SetState(new PlayerStateIdle());
            }
        }
        else
        {
            if (Controller.MoveInputVector.sqrMagnitude > 0f)
            {
                Vector3 addedVelocity = Controller.MoveInputVector * Controller.AirAccelerationSpeed * deltaTime;

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

            // Gravity
            if (!m_holdingJump)
            {
                currentVelocity += Controller.Gravity * deltaTime;
            }
            else
            {
                currentVelocity += Controller.Gravity * deltaTime * 0.5f;
            }

            // Drag
            currentVelocity *= (1f / (1f + (Controller.Drag * deltaTime)));

            if (Vector3.Dot(currentVelocity, Controller.Gravity) > 0f)
            {
                Controller.SetState(new PlayerStateFall());
            }
        }
    }

    public override void StateExit()
    {

    }
}