using UnityEngine;

public class PlayerStateFall : PlayerMovementState
{
    public override void StateEnter()
    {

    }

    public override void TickInput(PlayerCharacterInputs input)
    {

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
            AirStrafeMovement(ref currentVelocity, Controller.MoveInputVector, deltaTime);
            ApplyGravity(ref currentVelocity, deltaTime);
            ApplyDrag(ref currentVelocity, deltaTime);
        }
    }

    public override void StateExit()
    {

    }
}