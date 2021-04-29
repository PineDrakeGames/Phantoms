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
    [SerializeField]
    private BattlePlayerMenu m_playerMenu = null;
    [SerializeField]
    private GameObject m_menuParent = null;

    [Header("Wager amounts")]
    [SerializeField]
    private TMP_InputField m_healthWagerAmount = null;
    [SerializeField]
    private Button m_addButton = null;
    [SerializeField]
    private Button m_subtractButton = null;

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

    private Ares.Actor m_playerActor;
    private Ares.ActionInput m_actionInput;

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Awake()
    {
        m_healthWagerAmount.onValueChanged.AddListener(OnValueChange);
        m_healthWagerAmount.onEndEdit.AddListener(OnEndEdit);
        StopCatching();
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

    }

    ////////////////////////
    /// Public Functions ///
    ////////////////////////
    public void StartCatch(Ares.Actor playerActor, Ares.ActionInput actionInput)
    {
        m_menuParent.SetActive(true);
        CurrentWager = 1;
        m_playerCurrentHealth = playerActor.HP;
        m_playerActor = playerActor;
        m_actionInput = actionInput;
    }

    public void StopCatching()
    {
        m_menuParent.SetActive(false);
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
        StopCatching();
    }

    /////////////////////////
    /// Private Functions ///
    /////////////////////////
    private void UpdateUI()
    {
        // TODO: Clamp to player's health
        m_currentWager = Mathf.Clamp(m_currentWager, 1, (m_playerCurrentHealth - 1));
        m_addButton.interactable = (m_currentWager < (m_playerCurrentHealth - 1));
        m_subtractButton.interactable = (m_currentWager > 1);
        m_healthWagerAmount.text = m_currentWager.ToString();
    }

    private bool TryCatchPhantom()
    {
        if (!m_battleManager.CanCatch()) { return false; }

        Ares.Actor phantomToCatch = m_battleManager.CatchablePhantom();

        PhantomInstanceData phantomData = m_battleManager.ActorToData[phantomToCatch] as PhantomInstanceData;

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
        if (randomRoll <= acceptanceChance)
        {
            // Fail catch
            FailCatch();
            return false;
        }

        // At this point, determine the required health cost in order to catch the phantom (using level difference, health % and randomness),
        // and check it vs the wagered health
        //float baseHealthCost = healthPercentage;
        // Modify based on level difference

        // FOR NOW, JUST MAKING SURE IT'S AT LEAST 3 HP
        if (m_currentWager < 3)
        {
            FailCatch();
            return false;
        }

        phantomData.CurrentHP = phantomToCatch.HP;
        phantomData.CurrentMana = phantomToCatch.Mana;
        PlayerInventoryManager.Instance.AddPhantom(phantomData);
        m_battleManager.CurrentBattle.EndBattle(Ares.Battle.EndReason.PhantomCaught);
        return true;
    }

    private void FailCatch()
    {
        m_playerActor.TakeDamage(m_currentWager);
        m_actionInput.SkipCallback();
    }
}
