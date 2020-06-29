using UnityEngine;

public class PlayerStateInteract : PlayerMovementState
{
    public override void StateEnter()
    {

    }

    public override void TickInput(PlayerCharacterInputs input)
    {
        
    }

    public override void TickRotation(ref Quaternion currentRotation, float deltaTime)
    {
        
    }

    public override void TickVelocity(ref Vector3 currentVelocity, float deltaTime)
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

    public override void StateExit()
    {

    }
}