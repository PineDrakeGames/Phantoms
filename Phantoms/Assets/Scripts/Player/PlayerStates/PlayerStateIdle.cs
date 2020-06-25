using UnityEngine;

public class PlayerStateIdle : PlayerMovementState
{
    public override void StateEnter()
    {

    }

    public override void TickInput()
    {
        if (Controller.MoveInputVector.magnitude > 0f)
        {
            Controller.SetState(new PlayerStateMove());
        }
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

            // Smooth movement Velocity
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, 1f - Mathf.Exp(-Controller.StableMovementSharpness * deltaTime));
        }
    }

    public override void StateExit()
    {

    }
}