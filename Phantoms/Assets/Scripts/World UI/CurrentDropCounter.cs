using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrentDropCounter : MonoBehaviour
{
    ////////////////////////
    /// Serialize Fields ///
    ////////////////////////
    [SerializeField]
    [Tooltip("The text for displaying the current drop count")]
    private TextMeshProUGUI m_currentDropCountText = null;

    [SerializeField]
    private Animator m_counterAnimator = null;

    [Header("Variables to tweak the timing of stuff - should be just made a script constant once we pick numbers we like.")]
    [SerializeField]
    private float m_tickDelay = 0.1f;

    /////////////////////////
    /// Private Variables ///
    /////////////////////////
    private int m_displayedDropCount = 0;
    private int m_targetDropCount = 0;

    private bool m_tickingTotal = false;
    private float m_currentTime = 0f;
    private bool increasing = true;


    ///////////////////////
    /// Unity Functions ///
    ///////////////////////

    // Listen to the drop total change in the data manager, update the UI when this happens.
    private void Start()
    {
        DataManager.CurrentDropChange.AddListener(DropTotalChange);
        m_currentDropCountText.text = DataManager.CurrentDrops.ToString();
    }
    private void OnDestroy()
    {
        if (DataManager.CurrentDropChange != null)
        {
            DataManager.CurrentDropChange.RemoveListener(DropTotalChange);
        }
    }

    // If we are currently ticking up/down, check that in update.
    private void Update()
    {
        if (m_tickingTotal)
        {
            m_currentTime += Time.deltaTime;
            if (m_currentTime >= m_tickDelay)
            {
                m_currentTime -= m_tickDelay;
                TickDropTotal();
            }
        }
    }

    ////////////////////////
    /// Public Functions ///
    ////////////////////////

    // Called to change the drop total to a new specific amount
    public void DropTotalChange(int newDropAmount)
    {
        // If we are already going to that amount, or already at that amount, then ignore
        if (newDropAmount == m_targetDropCount) { return; }

        increasing = (newDropAmount > m_targetDropCount);

        m_targetDropCount = newDropAmount;

        // If not ticking, get it started up!
        if (!m_tickingTotal)
        {
            TickDropTotal();
            m_tickingTotal = true;
            m_currentTime = 0f;
        }
    }


    /////////////////////////
    /// Private functions ///
    /////////////////////////
    private void TickDropTotal()
    {
        if (m_targetDropCount == m_displayedDropCount)
        {
            m_tickingTotal = false;
            return;
        }

        // First, get the total number of digits we are workin with
        int targetDigits = 1;
        int displayedDigits = 1;
        if (m_displayedDropCount > 0)
        {
            displayedDigits = Mathf.CeilToInt(Mathf.Log10((float)m_displayedDropCount + 1f));
        }
        if (m_targetDropCount > 0)
        {
            targetDigits = Mathf.CeilToInt(Mathf.Log10((float)m_targetDropCount + 1f));
        }

        int newDisplayNumber = m_displayedDropCount;
        int biggestDigitChange = 1;
        for (int i = 1; i <= Mathf.Max(targetDigits, displayedDigits); i++)
        {
            int power = IntPow(10, (uint)i);
            int lessPower = IntPow(10, (uint)(i - 1));
            int displayedDigit = (m_displayedDropCount % power) / lessPower;
            int targetDigit = (m_targetDropCount % power) / lessPower;

            if (displayedDigit != targetDigit)
            {
                if (increasing)
                {
                    if (displayedDigit < 9)
                    {
                        newDisplayNumber += lessPower;
                    }
                    else
                    {
                        newDisplayNumber -= lessPower * 9;
                    }
                }
                else
                {
                    if (displayedDigit > 0)
                    {
                        newDisplayNumber -= lessPower;
                    }
                    else
                    {
                        newDisplayNumber += lessPower * 9;
                    }
                }

                if (i > biggestDigitChange) { biggestDigitChange = i; }
            }
        }

        //Debug.Log(string.Format("Changed from {0} to {1} with target {2}, with the biggest change being the {3} index", m_displayedDropCount, newDisplayNumber, m_targetDropCount, biggestDigitChange));
        if (m_counterAnimator)
        {
            if (biggestDigitChange >= 3)
            {
                m_counterAnimator.SetTrigger("Big");
            }
            else if (biggestDigitChange == 2)
            {
                m_counterAnimator.SetTrigger("Med");
            }
            else
            {
                m_counterAnimator.SetTrigger("Small");
            }
        }

        m_currentDropCountText.text = newDisplayNumber.ToString();
        m_displayedDropCount = newDisplayNumber;

        if (m_targetDropCount == m_displayedDropCount)
        {
            m_tickingTotal = false;
        }
    }

    private int IntPow(int x, uint pow)
    {
        if (pow < 0) { return 0; }
        int ret = 1;
        while (pow != 0)
        {
            if ((pow & 1) == 1)
                ret *= x;
            x *= x;
            pow >>= 1;
        }
        return ret;
    }
}
