using UnityEngine;

public class PlayerStateIdle : PlayerMovementState
{
    public override void StateEnter()
    {

    }

    public override void TickInput(PlayerCharacterInputs input)
    {
        if (Controller.MoveInputVector.magnitude > 0f)
        {
            Controller.SetState(new PlayerStateMove());
        }
        CheckForJumpInput();
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
            GroundMovement(ref currentVelocity, Vector3.zero, deltaTime);
        }
    }

    public override void StateExit()
    {

    }
}