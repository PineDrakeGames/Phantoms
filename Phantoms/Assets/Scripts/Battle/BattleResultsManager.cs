using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ares;
using TMPro;

public class BattleResultsManager : MonoBehaviour
{
    [Header("UI References!")]
    [SerializeField]
    private GameObject m_resultsParent = null;
    [SerializeField]
    private TextMeshProUGUI m_resultsHeaderText = null;
    [SerializeField]
    private TextMeshProUGUI m_resultsDescriptionText = null;

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Awake()
    {
        HideResults();
    }


    ////////////////////////
    /// Public Functions ///
    ////////////////////////
    public void HideResults()
    {
        m_resultsParent.SetActive(false);
    }

    public void ShowResults(Battle.EndReason endReason)
    {
        m_resultsParent.SetActive(true);
        string headerText = "";
        string descriptionText = "";
        switch (endReason)
        {
            case Battle.EndReason.PlayerWin:
                headerText = "You Won!";
                descriptionText = "This should be where it lists your XP and lets you pick level ups, but that's not set up yet so you get this text instead!";
                break;

            case Battle.EndReason.EnemyWin:
                headerText = "You Lost..";
                descriptionText = "You probably lost some money? Or have to reload at a prev save or something?";
                break;

            case Battle.EndReason.Ran:
                headerText = "You Escaped!";
                descriptionText = "You might have lost some money? Haven't figured out how this works yet.";
                break;

            case Battle.EndReason.PhantomCaught:
                headerText = "You Caught a Phantom!";
                descriptionText = "This is where I would normally add some information about the phantom you just caught, but I didn't do that yet!";
                break;

            case Battle.EndReason.WinLoseConditionMet:
                headerText = "You tied or something?";
                descriptionText = "Yeah this shouldn't really happen.";
                break;

            case Battle.EndReason.OutOfTurns:
                headerText = "You ran out of turns somehow! Didn't know we added this!";
                descriptionText = "You should probably let CJ know that this happened.";
                break;

            default:
                headerText = "Uhh whats goin on here";
                descriptionText = "This is uncharted territory...";
                break;
        }

        m_resultsHeaderText.text = headerText;
        m_resultsDescriptionText.text = descriptionText;
    }

    public void QuitBattle()
    {
        LoadingManager.ReturnFromBattle();
    }
}
