using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Simple script - set flag when you enter this trigger!

public class SetFlagTrigger : MonoBehaviour
{
    [SerializeField]
    private string m_flagName = null;

    [SerializeField]
    private bool m_disableObjectIfFlagSet = true;

    [SerializeField]
    private bool m_disableOnTrigger = true;

    private void Awake() {
        if (m_disableObjectIfFlagSet && SaveDataManager.CheckFlag(m_flagName))
        {
            this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            SaveDataManager.SetFlag(m_flagName, true);
            if (m_disableObjectIfFlagSet || m_disableOnTrigger)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
