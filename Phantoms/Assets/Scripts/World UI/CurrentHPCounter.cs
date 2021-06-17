using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrentHPCounter : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The text for displaying current health")]
    private TMP_Text m_currentHPText = null;

    [SerializeField]
    [Tooltip("The text for displaying max health")]
    private TMP_Text m_maxHPText = null;

    private PlayerBattleInstanceData playerData;

    // Start is called before the first frame update
    void Start()
    {
        DataManager.PlayerHPChange.AddListener(CurrentHPChange);
        
        if (playerData == null)
        {
            playerData = DataManager.Instance.GetPlayerBattleInstanceData();
        }
        playerData.OnStatsUpdate.AddListener(OnPlayerStatChange);

        SetHPIndicator();
    }

    private void OnDestroy()
    {
        if (DataManager.PlayerHPChange != null)
        {
            DataManager.PlayerHPChange.RemoveListener(CurrentHPChange);
        }
        if (playerData != null && playerData.OnStatsUpdate != null)
        {
            playerData.OnStatsUpdate.RemoveListener(OnPlayerStatChange);
        }
    }

    // Called to change the drop total to a new specific amount
    public void CurrentHPChange(int newPlayerHP)
    {
        SetHPIndicator();
    }

    public void OnPlayerStatChange()
    {
        SetHPIndicator();
    }


    private void SetHPIndicator()
    {
        if (playerData == null)
        {
            playerData = DataManager.Instance.GetPlayerBattleInstanceData();
        }
        
        if (m_currentHPText != null)
        {
            m_currentHPText.text = playerData.CurrentHP.ToString();
        }
        if (m_maxHPText != null)
        {
            m_maxHPText.text = playerData.CurrentStats.MaxHP.ToString();
        }
    }
}
