using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateDoubleJump : PlayerStateJump
{
    public override void StateEnter()
    {
        base.StateEnter();

        Controller.DoubleJump();
    }

    protected override void InitialJump(ref Vector3 currentVelocity)
    {
        Vector3 jumpDirection = Controller.Motor.CharacterUp;

        // Assume that we are in the air, not on ground

        // If we are already jumping upward, just add the extra height - otherwise, set the jump speed.
        if (Mathf.Cos(Vector3.Angle(currentVelocity, jumpDirection)) > 0)
        {
            currentVelocity += (jumpDirection * Controller.DoubleJumpSpeed) - Vector3.Project(currentVelocity, jumpDirection);
        }
        else
        {
            currentVelocity += (jumpDirection * Controller.DoubleJumpSpeed) - Vector3.Project(currentVelocity, jumpDirection);
        }
        
        Controller.Jump();
    }
}
