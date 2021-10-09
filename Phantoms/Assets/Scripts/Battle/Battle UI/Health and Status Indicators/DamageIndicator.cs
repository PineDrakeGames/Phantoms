using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField]
    private Animator m_damageIndicatorAnimation = null;
    [SerializeField]
    private TextMeshPro m_damageIndicatorText = null;

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

    public void SetDamageIndicator(Vector3 position, int damage, float modifier = 1f)
    {
        transform.position = position;
        m_damageIndicatorText.text = damage.ToString();
        if (modifier <= 0.5f)
        {
            m_damageIndicatorAnimation.SetTrigger("Weak");
        }
        else if (modifier >= 2f)
        {
            m_damageIndicatorAnimation.SetTrigger("Strong");
        }
        else
        {
            m_damageIndicatorAnimation.SetTrigger("Play");
        }

        Ready = false;
        m_currentTime = DAMAGE_INDICATOR_DURATION;
    }
}
