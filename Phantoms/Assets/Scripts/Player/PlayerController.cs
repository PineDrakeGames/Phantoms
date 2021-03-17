//#define DEBUG_LOG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public struct PlayerCharacterInputs
{
    public float MoveAxisForward;
    public float MoveAxisRight;
    public Quaternion CameraRotation;
    public bool JumpDown;
    public bool JumpHeld;
    public bool InteractDown;
    public bool AttackDown;
}

public struct AICharacterInputs
{
    public Vector3 MoveVector;
    public Vector3 LookVector;
}

public class PlayerController : MonoBehaviour, ICharacterController
{
    public KinematicCharacterMotor Motor;

    [Header("Stable Movement")]
    public float MaxStableMoveSpeed = 10f;
    public float StableMovementSharpness = 15f;
    public float OrientationSharpness = 10f;

    [Header("Air Movement")]
    public float MaxAirMoveSpeed = 15f;
    public float AirAccelerationSpeed = 15f;
    public float Drag = 0.1f;

    [Header("Jumping")]
    public float JumpUpSpeed = 10f;
    public float JumpScalableForwardSpeed = 10f;
    public float JumpPreGroundingGraceTime = 0f;
    public float JumpPostGroundingGraceTime = 0f;

    [Header("Attack Info")]
    public float AttackDuration = 0.35f;
    [MinMaxRange(0f, 1f)]
    public RangedFloat AttackActiveTime = new RangedFloat();
    public GameObject AttackHitbox = null;
    public float AttackInputLingerTime = 0.15f;
    public float AttackMaxMoveSpeed = 1f;

    [Header("Art stuff")]
    [SerializeField]
    private Animator m_characterAnimator = null;
    public Animator CharacterAnimator { get { return m_characterAnimator; }}
    public float RunSpeedScale = 0.5f;

    [Header("Misc")]
    public List<Collider> IgnoredColliders = new List<Collider>();
    public float BonusOrientationSharpness = 10f;
    public Vector3 Gravity = new Vector3(0, -30f, 0);
    public Transform MeshRoot;
    public Transform CameraFollowPoint;

    private Collider[] _probedColliders = new Collider[8];
    private RaycastHit[] _probedHits = new RaycastHit[8];
    private Vector3 _moveInputVector;
    public Vector3 MoveInputVector { get { return _moveInputVector; } }
    private Vector3 _lookInputVector;
    public Vector3 LookInputVector { get { return _lookInputVector; } }
    private bool _jumpRequested = false;
    private bool _attackRequested = false;
    private bool _attackedInAir = false;
    private float _timeSinceJumpRequested = Mathf.Infinity;
    private float _timeSinceLastAbleToJump = 0f;
    private float _timeSinceAttackRequested = Mathf.Infinity;
    private Vector3 _internalVelocityAdd = Vector3.zero;

    private Interactable m_currentInteractable = null;
    public Interactable CurrentInteractable
    {
        set { m_currentInteractable = value; }
    }

    private Vector3 lastInnerNormal = Vector3.zero;
    private Vector3 lastOuterNormal = Vector3.zero;

    private PlayerMovementState m_prevState = null;
    public PlayerMovementState PrevState
    {
        get { return m_prevState; }
    }
    private PlayerMovementState m_currentState = null;
    public PlayerMovementState CurrentState
    {
        get { return m_currentState; }
        set { SetState(value); }
    }

    private void Start()
    {
        if (AttackHitbox) { AttackHitbox.SetActive(false); }

        // Assign the characterController to the motor
        Motor.CharacterController = this;

        // Handle initial state
        m_currentState = new PlayerStateIdle();
        SetState(new PlayerStateIdle());
    }

    /// <summary>
    /// Handles movement state transitions and enter/exit callbacks
    /// </summary>
    public void SetState(PlayerMovementState newState)
    {
        if (newState == null) { return; }

#if DEBUG_LOG
        Debug.Log("Transitioning from state - " + m_currentState.GetType() + " - to state - " + newState.GetType());
#endif

        PlayerMovementState m_prevState = m_currentState;
        m_currentState.StateExit();
        newState.Controller = this;
        m_currentState = newState;
        m_currentState.StateEnter();
    }


    /// <summary>
    /// This is called every frame by ExamplePlayer in order to tell the character what its inputs are
    /// </summary>
    public void SetInputs(ref PlayerCharacterInputs inputs)
    {
        // Clamp input
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);

