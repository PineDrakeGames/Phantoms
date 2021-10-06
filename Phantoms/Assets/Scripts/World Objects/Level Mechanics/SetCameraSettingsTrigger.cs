using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraSettingsTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField]
    private bool m_resetCameraOnExit = true;

    [Header("Cam Settings")]
    [SerializeField]
    private OverworldCameraSettings m_camSettings = new OverworldCameraSettings();

    private void Awake()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OverworldManager.Instance.CamController.SetCameraSettings(m_camSettings);
            Debug.Log("Updating camera settings!");
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && m_resetCameraOnExit)
        {
            OverworldManager.Instance.CamController.ResetToSceneDefault();
            Debug.Log("Resetting Camera state!");
        } 
    }
}
