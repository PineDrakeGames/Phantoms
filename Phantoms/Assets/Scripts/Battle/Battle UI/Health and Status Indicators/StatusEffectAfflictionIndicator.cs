using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusEffectAfflictionIndicator : StatusEffectIndicator
{
     [Header("Affliction Display References")]
    [SerializeField]
    private TMP_Text m_afflictionDurationText = null;
    [SerializeField]
    private TMP_Text m_afflictionStageText = null;
    [SerializeField]
    private Image m_afflictionIcon = null;

    private Ares.Affliction m_affliction;

    public void SetAffliction(Ares.Affliction newAffliction)
    {
        if (m_affliction != null)
        {
            newAffliction.StageChange.RemoveListener(UpdateStage);
            newAffliction.RoundsRemainingChange.RemoveListener(UpdateTurnsRemaining);
        }
        m_affliction = newAffliction;

        m_afflictionIcon.sprite = newAffliction.Data.Icon;
        UpdateTurnsRemaining(newAffliction.RoundsRemaining);
        UpdateStage(newAffliction.Stage);

        newAffliction.StageChange.AddListener(UpdateStage);
        newAffliction.RoundsRemainingChange.AddListener(UpdateTurnsRemaining);
    }

    public void UpdateStage(int newStage)
    {
        if (m_affliction.Data.MinStage != m_affliction.Data.MaxStage)
        {
            m_afflictionStageText.text = newStage.ToString();
        }
        else
        {
            m_afflictionStageText.text = string.Empty;
        }
    }

    public void UpdateTurnsRemaining(int remainingTurns)
    {
        m_afflictionDurationText.text = remainingTurns.ToString();
        if (remainingTurns == 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
