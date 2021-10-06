using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OverworldCameraSettings
{
    [Header("Player Camera Variables")]
    [SerializeField]
    private float m_focusDistance = 13f;
    [SerializeField]
    private Vector3 m_cameraForward = Vector3.forward;
    [SerializeField]
    private float m_angleUp = 12f;

    [Header("Camera height variables")]
    [SerializeField]
    private float m_maxHeightDifference = 3f;
    [SerializeField]
    private float m_heightApproachTime = 0.35f;

    [Header("Lead Player Variables")]
    [SerializeField]
    private float m_leadDistance = 1f;
    [SerializeField]
    private float m_depthLeadDistance = 2f;
    [SerializeField]
    private float m_leadDelay = 0.6f;

    [Header("Camera Restrictions")]
    [SerializeField]
    private bool m_restrictZMovement = true;
    [SerializeField]
    private float m_minZPosition = -17.21053f;
    [SerializeField]
    private float m_maxZPosition = -17.21053f;

    [SerializeField]
    private bool m_restrictXMovement = false;
    [SerializeField]
    private float m_minXPosition = -15f;
    [SerializeField]
    private float m_maxXPosition = 15f;

    // Public Getters
    public float FocusDistance { get { return m_focusDistance; } }
    public Vector3 CameraForward { get { return m_cameraForward.normalized; } }
    public float AngleUp { get { return m_angleUp; } }
    public float MaxHeightDifference { get { return m_maxHeightDifference; } }
    public float HeightApproachTime { get { return m_heightApproachTime; } }
    public float LeadDistance { get { return m_leadDistance; } }
    public float DepthLeadDistance { get { return m_depthLeadDistance; } }
    public float LeadDelay { get { return m_leadDelay; } }
    public bool RestrictZMovement { get { return m_restrictZMovement; } }
    public float MinZPosition { get { return m_minZPosition; } }
    public float MaxZPosition { get { return m_maxZPosition; } }
    public bool RestrictXMovement { get { return m_restrictXMovement; } }
    public float MinXPosition { get { return m_minXPosition; } }
    public float MaxXPosition { get { return m_maxXPosition; } }

    public OverworldCameraSettings()
    {

    }

    public OverworldCameraSettings(OverworldCameraSettings camSettings)
    {
        m_focusDistance = camSettings.FocusDistance;
        m_cameraForward = camSettings.CameraForward;
        m_angleUp = camSettings.AngleUp;
        m_maxHeightDifference = camSettings.MaxHeightDifference;
        m_heightApproachTime = camSettings.HeightApproachTime;
        m_leadDistance = camSettings.LeadDistance;
        m_depthLeadDistance = camSettings.DepthLeadDistance;
        m_leadDelay = camSettings.LeadDistance;
        m_restrictZMovement = camSettings.RestrictZMovement;
        m_minZPosition = camSettings.MinZPosition;
        m_maxZPosition = camSettings.MaxZPosition;
        m_restrictXMovement = camSettings.RestrictXMovement;
        m_minXPosition = camSettings.MinXPosition;
        m_maxXPosition = camSettings.MaxXPosition;
    }
}
