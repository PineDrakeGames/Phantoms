using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ares;

public class BattleCameraManager : MonoBehaviour
{
    [SerializeField]
    private Transform m_cameraTransform = null;

    [SerializeField]
    private Transform m_defaultCameraPosition = null;

    [SerializeField]
    private Transform m_battleFinishedCameraPosition = null;

    [SerializeField]
    private AnimationCurve m_cameraTransitionCurve = null;


    // Things needed for camera transitions
    private Vector3 m_targetposition = Vector3.zero;
    private Quaternion m_targetRotation = Quaternion.identity;

    private Vector3 m_startposition = Vector3.zero;
    private Quaternion m_startRotation = Quaternion.identity;

    private float m_currentTransitionTime = 0f;
    private float m_transitionDuration = 0f;
    private bool m_transitioning = false;

    // Some variables that are constant
    private const float DEFAULT_TRANSITION_TIME = 0.4f;

    // For over the should things, some variables.
    [SerializeField]
    private float DISTANCE_BEHIND_PLAYER = 4.5f;
    [SerializeField]
    private float SIDE_ROTATION = -35f;
    [SerializeField]
    private float UP_ROTATION = -25f;

    // static instance stuff
    private static BattleCameraManager s_instance = null;
    public static BattleCameraManager Instance
    {
        get 
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<BattleCameraManager>();
                if (s_instance)
                {
                    GameObject cameraObject = Instantiate(new GameObject());
                    s_instance = cameraObject.AddComponent<BattleCameraManager>();
                    s_instance.m_cameraTransform = Camera.main.transform;
                    s_instance.m_cameraTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
                }
            }
            return s_instance;
        }
    }


    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }
        else if (s_instance != this)
        {
            Destroy(this);
            return;
        }
        m_cameraTransform.position = m_defaultCameraPosition.position;
        m_cameraTransform.rotation = m_defaultCameraPosition.rotation;
    }

    private void Update()
    {
        if (m_transitioning)
        {
            m_currentTransitionTime += Time.deltaTime;
            float progress = m_currentTransitionTime / m_transitionDuration;

            if (progress >= 1f)
            {
                m_cameraTransform.position = m_targetposition;
                m_cameraTransform.rotation = m_targetRotation;
                m_transitioning = false;
            }
            else
            {
                float curvePoint = m_cameraTransitionCurve.Evaluate(progress);
                m_cameraTransform.position = Vector3.Lerp(m_startposition, m_targetposition, curvePoint);
                m_cameraTransform.rotation = Quaternion.Slerp(m_startRotation, m_targetRotation, curvePoint);
            }
        }
    }

    /////////////////////////////////////////////////////////
    /// Public functions to handle specific battle events ///
    /////////////////////////////////////////////////////////
    public void OnTurnEnd(Actor currentActor)
    {
        if (currentActor.Group.Name != "Player")
        {
            return;
        }

        BattleGroup group = currentActor.Group;
        foreach (Actor actor in group.Actors)
        {
            if (actor != currentActor && BattleManager.Instance.CurrentBattle.HasRemainingTurns(actor))
            {
                // If an actor in the player group still has turn(s) left, then the player turn is not done.
                return;
            }
        }

        // If we reach this point, the player team's turn is over - should also reset the camera in this case.
        ResetCamera();
    }


    ///////////////////////////////////////////////////////////////
    /// Public functions to set the camera to various positions ///    
    ///////////////////////////////////////////////////////////////

    public void ResetCamera()
    {
        
        SetCamera(m_defaultCameraPosition.position, m_defaultCameraPosition.rotation);
    }

    public void BattleOverCamera()
    {
        SetCamera(m_battleFinishedCameraPosition.position, m_battleFinishedCameraPosition.rotation, 2f);
    }

    // Setting the camera using an explicit position and rotation
    public void SetCamera(Vector3 targetPosition, Quaternion targetRotation, float transitionDuration = DEFAULT_TRANSITION_TIME)
    {
        if (m_targetposition == targetPosition && m_targetRotation == targetRotation)
        {
            // If already approximately heading to the given position and rotation, just keep goin.
            return;
        }

        m_targetposition = targetPosition;
        m_targetRotation = targetRotation;

        SetTransitionTime(transitionDuration);
    }

    // Setting the camera using a given position, and a point for the camera to look at
    public void SetCamera(Vector3 targetPosition, Vector3 focusPoint, float transitionDuration = DEFAULT_TRANSITION_TIME)
    {
        m_targetposition = targetPosition;
        m_targetRotation = Quaternion.LookRotation(focusPoint - targetPosition, Vector3.up);

        SetTransitionTime(transitionDuration);
    }

    public void SetCameraOverShoulder(Vector3 player, Vector3[] targets)
    {
        Vector3 centeredTarget = Vector3.zero;
        foreach(Vector3 target in targets)
        {
            centeredTarget += target;
        }
        centeredTarget /= targets.Length;
        SetCameraOverShoulder(player, centeredTarget);
    }

    public void SetCameraOverShoulder(Vector3 player, Vector3 target)
    {
        Vector3 center = Vector3.Lerp(player, target, 0.5f);
        Vector3 cameraDirection = player - center;
        cameraDirection = Quaternion.Euler(0f, SIDE_ROTATION, UP_ROTATION) * cameraDirection.normalized;
        Vector3 cameraPosition = center + (cameraDirection.normalized * (Vector3.Distance(center, player) + DISTANCE_BEHIND_PLAYER));

        SetCamera(cameraPosition, center);
    }

    ////////////////////////////////
    /// Private Helper Functions ///
    ////////////////////////////////

    private void SetTransitionTime(float transitionDuration = DEFAULT_TRANSITION_TIME)
    {
        m_startposition = m_cameraTransform.position;
        m_startRotation = m_cameraTransform.rotation;

        m_transitioning = true;
        m_currentTransitionTime = 0f;
        m_transitionDuration = transitionDuration;
        if (m_transitionDuration < 0f) { m_transitionDuration = 0f; }
    }
}
