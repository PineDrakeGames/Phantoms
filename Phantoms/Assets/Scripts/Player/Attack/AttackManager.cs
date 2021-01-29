using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public static List<AttackTarget> AttackedTargets = new List<AttackTarget>();

    public static void StartAttack()
    {
        AttackedTargets.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check both the object with the collider itself, the parent and then the children.
        AttackTarget attackedTarget = other.GetComponent<AttackTarget>();
        if (attackedTarget == null) { attackedTarget = other.GetComponentInParent<AttackTarget>(); }
        if (attackedTarget == null) { attackedTarget = other.GetComponentInChildren<AttackTarget>(); }
        if (attackedTarget != null && !AttackedTargets.Contains(attackedTarget))
        {
            AttackedTargets.Add(attackedTarget);
            attackedTarget.Attacked();
        }
    }
}
