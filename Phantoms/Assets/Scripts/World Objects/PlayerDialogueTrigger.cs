using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDialogueTrigger : MonoBehaviour
{
    [SerializeField]
    PixelCrushers.DialogueSystem.Wrappers.DialogueSystemTrigger m_dialogueTrigger = null;

    [Header("Trigger Settings")]
    [SerializeField]
    private bool m_disableWhenTriggered = true;

    [SerializeField]
    private bool m_saveTrigger = true;
    [SerializeField]
    private string m_triggerID = string.Empty;

    private bool m_triggered = false;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(m_triggerID))
        {
            m_triggerID = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name + "_" + gameObject.name;
        }
    }
#endif

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

        if (m_saveTrigger)
        {
            m_triggered = SaveDataManager.CheckFlag(m_triggerID);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            if (!m_disableWhenTriggered || !m_triggered)
            {
                m_dialogueTrigger.OnUse(OverworldManager.Instance.PlayerInstance.transform);
                PlayerController player = OverworldManager.Instance.PlayerController;
                player.SetState(new PlayerStateInteract());

                m_triggered = true;

                if (m_saveTrigger)
                {
                    SaveDataManager.SetFlag(m_triggerID, true);
                }
            }
        }
    }

    private void OnConversationEnd(Transform conversant)
    {
        PlayerController player = OverworldManager.Instance.PlayerController;
        player.SetState(new PlayerStateIdle());
    }
}
