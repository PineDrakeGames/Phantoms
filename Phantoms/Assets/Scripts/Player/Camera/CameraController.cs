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
            myScript.ResetToSceneDefault();
        }
    }
}
#endif

public class CameraController : MonoBehaviour
{
    /////////////////////////
    /// Serialized Fields ///
    /////////////////////////
    [Header("Default Camera Settings")]
    [SerializeField]
    private OverworldCameraSettings m_defaultSettings;

    [HideInInspector]
    public Transform Player = null;

    [HideInInspector]
    public KinematicCharacterMotor PlayerMotor;

    //////////////////////////
    /// Camera State Stuff ///
    //////////////////////////

    private PlayerCameraState m_sceneDefaultState = null;

    private PlayerCameraState m_currentState = null;
    private PlayerCameraState m_prevState = null;

    private const float DEFAULT_TRANSITION_TIME = 0.5f;

    private bool m_transitioning = false;
    private float m_transitionDuration = 0.5f;
    private float m_currentTransitionTime = 0f;
    private Vector3 m_startTransitionPosition = Vector3.zero;
    private Quaternion m_startTransitionRotation = Quaternion.identity;

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////

    private void Awake()
    {
        if (m_sceneDefaultState == null)
        {
            SetSceneDefaultState(m_defaultSettings);
        }
    }

    private void LateUpdate()
    {
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;
        if (m_currentState != null)
        {
            m_currentState.GetCameraPosition(ref position, ref rotation);
        }

        if (m_transitioning)
        {
            m_currentTransitionTime += Time.deltaTime;
            if (m_currentTransitionTime >= m_transitionDuration)
            {
                m_transitioning = false;
            }
            else
            {
                float progress = Mathf.Clamp01(m_currentTransitionTime / m_transitionDuration);
                position = Vector3.Lerp(m_startTransitionPosition, position, progress);
                rotation = Quaternion.Slerp(m_startTransitionRotation, rotation, progress);
            }
        }

        transform.position = position;
        transform.rotation = rotation;
    }

    ////////////////////////
    /// Public Functions ///
    ////////////////////////

    public void SetSceneDefaultState(OverworldCameraSettings newSettings)
    {
        SetCameraSettings(newSettings, 0f);
        m_sceneDefaultState = m_currentState;
    }

    public void ResetToSceneDefault(float transitionTime = DEFAULT_TRANSITION_TIME)
    {
        SetCameraState(m_sceneDefaultState, transitionTime);
    }

    public void SetCameraSettings(OverworldCameraSettings newSettings, float transitionTime = DEFAULT_TRANSITION_TIME)
    {
        if (newSettings != null)
        {
            PlayerCamStateDefault newState = new PlayerCamStateDefault(this, new OverworldCameraSettings(newSettings));
            SetCameraState(newState, transitionTime);
        }
    }

    public void SetCameraStateImmediate(PlayerCameraState newState)
    {
        SetCameraState(newState, 0f);
    }

    public void SetCameraState(PlayerCameraState newState, float transitionTime = DEFAULT_TRANSITION_TIME)
    {
        m_prevState = m_currentState;
        m_currentState = newState;
        if (transitionTime <= 0f)
        {
            return;
        }

        m_startTransitionPosition = transform.position;
        m_startTransitionRotation = transform.rotation;
        m_transitioning = true;
        m_transitionDuration = transitionTime;
        m_currentTransitionTime = 0f;
    }
}
