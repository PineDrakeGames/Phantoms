using UnityEngine;

public class PlayerStateHurt : PlayerMovementState
{

    public Vector3 TargetPosition = Vector3.zero;
    public Quaternion TargetRotation = Quaternion.identity;
    public bool MoveToTarget = false;
    public bool LookAtTarget = false;

    private Vector3 m_startingPosition = Vector3.zero;
    private float m_currentRespawnTime = 0f;
    bool finishedFading = false;

    private const float MAX_RESPAWN_MOVE_DISTANCE = 25f;

    private const float MOVING_DURATION = 0.3f;
    private const float FADE_OUT_DURATION = 0.75f;
    private const float FADE_IN_DURATION = 1f;

    private enum HurtSubState
    {
        FADING_OUT,
        MOVING,
        FADING_IN
    }

    private HurtSubState State = HurtSubState.FADING_OUT;

    public override void StateEnter()
    {
        Controller.CharacterAnimator.ResetTrigger("Respawn");
        Controller.CharacterAnimator.SetTrigger("Damage");

        AudioManager.PlaySound("BATTLE_HIT");

        float distance = Vector3.Distance(Controller.transform.position, TargetPosition);
        if (distance > MAX_RESPAWN_MOVE_DISTANCE)
        {
            MoveToTarget = false;
        }
        else
        {
            m_startingPosition = Controller.transform.position;
            MoveToTarget = true;
        }

        State = HurtSubState.FADING_OUT;
        m_currentRespawnTime = 0f;
        finishedFading = false;
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

        switch (State)
        {
            case HurtSubState.FADING_OUT:
                if (m_currentRespawnTime >= FADE_OUT_DURATION)
                {
                    if (!MoveToTarget)
                    {
                        Controller.Motor.SetPosition(TargetPosition);
                        if (LookAtTarget)
                        {
                            Controller.Motor.SetRotation(TargetRotation);
                        }
                    }
                    State = HurtSubState.MOVING;
                    Controller.CharacterAnimator.SetTrigger("Fall");
                    Controller.CharacterAnimator.SetTrigger("Landed");
                    m_currentRespawnTime = 0f;
                }
                break;

            case HurtSubState.MOVING:
                if (MoveToTarget)
                {
                    float progress = Mathf.Clamp01(m_currentRespawnTime / MOVING_DURATION);
                    // Faster at start, slower at end - exponential change
                    float easeInProgress = 1f - (Mathf.Pow((1f - progress), 2f));
                    Vector3 position = Vector3.Lerp(m_startingPosition, TargetPosition, easeInProgress);
                    Controller.Motor.SetPosition(position);
                }
                if (m_currentRespawnTime >= MOVING_DURATION)
                {
                    if (MoveToTarget)
                    {
                        Controller.Motor.SetPosition(TargetPosition);
                    }
                    if (LookAtTarget)
                    {
                        Controller.Motor.SetRotation(TargetRotation);
                    }

                    State = HurtSubState.FADING_IN;
                    Controller.CharacterAnimator.SetTrigger("Respawn");
                    AudioManager.PlaySound("BATTLE_HEAL");
                    m_currentRespawnTime = 0f;
                }
                break;

            case HurtSubState.FADING_IN:

                if (!finishedFading && m_currentRespawnTime >= FADE_IN_DURATION)
                {
                    finishedFading = true;
                    Controller.SetState(new PlayerStateIdle());
                }
                break;
        }
    }

    public override void StateExit()
    {
        Controller.CharacterAnimator.SetTrigger("Respawn");
    }
}