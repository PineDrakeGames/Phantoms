using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerExperienceBox : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_playerName = null;

    [SerializeField]
    private TMP_Text m_currentExperienceText = null;

    [SerializeField]
    private Image m_currentExperienceMeter = null;

    [SerializeField]
    private GameObject m_levelUpText = null;

    // Private variables
    private bool m_filling = false;
    public bool Filling { get { return m_filling; } }
    private float m_fillTime = 0f;

    private float m_startExp = 0f;
    private float m_endExp = 0f;
    private float m_currentDisplayExp = 0f;
    private int m_targetLevels = 0;
    private int m_currentDisplayLevels = 0;

    private const float START_XP_PER_SECOND = 1f;
    private const float END_XP_PER_SECOND = 50f;
    private const float SPEEDUP_DURATION = 2.5f;

    private const float MAX_XP = 100f;

    // Start is called before the first frame update
    void Awake()
    {
        if (m_levelUpText)
        {
            m_levelUpText.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_filling)
        {
            m_fillTime += Time.deltaTime;
            float fillAmount = Mathf.Lerp(START_XP_PER_SECOND, END_XP_PER_SECOND, Mathf.Clamp01(m_fillTime / SPEEDUP_DURATION));

            m_currentDisplayExp += fillAmount * Time.deltaTime;

            // Check if finished filling
            if (m_currentDisplayLevels >= m_targetLevels && (m_currentDisplayExp >= m_endExp))
            {
                // Finished filling!
                m_currentDisplayExp = m_endExp;
                m_filling = false;
                SetExperienceDisplay();
                return;
            }

            // Check if level up
            while (m_currentDisplayExp >= 100f)
            {
                m_currentDisplayExp -= 100f;
                m_currentDisplayLevels += 1;
                m_levelUpText.SetActive(true);
            }

            SetExperienceDisplay();
        }
    }

    // Just needs the name and the experiences, assumes that if ending Exp is less than starting exp that it's because of a level up.
    // Uses floats for the meter fill stuff, will just round to int for the display.
    public void SetExpOverTime(string name, float startingExp, float endingExp, int numLevelUps)
    {
        m_startExp = startingExp;
        m_endExp = endingExp;
        m_currentDisplayExp = startingExp;

        m_targetLevels = numLevelUps;
        m_currentDisplayLevels = 0;

        m_filling = true;
        m_fillTime = 0f;

        m_playerName.text = name;
        m_levelUpText.SetActive(false);
        SetExperienceDisplay();
    }

    public void FinishSettingExp()
    {
        m_currentDisplayExp = m_endExp;

        m_currentDisplayLevels = m_targetLevels;

        m_filling = false;
        m_fillTime = 0f;

        if (m_currentDisplayLevels > 0)
        {
            m_levelUpText.SetActive(true);
        }
        else
        {
            m_levelUpText.SetActive(false);
        }
        SetExperienceDisplay();
    }

    private void SetExperienceDisplay()
    {
        m_currentExperienceText.text = Mathf.FloorToInt(m_currentDisplayExp).ToString();
        m_currentExperienceMeter.fillAmount = m_currentDisplayExp / MAX_XP;
    }
}
