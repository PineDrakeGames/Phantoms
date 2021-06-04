using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class LevelUpOptionButton : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_levelUpOptionText = null;
    [SerializeField]
    private TMP_Text m_prevValueText = null;
    [SerializeField]
    private TMP_Text m_newValueText = null;

    [HideInInspector]
    public BattleResultsManager ResultsManager = null;

    private Button m_buttonComponent = null;
    private BattleStatType m_statType = BattleStatType.MAXHP;
    private UserBattleInstanceData m_user = null;

    // An added caution to make sure this button can't really be double clicked, to level up twice...
    private bool m_clicked = true;

    private void Awake()
    {
        m_buttonComponent = GetComponent<Button>();
        m_buttonComponent.onClick.AddListener(OnClick);
    }

    private void OnDestroy() {
        if (m_buttonComponent && m_buttonComponent.onClick != null)
        {
            m_buttonComponent.onClick.RemoveListener(OnClick);
        }
    }

    public void OnClick()
    {
        if (!m_clicked)
        {
            m_clicked = true;
            m_user.LevelUps.SetStat(m_statType, m_user.LevelUps.GetStat(m_statType) + 1);
            m_user.SetCurrentStats();

            if (ResultsManager)
            {
                ResultsManager.AdvanceResults();
            }
        }
    }

    public void SetButton(UserBattleInstanceData user, BattleStatType statType)
    {
        m_user = user;
        m_statType = statType;

        switch(statType)
        {
            case BattleStatType.MAXHP:
                m_levelUpOptionText.text = "Max HP Up";
                break;
            case BattleStatType.MANA:
                m_levelUpOptionText.text = "Max MP Up";
                break;
            case BattleStatType.ATTACK:
                m_levelUpOptionText.text = "Attack Up";
                break;
            case BattleStatType.DEFENSE:
                m_levelUpOptionText.text = "Defense Up";
                break;
            case BattleStatType.RELIC:
                m_levelUpOptionText.text = "Relic Up";
                break;
            default:
                m_levelUpOptionText.text = "CJ needs to set this stat up!";
                break;
        }
        
        m_prevValueText.text = user.CurrentStats.GetStat(statType).ToString();
        m_newValueText.text = (user.CurrentStats.GetStat(statType) + user.UserData.LevelUpAmounts.GetStat(statType)).ToString();

        m_clicked = false;
    }
}
