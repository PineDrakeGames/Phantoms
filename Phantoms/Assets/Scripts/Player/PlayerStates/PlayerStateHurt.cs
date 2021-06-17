using UnityEngine;

public class PlayerStateHurt : PlayerMovementState
{

    public Vector3 TargetPosition = Vector3.zero;
    public Quaternion TargetRotation = Quaternion.identity;
    public bool MoveToTarget = false;
    public bool LookAtTarget = false;

    private Vector3 m_startingPosition = Vector3.zero;
    private float m_currentRespawnTime = 0f;

    private const float MAX_RESPAWN_MOVE_DISTANCE = 15f;
    private const float RESPAWN_DURATION = 0.65f;


    public override void StateEnter()
    {
        Controller.CharacterAnimator.SetTrigger("Fall");
        Controller.CharacterAnimator.SetTrigger("Landed");

        float distance = Vector3.Distance(Controller.transform.position, TargetPosition);
        if (distance > MAX_RESPAWN_MOVE_DISTANCE)
        {
            Controller.Motor.SetPosition(TargetPosition);
            MoveToTarget = false;
            if (LookAtTarget)
            {
                Controller.Motor.SetRotation(TargetRotation);
            }
        }
        else
        {
            m_startingPosition = Controller.transform.position;
            MoveToTarget = true;
        }

        m_currentRespawnTime = 0f;
    }

    public override void TickInput(PlayerCharacterInputs input)
    {

    }

    public override void TickRotation(ref Quaternion currentRotation, float deltaTime)
    {
        // Do not update rotation.
        // TODO maybe: look at interaction item?
        if (LookAtTarget)
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
        currentVelocity = Vector3.zero;

        m_currentRespawnTime += Time.deltaTime;

        if (MoveToTarget)
        {
            float progress = Mathf.Clamp01(m_currentRespawnTime / RESPAWN_DURATION);
            // Faster at start, slower at end - exponential change
            float easeInProgress = 1f - (Mathf.Pow((1f - progress), 2f));
            Debug.Log(progress  + ", " + easeInProgress);
            Vector3 position = Vector3.Lerp(m_startingPosition, TargetPosition, easeInProgress);
            Controller.Motor.SetPosition(position);
        }

        if (m_currentRespawnTime >= RESPAWN_DURATION)
        {
            if (MoveToTarget)
            {
                Controller.Motor.SetPosition(TargetPosition);
            }
            if (LookAtTarget)
            {
                Controller.Motor.SetRotation(TargetRotation);
            }

            Controller.SetState(new PlayerStateIdle());
        }
    }

    public override void StateExit()
    {

    }
}