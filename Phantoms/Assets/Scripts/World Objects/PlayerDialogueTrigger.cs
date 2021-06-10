using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDialogueTrigger : MonoBehaviour
{
    [SerializeField]
    PixelCrushers.DialogueSystem.Wrappers.DialogueSystemTrigger m_dialogueTrigger = null;

    private void Awake()
    {
        if (!m_dialogueTrigger)
        {
            m_dialogueTrigger = GetComponent<PixelCrushers.DialogueSystem.Wrappers.DialogueSystemTrigger>();
        }
        if (m_dialogueTrigger)
        {
            m_dialogueTrigger.trigger = PixelCrushers.DialogueSystem.DialogueSystemTriggerEvent.OnUse;
        }

        PixelCrushers.DialogueSystem.DialogueManager.instance.conversationEnded += OnConversationEnd;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            m_dialogueTrigger.OnUse(OverworldManager.Instance.PlayerInstance.transform);
            PlayerController player = OverworldManager.Instance.PlayerController;
            player.SetState(new PlayerStateInteract());

            
        }
    }

    private void OnConversationEnd(Transform conversant)
    {
        PlayerController player = OverworldManager.Instance.PlayerController;
        player.SetState(new PlayerStateIdle());
    }
}
