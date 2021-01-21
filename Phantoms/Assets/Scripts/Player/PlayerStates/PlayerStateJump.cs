using UnityEngine;

public class PlayerStateJump : PlayerMovementState
{
    private bool m_holdingJump = true;
    private bool m_SetInitialForce = false;

    public override void StateEnter()
    {
        m_holdingJump = true;
        m_SetInitialForce = true;
    }

    public override void TickInput(PlayerCharacterInputs input)
    {
        m_holdingJump = input.JumpHeld;
        CheckForAttackInput();
    }

    public override void TickRotation(ref Quaternion currentRotation, float deltaTime)
    {
        //RotateTowardsMovement(ref currentRotation, Controller.OrientationSharpness, deltaTime);
    }

    public override void TickVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (m_SetInitialForce)
        {
            Vector3 jumpDirection = Controller.Motor.CharacterUp;
            if (Controller.Motor.GroundingStatus.FoundAnyGround && !Controller.Motor.GroundingStatus.IsStableOnGround)
            {
                jumpDirection = Controller.Motor.GroundingStatus.GroundNormal;
            }

            // Makes the character skip ground probing/snapping on its next update. 
            // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
            Controller.Motor.ForceUnground();

            // Add to the return velocity and reset jump state
            currentVelocity += (jumpDirection * Controller.JumpUpSpeed) - Vector3.Project(currentVelocity, Controller.Motor.CharacterUp);
            currentVelocity += (Controller.MoveInputVector * Controller.JumpScalableForwardSpeed);
            Controller.Jump();
            m_SetInitialForce = false;
            return;
        }

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