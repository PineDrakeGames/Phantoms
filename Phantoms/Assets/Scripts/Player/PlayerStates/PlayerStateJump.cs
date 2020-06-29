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
        //RotateTowardsMovement(ref currentRotation, Controller.OrientationSharpness, deltaTime);
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

            // Gravity
            if (!m_holdingJump)
            {
                ApplyGravity(ref currentVelocity, deltaTime);
            }
            else
            {
                currentVelocity += Controller.Gravity * deltaTime * 0.5f;
            }

            // Drag
            ApplyDrag(ref currentVelocity, deltaTime);

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