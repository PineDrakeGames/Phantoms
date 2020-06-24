using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Player Camera Variables")]
    [SerializeField]
    private float m_distanceFromFocus = 10f;
    [SerializeField]
    private Vector3 m_cameraForward = Vector3.forward;
    [SerializeField]
    private float m_angleUp = 30f;

    [Header("Lead Player Variables")]
    [SerializeField]
    private float m_leadDistance = 2f;
    [SerializeField]
    private float m_leadDelay = 3f;


    [HideInInspector]
    public Transform Player = null;


    private float m_currentLead = 0f;
    private Vector3 m_prevPlayerPosition = Vector3.zero;
    private Vector3 m_focusPosition = Vector3.zero;

    private void OnValidate() {
        m_cameraForward.y = 0;
        m_cameraForward.Normalize();
    }

    private void Start() 
    {
        m_prevPlayerPosition = Player.position;
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
        Vector3 distance = (Player.position - m_prevPlayerPosition);
        Vector3 offsetDirection =  Quaternion.Euler(0, -90, 0) * m_cameraForward;

        float lead = Vector3.Dot(offsetDirection, distance) / m_leadDelay;
        m_currentLead += lead;
        m_currentLead = Mathf.Clamp(m_currentLead, -1f, 1f);

        m_focusPosition = Player.position + (offsetDirection * m_leadDistance * Mathf.SmoothStep(-1f, 1f, (m_currentLead + 1f) / 2f));

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
}
