using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamStateDefault : PlayerCameraState
{
    public OverworldCameraSettings m_camSettings = null;

    // Variables used to calculate how much the camera should lead the player
    private float m_currentLead = 0f;
    private float m_currentDepthLead = 0f;
    private Vector3 m_prevPlayerPosition = Vector3.zero;

    // Variables used to set the Y position of the player.
    private float m_currentYPosition = 0f;

    // The focus position of the camera.
    private Vector3 m_focusPosition = Vector3.zero;

    private const float SPEED_LEAD_SCALE = 0.1f;


    public PlayerCamStateDefault()
    {
        ResetCameraPosition();
    }
    public PlayerCamStateDefault(CameraController controller, OverworldCameraSettings settings)
    {
        Controller = controller;
        m_camSettings = settings;
        ResetCameraPosition();
    }


    public override void GetCameraPosition(ref Vector3 position, ref Quaternion rotation)
    {
        if (Controller.Player && Controller.PlayerMotor)
        {
            UpdateFocus();
            UpdateCamera(ref position, ref rotation);
        }
    }

    public void ResetCameraPosition()
    {
        if (Controller.Player)
        {
            m_focusPosition = Controller.Player.position;
            m_prevPlayerPosition = Controller.Player.position;
            m_currentYPosition = Controller.Player.position.y;
            m_currentLead = 0f;
            m_currentDepthLead = 0f;
        }
    }

    ////////////////////////////
    /// Private Helper Stuff ///
    ////////////////////////////
    private void UpdateFocus()
    {
        m_focusPosition = Controller.Player.position + GetLead();
        m_focusPosition.y = UpdateYPosition();

        m_prevPlayerPosition = Controller.Player.position;
    }

    private void UpdateCamera(ref Vector3 position, ref Quaternion rotation)
    {

        // Note: Assuming that the camera up will always just be Vector3.up
        Vector3 cameraOffset = Vector3.RotateTowards(m_camSettings.CameraForward * -1f, Vector3.up, Mathf.Deg2Rad * m_camSettings.AngleUp, 0f) * m_camSettings.FocusDistance;
        position = m_focusPosition + cameraOffset;

        // The camera should now be in the right position, so just have it look at the focus point.
        rotation = Quaternion.LookRotation(m_focusPosition - position);

        // Add in any camera restrictions at this point
        Vector3 clampedPosition = position;
        if (m_camSettings.RestrictZMovement)
        {
            clampedPosition.z = Mathf.Clamp(clampedPosition.z, m_camSettings.MinZPosition, m_camSettings.MaxZPosition);
        }
        if (m_camSettings.RestrictXMovement)
        {
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, m_camSettings.MinXPosition, m_camSettings.MaxXPosition);
        }
        position = clampedPosition;
    }

    private Vector3 GetLead()
    {
        // Get distance that the player just traveled
        Vector3 distance = (Controller.Player.position - m_prevPlayerPosition);

        // Getting a vector for the left/right direction and forward direction
        Vector3 offsetDirection = Quaternion.Euler(0, -90, 0) * m_camSettings.CameraForward;
        Vector3 depthDirection = m_camSettings.CameraForward;

        // Using dot product to figure out how much the player is moving in the left/right direction and forward/back
        float targetLead = Vector3.Dot(offsetDirection.normalized, distance.normalized);
        float targetDepthLead = Vector3.Dot(depthDirection.normalized, distance.normalized);

        // Getting the current lead speed based on the player speed
        if (m_camSettings.LeadDelay > 0)
        {
            float leadSpeed = (Controller.PlayerMotor.Velocity.magnitude * SPEED_LEAD_SCALE) / m_camSettings.LeadDelay;

            // Calculate the left/right lead
            float leadChange = targetLead - m_currentLead;
            m_currentLead += leadChange * leadSpeed * Time.deltaTime;
            m_currentLead = Mathf.Clamp(m_currentLead, -1f, 1f);
             // calculate the forward/back lead
            float leadDepthChange = targetDepthLead - m_currentDepthLead;
            m_currentDepthLead += leadDepthChange * leadSpeed * Time.deltaTime;
            m_currentDepthLead = Mathf.Clamp(m_currentDepthLead, -1f, 0f);
        }
        else
        {
            m_currentLead = targetLead;
            m_currentDepthLead = targetDepthLead;
        }

        return (offsetDirection * m_camSettings.LeadDistance * Mathf.SmoothStep(-1f, 1f, (m_currentLead + 1f) / 2f)) + (depthDirection * m_camSettings.DepthLeadDistance * Mathf.SmoothStep(-1f, 0f, (m_currentDepthLead + 1f) / 2f));
    }

    private float UpdateYPosition()
    {
        float newYPosition = Controller.Player.position.y;
        float distance = Mathf.Abs(m_currentYPosition - newYPosition);
        float maxHeightDiff = m_camSettings.MaxHeightDifference;
        if (Controller.PlayerMotor.GroundingStatus.FoundAnyGround || (newYPosition < m_currentYPosition) || (distance > maxHeightDiff))
        {
            if (!Controller.PlayerMotor.GroundingStatus.FoundAnyGround && distance > maxHeightDiff && newYPosition > m_currentYPosition)
            {
                distance -= maxHeightDiff;
                newYPosition -= maxHeightDiff;
            }
            float progress = Mathf.Sqrt(distance / maxHeightDiff);
            progress -= (Time.deltaTime / m_camSettings.HeightApproachTime);
            progress = Mathf.Clamp01(progress);

            float yDifference = Mathf.Pow(progress, 2f) * maxHeightDiff * Mathf.Sign(m_currentYPosition - newYPosition);
            m_currentYPosition = newYPosition + yDifference;
        }
        return m_currentYPosition;
    }
}
