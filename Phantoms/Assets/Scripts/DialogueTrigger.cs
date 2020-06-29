using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : Interactable
{
    [SerializeField]
    private DialogueManager m_dialogueManager = null;

    [TextArea]
    public string[] m_inputStrings;

    private PlayerController m_player;

    private bool m_inTrigger = false;
    private bool m_inDialogue = false;

    private int m_lineIndex = 0;

    public override void Interact()
    {
        if (m_inTrigger)
        {
            if (!m_inDialogue)
            {
                Debug.Log("Dialogue");
                if (m_player)
                {
                    m_player.SetState(new PlayerStateInteract());
                    m_inDialogue = true;
                    m_lineIndex = 0;
                }
            }
            else
            {
                Debug.Log("ContinueDialogue");
            }
            NextLine();
        }
    }

    private void NextLine()
    {
        if (m_dialogueManager.Revealing)
        {
            return;
        }

        if (m_lineIndex >= m_inputStrings.Length)
        {
            m_dialogueManager.Hide();
            m_inDialogue = false;
            m_player.SetState(new PlayerStateIdle());
        }
        else
        {
            m_dialogueManager.DisplayText(m_inputStrings[m_lineIndex]);
            m_lineIndex += 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            m_player = player;
            m_player.CurrentInteractable = this;
            m_inTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            m_player.CurrentInteractable = null;
            m_player = null;
            m_inTrigger = false;
        }
    }
}
