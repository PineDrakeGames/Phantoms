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

    [Header("Default Results Items")]
    [SerializeField]
    private GameObject m_defaultResultsParent = null;
    [SerializeField]
    private TextMeshProUGUI m_defaultResultsHeaderText = null;
    [SerializeField]
    private TextMeshProUGUI m_defaultResultsDescriptionText = null;

    [Header("Experience earned stuff")]
    [SerializeField]
    private GameObject m_experienceResultsParent = null;
    [SerializeField]
    private Transform m_experienceRewardsParent = null;
    [SerializeField]
    private PlayerExperienceBox m_keeperReward = null;
    [SerializeField]
    private GameObject m_experienceRewardPrefab = null;

    private enum ResultMenu
    {
        NONE,
        DEFAULT,
        EXPERIENCE,
        LEVELUP,
        PHANTOMCAUGHT
    }

    private ResultMenu m_currentMenu = ResultMenu.NONE;
    private Battle.EndReason m_endReason = Battle.EndReason.WinLoseConditionMet;

    private List<PlayerExperienceBox> m_experienceBoxes = new List<PlayerExperienceBox>();

    private List<UserBattleInstanceData> m_queuedLevelUps = new List<UserBattleInstanceData>();


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
        // Always start with the default menu, at least for now?
        m_currentMenu = ResultMenu.DEFAULT;
        m_endReason = endReason;

        m_resultsParent.SetActive(true);
        m_defaultResultsParent.SetActive(true);
        m_experienceResultsParent.SetActive(false);

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

        m_defaultResultsParent.SetActive(true);
        m_defaultResultsHeaderText.text = headerText;
        m_defaultResultsDescriptionText.text = descriptionText;
    }

    public void ShowExperience()
    {
        m_currentMenu = ResultMenu.EXPERIENCE;

        m_resultsParent.SetActive(true);
        m_defaultResultsParent.SetActive(false);
        m_experienceResultsParent.SetActive(true);

        // Set all the experience stuff!
        // Going to instantiate an experience display for each phantom, going to assume this doesn't need to be pooled as it should only be shown once?
        m_experienceBoxes.Add(m_keeperReward);

        Dictionary<UserBattleInstanceData, int> experienceRewards = BattleManager.Instance.ExperienceReward;
        foreach (UserBattleInstanceData player in experienceRewards.Keys)
        {
            // NOTE(CJ): THIS IS WHERE THE ACTUAL LEVEL UP AND EXPERIENCE REWARDING IS!
            int newExperience = player.Experience + experienceRewards[player];
            int numLevelUps = 0;
            while (newExperience >= 100)
            {
                newExperience -= 100;
                numLevelUps += 1;
                m_queuedLevelUps.Add(player);
            }

            if (player is PlayerBattleInstanceData)
            {
                m_keeperReward.SetExpOverTime(player.GetDisplayName(), player.Experience, newExperience, numLevelUps);
            }
            else
            {
                GameObject newRewardInstance = Instantiate(m_experienceRewardPrefab, m_experienceRewardsParent);
                PlayerExperienceBox experienceBox = newRewardInstance.GetComponent<PlayerExperienceBox>();
                experienceBox.SetExpOverTime(player.GetDisplayName(), player.Experience, newExperience, numLevelUps);
                m_experienceBoxes.Add(experienceBox);
            }

            player.Experience = newExperience;
            player.Level += numLevelUps;
        }
    }


    // This function is a bit of a mess, can do with some seperation into different functions
    public void AdvanceResults()
    {
        switch (m_endReason)
        {
            case Battle.EndReason.PlayerWin:
                switch (m_currentMenu)
                {
                    case ResultMenu.DEFAULT:
                        ShowExperience();
                        break;

                    case ResultMenu.EXPERIENCE:
                        bool stillFilling = false;
                        foreach (PlayerExperienceBox experienceBox in m_experienceBoxes)
                        {
                            if (experienceBox.Filling)
                            {
                                stillFilling = true;
                            }
                        }
                        if (stillFilling)
                        {
                            foreach (PlayerExperienceBox experienceBox in m_experienceBoxes)
                            {
                                experienceBox.FinishSettingExp();
                            }
                        }
                        else
                        {
                            QuitBattle();
                        }
                        break;

                    case ResultMenu.LEVELUP:
                        // TODO!
                        QuitBattle();
                        break;

                    case ResultMenu.NONE:
                    case ResultMenu.PHANTOMCAUGHT:
                    default:
                        QuitBattle();
                        break;
                }
                break;

            case Battle.EndReason.PhantomCaught:
                switch (m_currentMenu)
                {
                    case ResultMenu.DEFAULT:
                        ShowExperience();
                        break;

                    case ResultMenu.EXPERIENCE:
                        bool stillFilling = false;
                        foreach (PlayerExperienceBox experienceBox in m_experienceBoxes)
                        {
                            if (experienceBox.Filling)
                            {
                                stillFilling = true;
                            }
                        }
                        if (stillFilling)
                        {
                            foreach (PlayerExperienceBox experienceBox in m_experienceBoxes)
                            {
                                experienceBox.FinishSettingExp();
                            }
                        }
                        else
                        {
                            QuitBattle();
                        }
                        break;

                    case ResultMenu.LEVELUP:
                        // TODO!
                        QuitBattle();
                        break;

                    case ResultMenu.NONE:
                    case ResultMenu.PHANTOMCAUGHT:
                    default:
                        QuitBattle();
                        break;
                }
                break;

            case Battle.EndReason.EnemyWin:
            case Battle.EndReason.Ran:
            case Battle.EndReason.WinLoseConditionMet:
            case Battle.EndReason.OutOfTurns:
            default:
                QuitBattle();
                break;
        }
    }

    public void QuitBattle()
    {
        LoadingManager.ReturnFromBattle();
    }
}
