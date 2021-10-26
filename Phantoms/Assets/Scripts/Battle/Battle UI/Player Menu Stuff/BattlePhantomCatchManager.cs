using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattlePhantomCatchManager : MonoBehaviour
{
    [Header("Main Scene References")]
    [SerializeField]
    private BattleManager m_battleManager = null;


    [Header("Wager References")]
    [SerializeField]
    private GameObject m_menuParent = null;
    [SerializeField]
    private TMP_InputField m_healthWagerAmount = null;
    [SerializeField]
    private Button m_addButton = null;
    [SerializeField]
    private Button m_subtractButton = null;
    [SerializeField]
    private Image m_heartFillImage = null;

    [Header("Heart References")]
    [SerializeField]
    private GameObject m_heartObject = null;
    [SerializeField]
    private Animator m_heartAnimator = null;
    [SerializeField]
    private Vector3 m_heartMoveToPosition = Vector3.zero;

    [Header("Waiting for response references")]
    [SerializeField]
    private CanvasGroup m_responsesBox = null;
    [SerializeField]
    private FancyText m_responseBoxText = null;

    [Header("Press and hold values")]
    [SerializeField]
    private float m_holdDelay = 0.7f;
    [SerializeField]
    private float m_holdInterval = 0.2f;
    [SerializeField]
    private int m_holdAmount = 5;

    private int m_currentWager = 1;
    public int CurrentWager
    {
        get { return m_currentWager; }
        set
        {
            m_currentWager = value;
            UpdateUI();
        }
    }

    private int m_playerCurrentHealth = 99;

    private bool m_increaseHeld = false;
    private bool m_decreaseHeld = false;
    private float m_holdTime = 0f;

    private float m_currentHeartFillPercentage = 0f;
    private float m_targetHeartFillPercentage = 0f;
    private float m_currentHeartFillSpeed = 0f;

    private Ares.Actor m_playerActor;
    private Ares.ActionInput m_actionInput;

    private Vector3 m_heartStartingPos = Vector3.zero;

    private const float TIME_BETWEEN_PULSES = 1.1f;
    private const float MOVE_HEART_DURATION = 2f;
    private const float RESULTS_BOX_FADEINTIME = 0.5f;
    private const float RESULTS_WATCH_DURATION = 2f;
    private const float FADE_OUT_TIME = 0.7f;

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Awake()
    {
        m_healthWagerAmount.onValueChanged.AddListener(OnValueChange);
        m_healthWagerAmount.onEndEdit.AddListener(OnEndEdit);
        m_menuParent.SetActive(false);
        m_responsesBox.gameObject.SetActive(false);
        m_heartObject.gameObject.SetActive(false);
        m_heartStartingPos = m_heartObject.GetComponent<RectTransform>().anchoredPosition;
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    private void OnDestroy()
    {
        m_healthWagerAmount.onValueChanged.RemoveListener(OnValueChange);
        m_healthWagerAmount.onEndEdit.RemoveListener(OnEndEdit);
    }


    // Update is called once per frame
    void Update()
    {
        if (m_increaseHeld || m_decreaseHeld)
        {
            m_holdTime += Time.deltaTime;

            if (m_holdTime >= m_holdDelay)
            {
                if (m_increaseHeld)
                {
                    CurrentWager += m_holdAmount;
                }
                else if (m_decreaseHeld)
                {
                    CurrentWager -= m_holdAmount;
                }
                m_holdTime -= m_holdInterval;
            }
        }

        if (m_currentHeartFillPercentage != m_targetHeartFillPercentage)
        {
            m_currentHeartFillPercentage = Mathf.SmoothDamp(m_currentHeartFillPercentage, m_targetHeartFillPercentage, ref m_currentHeartFillSpeed, 0.1f, 10f);
            m_heartFillImage.fillAmount = m_currentHeartFillPercentage;
        }

    }

    ////////////////////////
    /// Public Functions ///
    ////////////////////////
    public void StartCatch(Ares.Actor playerActor, Ares.ActionInput actionInput)
    {
        m_menuParent.SetActive(true);
        m_heartObject.SetActive(true);
        m_heartObject.GetComponent<RectTransform>().anchoredPosition = m_heartStartingPos;
        CurrentWager = 1;
        m_playerCurrentHealth = playerActor.HP;
        m_playerActor = playerActor;
        m_actionInput = actionInput;
        SetHeartTargetFill();
    }

    public void StopCatching()
    {
        m_menuParent.SetActive(false);
        m_heartObject.SetActive(false);
        m_responsesBox.gameObject.SetActive(false);
        m_playerActor.GetComponentInChildren<Animator>().SetTrigger("Idle");
    }


    ////////////////////////////////
    /// Unity UI Event Functions ///
    ////////////////////////////////
    public void AddOne()
    {
        CurrentWager += 1;
    }
    public void SubtractOne()
    {
        CurrentWager -= 1;
    }

    public void PointerDownIncrease()
    {
        AddOne();
        m_increaseHeld = true;
        m_holdTime = 0f;
    }
    public void PointerUpIncrease()
    {
        m_increaseHeld = false;
    }
    public void PointerDownDecrease()
    {
        SubtractOne();
        m_decreaseHeld = true;
        m_holdTime = 0f;
    }
    public void PointerUpDecrease()
    {
        m_decreaseHeld = false;
    }

    public void OnValueChange(string newValue)
    {
        int parsedValue = 0;
        if (int.TryParse(newValue, out parsedValue))
        {
            CurrentWager = parsedValue;
        }
    }

    public void OnEndEdit(string newValue)
    {
        int parsedValue = 0;
        if (int.TryParse(newValue, out parsedValue))
        {
            CurrentWager = parsedValue;
        }
        else
        {
            CurrentWager = 1;
        }
    }

    public void SubmitAmount()
    {
        TryCatchPhantom();
    }

    /////////////////////////
    /// Private Functions ///
    /////////////////////////
    private void UpdateUI()
    {
        //Special case where HP = 1 just in case
        if (m_playerCurrentHealth == 1)
        {
            m_currentWager = 1;
            m_addButton.interactable = false;
            m_subtractButton.interactable = false;
            m_healthWagerAmount.text = m_currentWager.ToString();
        }

        // TODO: Clamp to player's health
        m_currentWager = Mathf.Clamp(m_currentWager, 1, (m_playerCurrentHealth - 1));
        m_addButton.interactable = (m_currentWager < (m_playerCurrentHealth - 1));
        m_subtractButton.interactable = (m_currentWager > 1);
        m_healthWagerAmount.text = m_currentWager.ToString();

        SetHeartTargetFill();
    }

    private void SetHeartTargetFill()
    {
        m_targetHeartFillPercentage = (float)m_currentWager / ((float)m_playerCurrentHealth - 1f);
    }

    private bool TryCatchPhantom()
    {
        if (!m_battleManager.CanCatch()) { return false; }

        Ares.Actor phantomToCatch = m_battleManager.CatchablePhantom();

        PhantomInstanceData phantomData = m_battleManager.ActorToData[phantomToCatch] as PhantomInstanceData;

        /// Special cases for specific phantoms first ///
        if (phantomToCatch is Ares.CatchTutorialAIActor)
        {
            SucceedCatch(phantomData, phantomToCatch);
            return true;
        }


        // First, get some values for determining how likely we are to catch the phantom
        int levelDifference = phantomData.Level - DataManager.Instance.GetPlayerBattleInstanceData().Level;
        float healthPercentage = (float)phantomToCatch.HP / (float)phantomToCatch.MaxHP;

        // Time to throw in some MAGIC NUMBERS
        // these numbers WILL PROBABLY BE BAD, BUT LET"S TRY IT OUT WOOOO

        // First step, if the phantom level is too close or higher than the player, there's a chance that it won't work no matter what.
        float acceptanceChance = .95f;
        if (levelDifference >= 0)
        {
            // Exponential dropoff for the level difference and chance for acceptance. Some number as examples:
            // Phantom is same level - 75% chance of acceptance
            // Phantom 1 level higher - 56.25% chance of acceptance
            // 2 levels = 42%, 3 levels = 30%, 4 levels = 23%
            // First impression is that it might still be too high for higher level phantoms, but let's seeeee
            acceptanceChance = Mathf.Pow(0.75f, (float)levelDifference + 1);
        }
        float randomRoll = Random.Range(0f, 1f);
        if (randomRoll > acceptanceChance)
        {
            // Fail catch
            Debug.Log(string.Format("Initial roll missed, try again lol (Random roll {0}, acceptance chance {1}", randomRoll, acceptanceChance));
            StartCoroutine(FailCatch(phantomToCatch, 1));
            return false;
        }

        // At this point, determine the required health cost in order to catch the phantom (using level difference, health %, afflictions, battle duration, randomness, etc),
        // and check it vs the wagered health

        string log = "Calculating min required health cost:\n";

        // Start with a random value between 0 and 1 - 0 being a min health wager, 1 being a max health wager.
        float healthCost = Random.Range(0f, 1f);

        log += string.Format("Initial Random Value: {0} ({1})\n", Mathf.RoundToInt(Mathf.Lerp(1f, (float)m_playerCurrentHealth, healthCost)), healthCost);


        // After this, take into account the level difference - so adjust this health cost based on the level difference (10% per level).
        healthCost += (Mathf.Clamp(((float)levelDifference / 10f), -1f, 1f));
        log += string.Format("After level difference adjustments: {0} ({1})\n", Mathf.RoundToInt(Mathf.Lerp(1f, (float)m_playerCurrentHealth, healthCost)), healthCost);

        // Adjust the health cost so that it's then halfway towards the enemy's health - so low roll vs high health would go to halfway, high roll vs low health would also go to half.
        healthCost -= (healthCost - healthPercentage) * 0.5f;
        log += string.Format("Scaling halfway to the enemy's health percentage: {0} ({1})\n", Mathf.RoundToInt(Mathf.Lerp(1f, (float)m_playerCurrentHealth, healthCost)), healthCost);


        healthCost = Mathf.Clamp01(healthCost);
        int convertedHealthMin = Mathf.RoundToInt(Mathf.Lerp(1f, (float)m_playerCurrentHealth, healthCost));

        log += "So, final health min requirement is " + convertedHealthMin;
        Debug.Log(log);

        if (m_currentWager < convertedHealthMin)
        {
            StartCoroutine(FailCatch(phantomToCatch));
            return false;
        }

        StartCoroutine(SucceedCatch(phantomData, phantomToCatch));
        return true;
    }

    private IEnumerator FailCatch(Ares.Actor phantomToCatch, int numPulses = 3)
    {
        Coroutine resultWait = StartCoroutine(WaitForResult(numPulses));
        yield return resultWait;

        m_heartAnimator.SetTrigger("Reject");
        m_responseBoxText.SetText("But it seems to reject you!");
        foreach (Animator anim in phantomToCatch.GetComponentsInChildren<Animator>())
        {
            anim.SetTrigger("Attack");
        }

        m_playerActor.TakeDamage(m_currentWager);

        yield return new WaitForSeconds(RESULTS_WATCH_DURATION);
        yield return StartCoroutine(FadeOutResults());

        m_actionInput.SkipCallback();
        StopCatching();
    }

    private IEnumerator SucceedCatch(PhantomInstanceData phantomData, Ares.Actor phantomToCatch)
    {
        Coroutine resultWait = StartCoroutine(WaitForResult());
        yield return resultWait;

        m_heartAnimator.SetTrigger("Accept");
        m_playerActor.GetComponentInChildren<Animator>().SetTrigger("Idle");
        m_responseBoxText.SetText("And it accepts you!");
        foreach (Animator anim in phantomToCatch.GetComponentsInChildren<Animator>())
        {
            anim.SetTrigger("Special");
        }


        yield return new WaitForSeconds(RESULTS_WATCH_DURATION);
        yield return StartCoroutine(FadeOutResults());

        phantomData.CurrentHP = phantomToCatch.HP;
        phantomData.CurrentMana = phantomToCatch.Mana;
        PlayerInventoryManager.Instance.AddPhantom(phantomData);
        BattleManager.Instance.ResultsManager.CaughtPhantom = phantomData;
        m_battleManager.OnEnemyDefeat(phantomToCatch);
        m_battleManager.CurrentBattle.EndBattle(Ares.Battle.EndReason.PhantomCaught);
        StopCatching();
    }

    private IEnumerator WaitForResult(int numPulses = 3)
    {
        float currentTime = 0f;

        // Set things up first
        m_menuParent.gameObject.SetActive(false);
        m_responsesBox.gameObject.SetActive(true);
        m_responseBoxText.SetText(" ");
        RectTransform heartRectTransform = m_heartObject.GetComponent<RectTransform>();
        bool heartMoved = false;
        bool boxFadedIn = false;
        while (!heartMoved || !boxFadedIn)
        {
            currentTime += Time.deltaTime;

            if (!boxFadedIn)
            {
                float fadeInProgress = currentTime / RESULTS_BOX_FADEINTIME;

                m_responsesBox.alpha = Mathf.Clamp01(fadeInProgress);
                if (fadeInProgress >= 1)
                {
                    boxFadedIn = true;
                    m_responseBoxText.SetText("You reach your heart out to the Phantom...");
                    m_playerActor.GetComponentInChildren<Animator>().SetTrigger("Catch");
                }
            }

            if (!heartMoved)
            {
                float heartMoveProgress = currentTime / MOVE_HEART_DURATION;

                heartRectTransform.anchoredPosition = Vector3.Lerp(m_heartStartingPos, m_heartMoveToPosition, Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(heartMoveProgress)));
                if (heartMoveProgress >= 1)
                {
                    heartRectTransform.anchoredPosition = m_heartMoveToPosition;
                    heartMoved = true;
                }
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < numPulses; i++)
        {
            m_heartAnimator.SetTrigger("Pulse");
            yield return new WaitForSeconds(TIME_BETWEEN_PULSES);
        }
    }

    private IEnumerator FadeOutResults()
    {
        float currentTime = 0f;

        while (currentTime < FADE_OUT_TIME)
        {
            currentTime += Time.deltaTime;
            float fadeOutProgress = currentTime / FADE_OUT_TIME;
            m_responsesBox.alpha = Mathf.Clamp01(1f - fadeOutProgress);
            yield return null;
        }
    }
}
