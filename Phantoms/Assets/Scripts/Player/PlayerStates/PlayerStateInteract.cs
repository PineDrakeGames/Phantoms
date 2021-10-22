using UnityEngine;

public class PlayerStateInteract : PlayerMovementState
{

    public Vector3 TargetPosition = Vector3.zero;
    public Quaternion TargetRotation = Quaternion.identity;
    public bool MoveToTarget = false;
    public bool LookAtTarget = false;

    public override void StateEnter()
    {
        if (!Controller.Motor.GroundingStatus.IsStableOnGround)
        {
            Controller.CharacterAnimator.SetTrigger("Fall");
            Controller.CharacterAnimator.SetTrigger("Landed");
        }

        Controller.PutAwayScythe();
    }

    public override void TickInput(PlayerCharacterInputs input)
    {

    }

    public override void TickRotation(ref Quaternion currentRotation, float deltaTime)
    {
        // Do not update rotation.
        // TODO maybe: look at interaction item?
        if (MoveToTarget)
        {
            RotateTowardsMovement(ref currentRotation, Controller.OrientationSharpness, deltaTime, true);
        }
        else if (LookAtTarget)
        {
            currentRotation = Quaternion.RotateTowards(currentRotation, TargetRotation, 360f * deltaTime);
            if (Quaternion.Angle(currentRotation, TargetRotation) < 1f)
            {
                currentRotation = TargetRotation;
                LookAtTarget = false;
            }            
        }
    }

    public override void TickVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        Vector3 moveDirection = Vector3.zero;

        if (MoveToTarget)
        {
            Vector3 distanceToTarget = TargetPosition - Controller.transform.position;
            if (distanceToTarget.magnitude <= 0.2f)
            {
                MoveToTarget = false;
            }
            else
            {
                // Get the direction to move in - ignore y direction though.
                moveDirection = distanceToTarget;
                moveDirection.y = 0;
                moveDirection.Normalize();
            }
        }

        if (Controller.Motor.GroundingStatus.IsStableOnGround)
        {
            GroundMovement(ref currentVelocity, moveDirection, deltaTime);
        }
        else
        {
            AirStrafeMovement(ref currentVelocity, moveDirection, deltaTime);
            ApplyGravity(ref currentVelocity, deltaTime);
            ApplyDrag(ref currentVelocity, deltaTime);
        }
    }

    public override void StateExit()
    {

    }
}