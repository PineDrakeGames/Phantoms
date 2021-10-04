using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldSceneSettings : MonoBehaviour
{

    [Header("Music Stuff!")]
    [SerializeField]
    private string m_sceneMusicIntro = null;
    [SerializeField]
    private string m_sceneMusicLoop = null;

    [Header("Camera Settings")]
    [SerializeField]
    private OverworldCameraSettings m_cameraSettings = null;

    private void Awake()
    {
        if (!string.IsNullOrEmpty(m_sceneMusicLoop))
        {
            AudioManager.PlayMusic(m_sceneMusicLoop, m_sceneMusicIntro);
        }
    }

    private void Start()
    {
        if (m_cameraSettings != null)
        {
            OverworldManager.Instance.CamController.SetCameraSettings(m_cameraSettings);
        }
    }
}
