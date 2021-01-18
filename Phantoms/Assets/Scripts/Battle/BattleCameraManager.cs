using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCameraManager : MonoBehaviour
{
    [SerializeField]
    private Transform m_cameraTransform = null;

    [SerializeField]
    private Transform m_defaultCameraPosition = null;

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


    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Awake()
    {
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

    ///////////////////////////////////////////////////////////////
    /// Public functions to set the camera to various positions ///    
    ///////////////////////////////////////////////////////////////

    public void ResetCamera()
    {
        SetCamera(m_defaultCameraPosition.position, m_defaultCameraPosition.rotation);
    }

    // Setting the camera using an explicit position and rotation
    public void SetCamera(Vector3 targetPosition, Quaternion targetRotation, float transitionDuration = DEFAULT_TRANSITION_TIME)
    {
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
