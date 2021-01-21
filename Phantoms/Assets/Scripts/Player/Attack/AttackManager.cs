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
        AttackTarget attackedTarget = other.GetComponent<AttackTarget>();
        if (attackedTarget != null && !AttackedTargets.Contains(attackedTarget))
        {
            AttackedTargets.Add(attackedTarget);
            attackedTarget.Attacked();
        }
    }
}
