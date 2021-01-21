using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackTarget : MonoBehaviour
{
    public UnityEvent AttackedEvent = new UnityEvent();

    public void Attacked()
    {
        Debug.Log("Attacked " + name);
        AttackedEvent.Invoke();
    }
}
