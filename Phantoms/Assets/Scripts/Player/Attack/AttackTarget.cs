using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackTarget : MonoBehaviour
{
    [SerializeField]
    private bool m_onlyAttackOnce = false;

    public UnityEvent<AttackTarget> AttackedEvent = new UnityEvent<AttackTarget>();

    [Tooltip("For things that should only be attacked once, and we want to remain attacked on re-enabling.")]
    public UnityEvent<AttackTarget> OnAttackedReEnableEvent = new UnityEvent<AttackTarget>();


    private bool m_attacked = false;

    public void Attacked()
    {
        Debug.Log("Attacked " + name);
        if (!m_onlyAttackOnce || !m_attacked)
        {
            AttackedEvent.Invoke(this);
        }
        m_attacked = true;
    }

    private void OnEnable()
    {
        if (m_onlyAttackOnce && m_attacked)
        {
            OnAttackedReEnableEvent.Invoke(this);
        }
    }
}
