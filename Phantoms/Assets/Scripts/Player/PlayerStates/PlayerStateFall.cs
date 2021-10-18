using UnityEngine;

public class PlayerStateFall : PlayerMovementState
{
    private float fallSpeed = 0f;

    public override void StateEnter()
    {
        Controller.CharacterAnimator.SetTrigger("Fall");
        Controller.CharacterAnimator.ResetTrigger("Landed");
    }

    public override void TickInput(PlayerCharacterInputs input)
    {
        CheckForJumpInput();
        CheckForAttackInput();
        CheckForDoubleJump();
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
            ApplyGravity(ref currentVelocity, deltaTime);
            ApplyDrag(ref currentVelocity, deltaTime);
            fallSpeed = currentVelocity.y * -1f;
        }
    }

    public override void StateExit()
    {
        Controller.CharacterAnimator.SetTrigger("Landed");

        if (fallSpeed >= Controller.LandSpeedRange.minValue)
        {
            float volume = Mathf.Lerp(Controller.LandVolumeRange.minValue, Controller.LandVolumeRange.maxValue,  Mathf.Clamp01((fallSpeed -  Controller.LandSpeedRange.minValue) / (Controller.LandSpeedRange.maxValue -  Controller.LandSpeedRange.minValue)));
            AudioManager.PlaySound(Controller.Land, volume);
        }
    }
}