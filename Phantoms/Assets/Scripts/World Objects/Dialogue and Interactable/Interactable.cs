using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    /// Serialized Stuff ///
    [Header("Default Interactable Requirements")]
    [SerializeField]
    private Transform m_interactIndicatorTarget = null;
    [SerializeField]
    private Vector3 m_interactIndicatorTargetOffset = Vector3.up;
    [SerializeField]
    private GameObject m_indicatorPrefab = null;

    /// Static things! ///
    private static GameObject s_indicator = null;


    protected bool m_isActive = true;
    protected bool m_playerInTrigger = false;

    public abstract void Interact();


    public void OnTriggerEnter(Collider other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();
        if (controller != null)
        {
            m_playerInTrigger = true;

            if (m_isActive && !(controller.CurrentState is PlayerStateInteract))
            {
                controller.CurrentInteractable = this;
                if (m_interactIndicatorTarget != null)
                {
                    SetIndicator();
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();
        if (controller != null)
        {
            m_playerInTrigger = false;
            if (controller.CurrentInteractable == this)
            {
                controller.CurrentInteractable = null;
                HideIndicator();
            }
        }
    }


    protected void SetIndicator()
    {
        Vector3 position = m_interactIndicatorTarget.position + m_interactIndicatorTargetOffset;
        if (s_indicator == null)
        {
            s_indicator = GameObject.Instantiate(m_indicatorPrefab);
            DontDestroyOnLoad(s_indicator);
        }

        s_indicator.gameObject.SetActive(true);
        s_indicator.transform.position = position;
    }

    protected void HideIndicator()
    {
        if (s_indicator != null)
        {
            s_indicator.gameObject.SetActive(false);
        }
    }
}
