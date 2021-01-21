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
        // Do not update rotation.
        // TODO maybe: look at interaction item?
    }

    public override void TickVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (Controller.Motor.GroundingStatus.IsStableOnGround)
        {
            GroundMovement(ref currentVelocity, Vector3.zero, deltaTime);
        }
        else
        {
            AirStrafeMovement(ref currentVelocity, Vector3.zero, deltaTime);
            ApplyGravity(ref currentVelocity, deltaTime);
            ApplyDrag(ref currentVelocity, deltaTime);
        }
    }

    public override void StateExit()
    {

    }
}