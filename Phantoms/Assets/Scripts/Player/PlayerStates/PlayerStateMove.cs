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
            GroundMovement(ref currentVelocity, Controller.MoveInputVector, deltaTime);
        }
    }

    public override void StateExit()
    {

    }
}