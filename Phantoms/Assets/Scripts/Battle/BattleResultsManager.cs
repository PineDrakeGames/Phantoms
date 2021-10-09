using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    [Header("Level Up Stuff")]
    [SerializeField]
    private GameObject m_levelUpParent = null;
    [SerializeField]
    private TMP_Text m_levelUpNameText = null;
    [SerializeField]
    private Transform m_levelUpOptionsParent = null;
    [SerializeField]
    private GameObject m_levelUpOptionPrefab = null;

    [Header("Phantom Caught stuff")]
    [SerializeField]
    private GameObject m_phantomCaughtParent = null;
    [SerializeField]
    private TMP_Text m_phantomName = null;
    [SerializeField]
    private TMP_Text m_phantomDescription = null;
    [SerializeField]
    private Image m_phantomIconFill = null;
    [SerializeField]
    private Image m_phantomIconLines = null;
    [SerializeField]
    private TMP_InputField m_phantomNameInputField = null;
    [SerializeField]
    private Button m_SetNameButton = null;

    [HideInInspector]
    public PhantomInstanceData CaughtPhantom = null;


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

    // Private level up stuff
    private List<UserBattleInstanceData> m_queuedLevelUps = new List<UserBattleInstanceData>();

    private List<LevelUpOptionButton> m_levelUpOptionInstances = new List<LevelUpOptionButton>();


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

        switch (m_endReason)
        {
            case Battle.EndReason.PlayerWin:
                ShowExperience();
                break;
            case Battle.EndReason.PhantomCaught:
                ShowCaughtPhantom();
                break;
            case Battle.EndReason.EnemyWin:
            case Battle.EndReason.Ran:
            case Battle.EndReason.WinLoseConditionMet:
            case Battle.EndReason.OutOfTurns:
            default:
                ShowDefault();
                break;
        }
    }

    public void ShowDefault()
    {
        m_currentMenu = ResultMenu.DEFAULT;

        m_resultsParent.SetActive(true);
        m_defaultResultsParent.SetActive(true);

        string headerText = "";
        string descriptionText = "";
        switch (m_endReason)
        {
            case Battle.EndReason.PlayerWin:
                headerText = "You Won!";
                descriptionText = "Shouldn't see this menu in that case - let CJ know!";
                break;

            case Battle.EndReason.EnemyWin:
                headerText = "You Lost..";
                descriptionText = "You probably lost some money? Or have to reload at a prev save or something? Haven't set up this result yet. For now, gonna set you to 1 HP and return you.";
                break;

            case Battle.EndReason.Ran:
                headerText = "You Escaped!";
                descriptionText = "You might have lost some money? Haven't figured out how this works yet - just a way to leave a battle immediately.";
                break;

            case Battle.EndReason.PhantomCaught:
                headerText = "You Caught a Phantom!";
                descriptionText = "Shouldn't see this menu in that case - let CJ know!";
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

    public void ShowLevelUp(UserBattleInstanceData user)
    {
        m_currentMenu = ResultMenu.LEVELUP;

        m_resultsParent.SetActive(true);
        m_levelUpParent.SetActive(true);

        m_levelUpNameText.text = user.GetDisplayName();

        foreach (LevelUpOptionButton button in m_levelUpOptionInstances)
        {
            button.gameObject.SetActive(false);
        }

        foreach (BattleStatType statType in user.LevelUpOptions())
        {
            LevelUpOptionButton button = GetLevelUpOptionButton();
            button.SetButton(user, statType);
        }
    }

    public void ShowCaughtPhantom()
    {
        m_currentMenu = ResultMenu.PHANTOMCAUGHT;

        if (CaughtPhantom != null)
        {
            m_resultsParent.SetActive(true);
            m_phantomCaughtParent.SetActive(true);

            m_phantomName.text = CaughtPhantom.Data.DisplayName;
            m_phantomDescription.text = CaughtPhantom.Data.Description;
            
            if (CaughtPhantom.Data.IconFill)
            {
                m_phantomIconFill.gameObject.SetActive(true);
                m_phantomIconFill.sprite = CaughtPhantom.Data.IconFill;
            }
            else
            {
                m_phantomIconFill.gameObject.SetActive(false);
            }
            if (CaughtPhantom.Data.IconLines)
            {
                m_phantomIconLines.gameObject.SetActive(true);
                m_phantomIconLines.sprite = CaughtPhantom.Data.IconLines;
            }
            else
            {
                m_phantomIconLines.gameObject.SetActive(false);
            }

            m_phantomNameInputField.text = CaughtPhantom.GetDisplayName();
            m_SetNameButton.interactable = true;
        }
        else
        {
            AdvanceResults();
        }
    }

    public void OnPhantomNicknameUpdate()
    {
        m_SetNameButton.interactable = !(string.IsNullOrWhiteSpace(m_phantomNameInputField.text));
    }


    // This function is a bit of a mess, can do with some seperation into different functions
    public void AdvanceResults()
    {
        m_defaultResultsParent.SetActive(false);
        m_experienceResultsParent.SetActive(false);
        m_levelUpParent.SetActive(false);
        m_phantomCaughtParent.SetActive(false);

        switch (m_endReason)
        {
            case Battle.EndReason.PlayerWin:
                switch (m_currentMenu)
                {
                    case ResultMenu.DEFAULT:
                        ShowExperience();
                        break;

                    case ResultMenu.EXPERIENCE:
                        AdvanceExperienceMenu();
                        break;

                    case ResultMenu.LEVELUP:
                        AdvanceLevelUp();
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
                        ShowCaughtPhantom();
                        break;

                    case ResultMenu.EXPERIENCE:
                        AdvanceExperienceMenu();
                        break;

                    case ResultMenu.LEVELUP:
                        AdvanceLevelUp();
                        break;

                    case ResultMenu.PHANTOMCAUGHT:
                        CaughtPhantom.NickName = m_phantomNameInputField.text;
                        ShowExperience();
                        break;

                    case ResultMenu.NONE:
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

    ////////////////////////////////
    /// Private helper functions ///
    ////////////////////////////////
    private void AdvanceExperienceMenu()
    {
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
            m_experienceResultsParent.SetActive(true);
        }
        else
        {
            AdvanceLevelUp();
        }
    }

    private void AdvanceLevelUp()
    {
        if (m_queuedLevelUps.Count > 0)
        {
            UserBattleInstanceData user = m_queuedLevelUps[0];
            m_queuedLevelUps.RemoveAt(0);
            ShowLevelUp(user);
        }
        else
        {
            QuitBattle();
        }
    }

    private LevelUpOptionButton GetLevelUpOptionButton()
    {
        foreach (LevelUpOptionButton button in m_levelUpOptionInstances)
        {
            if (!button.gameObject.activeSelf)
            {
                button.gameObject.SetActive(true);
                return button;
            }
        }

        // Instantiate a new button
        GameObject buttonInstance = Instantiate(m_levelUpOptionPrefab, m_levelUpOptionsParent);
        LevelUpOptionButton buttonComponent = buttonInstance.GetComponent<LevelUpOptionButton>();
        buttonComponent.ResultsManager = this;
        m_levelUpOptionInstances.Add(buttonComponent);
        return buttonComponent;
    }
}
