using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public class CameraController : MonoBehaviour
{
    [Header("Player Camera Variables")]
    [SerializeField]
    private float m_distanceFromFocus = 10f;
    [SerializeField]
    private Vector3 m_cameraForward = Vector3.forward;
    [SerializeField]
    private float m_angleUp = 30f;

    [Header("Camera height variables")]
    [SerializeField]
    private float m_maxHeightDifference = 3f;
    [SerializeField]
    private float m_heightApproachTime = 0.5f;

    [Header("Lead Player Variables")]
    [SerializeField]
    private float m_leadDistance = 2f;
    [SerializeField]
    private float m_leadDelay = 3f;


    [HideInInspector]
    public Transform Player = null;

    [HideInInspector]
    public KinematicCharacterMotor PlayerMotor;

    // Variables used to calculate how much the camera should lead the player
    private float m_currentLead = 0f;
    private Vector3 m_prevPlayerPosition = Vector3.zero;

    // Variables used to set the Y position of the player.

    private float m_currentYPosition = 0f;

    // The focus position of the camera.
    private Vector3 m_focusPosition = Vector3.zero;

    private void OnValidate() {
        m_cameraForward.y = 0;
        m_cameraForward.Normalize();
    }

    private void Start() 
    {
        m_prevPlayerPosition = Player.position;
        m_currentYPosition = Player.position.y;
    }

    private void LateUpdate()
    {
        // First get camera focus position
        UpdateFocus();
        // Then set the camera position based on the focus.
        UpdateCamera();
    }


    private void UpdateFocus()
    {
        m_focusPosition = Player.position + GetLead();
        m_focusPosition.y = UpdateYPosition();

        m_prevPlayerPosition = Player.position;
    }

    private void UpdateCamera()
    {

        // Note: Assuming that the camera up will always just be Vector3.up

        Vector3 cameraOffset = Vector3.RotateTowards(m_cameraForward * -1f, Vector3.up, Mathf.Deg2Rad * m_angleUp, 0f) * m_distanceFromFocus;

        transform.position = m_focusPosition + cameraOffset;


        // The camera should now be in the right position, so just have it look at the focus point.
        transform.LookAt(m_focusPosition);
    }


    /// Private helper functions ///
    private Vector3 GetLead()
    {
        Vector3 distance = (Player.position - m_prevPlayerPosition);
        Vector3 offsetDirection =  Quaternion.Euler(0, -90, 0) * m_cameraForward;

        float lead = Vector3.Dot(offsetDirection, distance) / m_leadDelay;
        m_currentLead += lead;
        m_currentLead = Mathf.Clamp(m_currentLead, -1f, 1f);

        return (offsetDirection * m_leadDistance * Mathf.SmoothStep(-1f, 1f, (m_currentLead + 1f) / 2f));
    }

    private float UpdateYPosition()
    {
        float newYPosition = Player.position.y;
        float distance = Mathf.Abs(m_currentYPosition - newYPosition);
        if (PlayerMotor.GroundingStatus.FoundAnyGround || (newYPosition < m_currentYPosition) || (distance > m_maxHeightDifference))
        {
            if (!PlayerMotor.GroundingStatus.FoundAnyGround && distance > m_maxHeightDifference && newYPosition > m_currentYPosition)
            {
                distance -= m_maxHeightDifference;
                newYPosition -= m_maxHeightDifference; 
            }
            float progress =  Mathf.Sqrt(distance / m_maxHeightDifference);
            progress -= (Time.deltaTime / m_heightApproachTime);
            progress = Mathf.Clamp01(progress);

            float yDifference = Mathf.Pow(progress, 2f) * m_maxHeightDifference * Mathf.Sign(m_currentYPosition - newYPosition);
            m_currentYPosition = newYPosition + yDifference;
        }
        return m_currentYPosition;
    }
}
