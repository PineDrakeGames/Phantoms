using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ares;

public class StatusEffectBuffIndicator : StatusEffectIndicator
{
    [Header("Buff Data Display References")]
    [SerializeField]
    private TMP_Text m_buffDurationText = null;
    [SerializeField]
    private TMP_Text m_buffAmountText = null;
    [SerializeField]
    private Image m_statIcon = null;
    [SerializeField]
    private RectTransform m_statArrowParent = null;
    [SerializeField]
    private Image m_statArrowColor = null;

    [Header("Some stuff")]
    [SerializeField]
    private Color m_buffColor = Color.green;
    [SerializeField]
    private Color m_debuffColor = Color.red;

    private TemporaryBuff m_tempBuff;

    public void SetBuff(TemporaryBuff tempBuff)
    {
        m_tempBuff = tempBuff;

        StatData statData = tempBuff.Stat;

        m_statIcon.sprite = statData.StatIcon;

        if (tempBuff.Stages >= 0)
        {
            m_buffAmountText.text = "+" + tempBuff.Stages.ToString();
            m_statArrowParent.localRotation = Quaternion.Euler(0f, 0f, 0f);
            m_statArrowColor.color = m_buffColor;
        }
        else
        {
            m_buffAmountText.text = tempBuff.Stages.ToString();
            m_statArrowParent.localRotation = Quaternion.Euler(0f, 0f, 180f);
            m_statArrowColor.color = m_debuffColor;
        }

        m_buffDurationText.text = tempBuff.TurnsRemaining.ToString();
        tempBuff.TurnsRemainingUpdate.AddListener(UpdateTurnsRemaining);
    }

    public void UpdateTurnsRemaining(int remainingTurns)
    {
        m_buffDurationText.text = remainingTurns.ToString();
        if (remainingTurns == 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
