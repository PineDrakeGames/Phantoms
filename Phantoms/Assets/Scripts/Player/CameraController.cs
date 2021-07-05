using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CameraController))]
public class CameraControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CameraController myScript = (CameraController)target;
        if (GUILayout.Button("Reset Camera"))
        {
            myScript.ResetCameraSettings();
        }
    }
}
#endif

public class CameraController : MonoBehaviour
{
    [Header("Default Camera Settings")]
    [SerializeField]
    private OverworldCameraSettings m_defaultSettings;

    [HideInInspector]
    public Transform Player = null;

    [HideInInspector]
    public KinematicCharacterMotor PlayerMotor;

    private OverworldCameraSettings m_camSettings = null;

    // Variables used to calculate how much the camera should lead the player
    private float m_currentLead = 0f;
    private Vector3 m_prevPlayerPosition = Vector3.zero;

    // Variables used to set the Y position of the player.
    private float m_currentYPosition = 0f;

    // The focus position of the camera.
    private Vector3 m_focusPosition = Vector3.zero;

    private const float SPEED_LEAD_SCALE = 0.1f;

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////

    private void Awake()
    {
        if (m_camSettings == null)
        {
            m_camSettings = m_defaultSettings;
        }
    }

    private void Start()
    {
        ResetCameraPosition();
    }

    private void LateUpdate()
    {
        if (Player && PlayerMotor)
        {
            // First get camera focus position
            UpdateFocus();
            // Then set the camera position based on the focus.
            UpdateCamera();
        }
    }

    ////////////////////////
    /// Public Functions ///
    ////////////////////////
    public void ResetCameraPosition()
    {
        if (Player)
        {
            m_focusPosition = Player.position;
            m_prevPlayerPosition = Player.position;
            m_currentYPosition = Player.position.y;
            m_currentLead = 0f;
            UpdateCamera();
        }
    }

    public void SetCameraSettings(OverworldCameraSettings newSettings)
    {
        if (newSettings != null)
        {
            m_camSettings = new OverworldCameraSettings(newSettings);
            ResetCameraPosition();
        }
    }

    public void ResetCameraSettings()
    {
        if (m_defaultSettings != null)
        {
            m_camSettings = new OverworldCameraSettings(m_defaultSettings);
            ResetCameraPosition();
        }
    }

    ////////////////////////////////
    /// Private Helper Functions ///
    ////////////////////////////////
    private void UpdateFocus()
    {
        m_focusPosition = Player.position + GetLead();
        m_focusPosition.y = UpdateYPosition();

        m_prevPlayerPosition = Player.position;
    }

    private void UpdateCamera()
    {

        // Note: Assuming that the camera up will always just be Vector3.up
        Vector3 cameraOffset = Vector3.RotateTowards(m_camSettings.CameraForward * -1f, Vector3.up, Mathf.Deg2Rad * m_camSettings.AngleUp, 0f) * m_camSettings.FocusDistance;
        transform.position = m_focusPosition + cameraOffset;

        // The camera should now be in the right position, so just have it look at the focus point.
        transform.LookAt(m_focusPosition);

        // Add in any camera restrictions at this point
        Vector3 clampedPosition = transform.position;
        if (m_camSettings.RestrictZMovement)
        {
            clampedPosition.z = Mathf.Clamp(clampedPosition.z, m_camSettings.MinZPosition, m_camSettings.MaxZPosition);
        }
        if (m_camSettings.RestrictXMovement)
        {
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, m_camSettings.MinXPosition, m_camSettings.MaxXPosition);
        }
        transform.position = clampedPosition;
    }


    /// Private helper functions ///
    private Vector3 GetLead()
    {
        Vector3 distance = (Player.position - m_prevPlayerPosition);
        Vector3 offsetDirection = Quaternion.Euler(0, -90, 0) * m_camSettings.CameraForward;

        float targetLead = Vector3.Dot(offsetDirection.normalized, distance.normalized);

        float leadSpeed = (PlayerMotor.Velocity.magnitude * SPEED_LEAD_SCALE) / m_camSettings.LeadDelay;

        float leadChange = targetLead - m_currentLead;
        m_currentLead += leadChange * leadSpeed * Time.deltaTime;

        m_currentLead = Mathf.Clamp(m_currentLead, -1f, 1f);

        return (offsetDirection * m_camSettings.LeadDistance * Mathf.SmoothStep(-1f, 1f, (m_currentLead + 1f) / 2f));
    }

    private float UpdateYPosition()
    {
        float newYPosition = Player.position.y;
        float distance = Mathf.Abs(m_currentYPosition - newYPosition);
        float maxHeightDiff = m_camSettings.MaxHeightDifference;
        if (PlayerMotor.GroundingStatus.FoundAnyGround || (newYPosition < m_currentYPosition) || (distance > maxHeightDiff))
        {
            if (!PlayerMotor.GroundingStatus.FoundAnyGround && distance > maxHeightDiff && newYPosition > m_currentYPosition)
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
