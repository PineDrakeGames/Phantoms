using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryRelicTick : MonoBehaviour
{
    [SerializeField]
    private GameObject m_tickFill = null;

    public void SetTick(bool available)
    {
        m_tickFill.SetActive(available);
    }
}
