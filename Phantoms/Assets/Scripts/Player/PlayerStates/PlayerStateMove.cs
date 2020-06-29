using UnityEngine;

public class PlayerStateMove : PlayerMovementState
{
    public override void StateEnter()
    {

    }

    public override void TickInput(PlayerCharacterInputs input)
    {
        if (Controller.MoveInputVector.magnitude == 0f)
        {
            Controller.SetState(new PlayerStateIdle());
        }
    }

    public override void TickRotation(ref Quaternion currentRotation, float deltaTime)
    {
        RotateTowardsMovement(ref currentRotation, Controller.OrientationSharpness, deltaTime);
    }

    public override void TickVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (!Controller.Motor.GroundingStatus.IsStableOnGround)
        {
            Controller.SetState(new PlayerStateFall());
        }
        else
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
            Vector3 inputRight = Vector3.Cross(Controller.MoveInputVector, Controller.Motor.CharacterUp);
            Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * Controller.MoveInputVector.magnitude;
            Vector3 targetMovementVelocity = reorientedInput * Controller.MaxStableMoveSpeed;

            // Smooth movement Velocity
            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-Controller.StableMovementSharpness * deltaTime));
        }
    }

    public override void StateExit()
    {

    }
}