using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryAbility : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    private TMP_Text m_abilityNameText = null;
    [SerializeField]
    private TMP_Text m_abilityCostText = null;
    [SerializeField]
    private TMP_Text m_abilityDescriptionText = null;

    private Ares.Ability m_currentAbility = null;
    public Ares.Ability CurrentAbility
    {
        get { return m_currentAbility; }
        set { SetAbility(value); }
    }

    public void SetAbility(Ares.Ability ability)
    {
        if (ability == m_currentAbility) { return; }

        m_currentAbility = ability;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (m_currentAbility == null)
        {
            // TODO: Hide everything maybe?
            return;
        }
        
        // Not all of these UI references might be here, just update the ones that are.
        if (m_abilityNameText) { m_abilityNameText.text = m_currentAbility.Data.DisplayName; }
        if (m_abilityDescriptionText) { m_abilityDescriptionText.text = m_currentAbility.Data.Description; }
        if (m_abilityCostText)
        {
            int mpCost = m_currentAbility.Data.ManaCost;
            if (mpCost <= 0)
            {
                m_abilityCostText.gameObject.SetActive(false);
            }
            else
            {
                m_abilityCostText.gameObject.SetActive(true);
                m_abilityCostText.text = mpCost.ToString() + " MP";
            }
        }
    }

}
