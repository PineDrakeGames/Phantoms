using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private Camera m_camera = null;

    private Vector3 m_originalPosition = Vector3.zero;
    private float m_currentShakeTime = 0f;

    // This is all with the assumption that all screen shakes are created equal, and we want it to always act the same.
    private const float MAX_SHAKE_DURATION = 2f;
    private const float MAX_SHAKE_DISTANCE = 0.5f;

    // Start is called before the first frame update
    void Awake()
    {
        if (m_camera == null)
        {
            m_camera = Camera.main;
        }
        m_originalPosition = m_camera.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_currentShakeTime > 0f)
        {
            m_currentShakeTime -= Time.deltaTime;

            if (m_currentShakeTime <= 0)
            {
                m_camera.transform.localPosition = m_originalPosition;
            }
            else
            {
                float intensity = Mathf.Pow(Mathf.Clamp01(m_currentShakeTime / MAX_SHAKE_DURATION), 2f);
                float offsetX = MAX_SHAKE_DISTANCE * intensity * Random.Range(-1f, 1f);
                float offsetY = MAX_SHAKE_DISTANCE * intensity * Random.Range(-1f, 1f);
                Vector3 newPosition = m_originalPosition;
                // makes assumption of no rotation
                newPosition += new Vector3(offsetX, offsetY, 0f);
                m_camera.transform.localPosition = newPosition;
            }
        }
    }

    ////////////////////////
    /// Public Functions ///
    ////////////////////////

    public void SetShake(float shakeAmount)
    {
        m_currentShakeTime = Mathf.Clamp01(shakeAmount) * MAX_SHAKE_DURATION;
    }

    public void AddShake(float shakeAmount)
    {
        if (shakeAmount <= 0) { return; }
        m_currentShakeTime += shakeAmount * MAX_SHAKE_DURATION;
        m_currentShakeTime = Mathf.Clamp(m_currentShakeTime, 0f, MAX_SHAKE_DURATION);
    }
}
