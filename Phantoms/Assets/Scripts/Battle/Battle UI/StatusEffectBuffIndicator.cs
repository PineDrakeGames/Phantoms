using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusEffectBuffIndicator : StatusEffectIndicator
{
    [Header("Buff Data Display")]
    [SerializeField]
    private TMP_Text m_buffDurationText = null;
    [SerializeField]
    private TMP_Text m_buffAmountText = null;
}
