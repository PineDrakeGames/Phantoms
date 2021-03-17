using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackTarget : MonoBehaviour
{
    [SerializeField]
    private bool m_onlyAttackOnce = false;

    public UnityEvent AttackedEvent = new UnityEvent();

    [Tooltip("For things that should only be attacked once, and we want to remain attacked on re-enabling.")]
    public UnityEvent OnAttackedReEnableEvent = new UnityEvent();


    private bool m_attacked = false;

    public void Attacked()
    {
        Debug.Log("Attacked " + name);
        if (!m_onlyAttackOnce || !m_attacked)
        {
            AttackedEvent.Invoke();
        }
        m_attacked = true;
    }

    private void OnEnable()
    {
        if (m_onlyAttackOnce && m_attacked)
        {
            OnAttackedReEnableEvent.Invoke();
        }
    }
}
