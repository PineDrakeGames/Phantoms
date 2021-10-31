using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RestoreIndicator : MonoBehaviour
{
    [SerializeField]
    private Animator m_restoreIndicatorAnimation = null;
    [SerializeField]
    private TextMeshPro m_restoreIndicatorText = null;

    [SerializeField]
    private Color m_restoreHealthColor = Color.red;
    [SerializeField]
    private Color m_restoreManaColor = Color.blue;

    private const float DAMAGE_INDICATOR_DURATION = 1.5f;
    private float m_currentTime = 0f;

    public bool Ready = true;

    private void Update()
    {
        if (!Ready)
        {
            m_currentTime -= Time.deltaTime;
            if (m_currentTime <= 0)
            {
                Ready = true;
            }
        }
    }

    public void SetRestoreIndicator(Vector3 position, int amount, bool isHealth = true)
    {
        transform.position = position + Vector3.up * 1.5f;
        m_restoreIndicatorText.text = "+" + amount.ToString();
        if (isHealth)
        {
            m_restoreIndicatorText.color = m_restoreHealthColor;
        }
        else
        {
            m_restoreIndicatorText.color = m_restoreManaColor;
        }
        m_restoreIndicatorAnimation.SetTrigger("Play");

        Ready = false;
        m_currentTime = DAMAGE_INDICATOR_DURATION;
    }
}
