using UnityEngine;

public class PlayerStateFall : PlayerMovementState
{
    public override void StateEnter()
    {

    }

    public override void TickInput()
    {

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
            currentVelocity += Controller.Gravity * deltaTime;

            // Drag
            currentVelocity *= (1f / (1f + (Controller.Drag * deltaTime)));
        }
    }

    public override void StateExit()
    {

    }
}