using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PixelCrushers.DialogueSystem.Wrappers.DialogueSystemTrigger))]
public class InteractableDialogueTrigger : Interactable
{
    [Header("Dialogue References")]
    [SerializeField]
    PixelCrushers.DialogueSystem.Wrappers.DialogueSystemTrigger m_dialogueTrigger = null;

    private void Start()
    {
        PixelCrushers.DialogueSystem.DialogueManager.instance.conversationEnded += OnConversationEnd;
        PixelCrushers.DialogueSystem.DialogueManager.instance.conversationStarted += OnConversationStart;
    }

    private void OnDestroy()
    {
        PixelCrushers.DialogueSystem.DialogueManager.instance.conversationEnded -= OnConversationEnd;
    }

    public override void Interact()
    {
        PixelCrushers.DialogueSystem.DialogueManager.instance.conversationEnded += OnTriggeredDialogueEnd;
        m_dialogueTrigger.OnUse(OverworldManager.Instance.PlayerInstance.transform);
        PlayerController player = OverworldManager.Instance.PlayerController;
        player.SetState(new PlayerStateInteract());
        HideIndicator();
    }

    private void OnConversationStart(Transform conversant)
    {
        m_isActive = false;
    }

    private void OnConversationEnd(Transform conversant)
    {
        m_isActive = true;
    }


    private void OnTriggeredDialogueEnd(Transform conversant)
    {
        PlayerController player = OverworldManager.Instance.PlayerController;
        player.SetState(new PlayerStateIdle());
        PixelCrushers.DialogueSystem.DialogueManager.instance.conversationEnded -= OnTriggeredDialogueEnd;
    }
}
