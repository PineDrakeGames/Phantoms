using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEffectsManager : MonoBehaviour
{
    /////////////////////////
    /// Serialized Fields ///
    /////////////////////////
    [SerializeField]
    private GameObject m_damageIndicatorPrefab = null;
    [SerializeField]
    private GameObject m_buffIndicatorPrefab = null;

    /////////////////////////////
    /// Static Instance Stuff ///
    /////////////////////////////
    private static BattleEffectsManager s_instance = null;
    public static BattleEffectsManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<BattleEffectsManager>();
            }
            return s_instance;
        }
    }


    private List<DamageIndicator> m_damageIndicators = new List<DamageIndicator>();
    private List<BuffIndicator> m_buffIndicators = new List<BuffIndicator>();


    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }
    }

    ///////////////////////////////////////
    /// Public Show Indicator Functions ///
    ///////////////////////////////////////
    public void SetDamageIndicator(Vector3 position, int damage, float modifier = 1f)
    {
        GetDamageIndicator().SetDamageIndicator(position, damage, modifier);

        float damageIntensity = ((float)damage / 10f);

        CameraShakeBattle.Instance.SetShake(Mathf.Lerp(0.3f, 0.6f, damageIntensity));


        if (modifier <= 0.5f)
        {
            AudioManager.PlaySound("BATTLE_HIT_WEAK");
        }
        else if (modifier >= 0.5f)
        {
            AudioManager.PlaySound("BATTLE_HIT_STRONG");
        }
        else
        {
            AudioManager.PlaySound("BATTLE_HIT");
        }
    }

    public void SetBuffIndicator(Vector3 position, Ares.Stat stat, int stages)
    {
        GetBuffIndicator().SetBuffIndicator(position, stat, stages);

        if (stages >= 0)
        {
            AudioManager.PlaySound("BATTLE_BUFF");

        }
        else
        {
            AudioManager.PlaySound("BATTLE_DEBUFF");
        }
    }


    ////////////////////////////////
    /// Private Helper Functions ///
    ////////////////////////////////
    private DamageIndicator GetDamageIndicator()
    {
        foreach (DamageIndicator indicator in m_damageIndicators)
        {
            if (indicator.Ready)
            {
                return indicator;
            }
        }
        GameObject newIndicatorObject = Instantiate(m_damageIndicatorPrefab);
        DamageIndicator newIndicator = newIndicatorObject.GetComponent<DamageIndicator>();
        m_damageIndicators.Add(newIndicator);
        return newIndicator;
    }

    private BuffIndicator GetBuffIndicator()
    {
        foreach (BuffIndicator indicator in m_buffIndicators)
        {
            if (indicator.Ready)
            {
                return indicator;
            }
        }
        GameObject newIndicatorObject = Instantiate(m_buffIndicatorPrefab);
        BuffIndicator newIndicator = newIndicatorObject.GetComponent<BuffIndicator>();
        m_buffIndicators.Add(newIndicator);
        return newIndicator;
    }
}
