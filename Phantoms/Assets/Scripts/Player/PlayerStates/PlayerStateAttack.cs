using UnityEngine;

public class PlayerStateAttack : PlayerMovementState
{
    private float m_currentAttackTime = 0f;
    private bool m_attackStarted = false;
    private bool m_attackEnded = false;

    public override void StateEnter()
    {
        m_currentAttackTime = 0f;
        Controller.AttackHitbox.SetActive(true);
        m_attackStarted = false;
        m_attackEnded = false;

        Controller.Attack();
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
        m_currentAttackTime += deltaTime;
        // Start attack
        if (!m_attackStarted && m_currentAttackTime >= Controller.AttackDuration * Controller.AttackActiveTime.minValue)
        {
            Controller.AttackHitbox.SetActive(true);
            m_attackStarted = true;
        }
        else if (m_currentAttackTime >= Controller.AttackDuration * Controller.AttackActiveTime.maxValue)
        {
            if (!m_attackEnded)
            {
                Controller.AttackHitbox.SetActive(false);
                m_attackEnded = true;
            }

            if (m_currentAttackTime >= Controller.AttackDuration)
            {
                if (!Controller.Motor.GroundingStatus.IsStableOnGround)
                {
                    Controller.SetState(new PlayerStateFall());
                }
                else
                {
                    Controller.SetState(new PlayerStateIdle());
                }
                return;
            }
        }

        if (currentVelocity.magnitude > Controller.AttackMaxMoveSpeed)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, 1f - Mathf.Exp(-Controller.StableMovementSharpness * deltaTime));
            if (currentVelocity.magnitude < Controller.AttackMaxMoveSpeed)
            {
                currentVelocity = currentVelocity.normalized * Controller.AttackMaxMoveSpeed;
            }
        }
        
        /*
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
        */
    }

    public override void StateExit()
    {
        Controller.AttackHitbox.SetActive(false);
    }
}