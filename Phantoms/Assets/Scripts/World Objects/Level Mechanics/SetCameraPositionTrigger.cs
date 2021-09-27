using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraPositionTrigger : MonoBehaviour
{
    [Header("Prefab References")]
    [SerializeField]
    private Transform m_cameraTargetPosition = null;

    private PlayerCamStateFixedPos m_camState = null;

    private void Awake()
    {
        m_camState = new PlayerCamStateFixedPos();
        m_camState.FixedPosition = m_cameraTargetPosition.position;
        m_camState.FixedRotation = m_cameraTargetPosition.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OverworldManager.Instance.CamController.SetCameraState(m_camState);
            Debug.Log("Setting camera to fixed position!");
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OverworldManager.Instance.CamController.ResetCameraState();
            Debug.Log("Resetting Camera state!");
        } 
    }
}