        // Calculate camera direction and rotation on the character plane
        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
        if (cameraPlanarDirection.sqrMagnitude == 0f)
        {
            cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Motor.CharacterUp).normalized;
        }
        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);


        // Move and look inputs
        _moveInputVector = cameraPlanarRotation * moveInputVector;
        _lookInputVector = _moveInputVector.normalized;

        // Jumping input
        if (inputs.JumpDown)
        {
            _timeSinceJumpRequested = 0f;
            _jumpRequested = true;
        }

        if (inputs.AttackDown)
        {
            _timeSinceAttackRequested = 0f;
            _attackRequested = true;
        }

        m_currentState.TickInput(inputs);

        if (inputs.InteractDown && m_currentInteractable != null)
        {
            m_currentInteractable.Interact();
        }
    }

    /// <summary>
    /// This is called every frame by the AI script in order to tell the character what its inputs are
    /// </summary>
    public void SetInputs(ref AICharacterInputs inputs)
    {
        _moveInputVector = inputs.MoveVector;
        _lookInputVector = inputs.LookVector;
    }

    private Quaternion _tmpTransientRot;

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is called before the character begins its movement update
    /// </summary>
    public void BeforeCharacterUpdate(float deltaTime)
    {
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is where you tell your character what its rotation should be right now. 
    /// This is the ONLY place where you should set the character's rotation
    /// </summary>
    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        m_currentState.TickRotation(ref currentRotation, deltaTime);
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is where you tell your character what its velocity should be right now. 
    /// This is the ONLY place where you can set the character's velocity
    /// </summary>
    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        m_currentState.TickVelocity(ref currentVelocity, deltaTime);

        // Take into account additive velocity
        if (_internalVelocityAdd.sqrMagnitude > 0f)
        {
            currentVelocity += _internalVelocityAdd;
            _internalVelocityAdd = Vector3.zero;
        }

        // Update animator with speed
        m_characterAnimator.SetFloat("Speed", currentVelocity.magnitude * RunSpeedScale);
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is called after the character has finished its movement update
    /// </summary>
    public void AfterCharacterUpdate(float deltaTime)
    {
        // Handle jump-related values
        {
            _timeSinceJumpRequested += deltaTime;
            // Handle jumping pre-ground grace period
            if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
            {
                _jumpRequested = false;
            }

            if (Motor.GroundingStatus.IsStableOnGround)
            {
                _timeSinceLastAbleToJump = 0f;
            }
            else
            {
                // Keep track of time since we were last able to jump (for grace period)
                _timeSinceLastAbleToJump += deltaTime;
            }
        }
        
        // Handle Attack-related values
        {
            _timeSinceAttackRequested += deltaTime;
            if (_attackRequested && _timeSinceAttackRequested > AttackInputLingerTime)
            {
                _attackRequested = false;
            }
        }
    }

    public void PostGroundingUpdate(float deltaTime)
    {
        // Handle landing and leaving ground
        if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
        {
            OnLanded();
        }
        else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
        {
            OnLeaveStableGround();
        }
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        if (IgnoredColliders.Count == 0)
        {
            return true;
        }

        if (IgnoredColliders.Contains(coll))
        {
            return false;
        }

        return true;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        Rigidbody r = hitCollider.attachedRigidbody;
        if (r)
        {
            Vector3 relativeVel = Vector3.Project(r.velocity, hitNormal) - Vector3.Project(Motor.Velocity, hitNormal);
        }
    }

    public void AddVelocity(Vector3 velocity)
    {
        _internalVelocityAdd += velocity;
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
    }

    protected void OnLanded()
    {
        _attackedInAir = false;
    }

    protected void OnLeaveStableGround()
    {
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
    }


    /// Functions to send data to the different player states. ///
    public bool CanJump()
    {
        if (_jumpRequested)
        {
            // See if we actually are allowed to jump
            if (Motor.GroundingStatus.IsStableOnGround || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime)
            {
                return true;
            }
        }
        return false;
    }

    public void Jump()
    {
        // Calculate jump direction before ungrounding
        _jumpRequested = false;
    }

    public bool CanAttack()
    {
        return (_attackRequested && !_attackedInAir);
    }

    public void Attack()
    {
        // Calculate jump direction before ungrounding
        _attackRequested = false;
        if (!Motor.GroundingStatus.IsStableOnGround)
        {
            _attackedInAir =  true;
        }

        AttackManager.StartAttack();
    }
}
