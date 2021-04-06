using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldSceneSettings : MonoBehaviour
{

    [Header("Camera Settings")]
    [SerializeField]
    private OverworldCameraSettings m_cameraSettings = null;

    // Start is called before the first frame update
    void Start()
    {
        if (m_cameraSettings != null)
        {
            OverworldManager.Instance.CamController.SetCameraSettings(m_cameraSettings);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
