using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthIndicator : MonoBehaviour
{
    [Header("References to UI Elements")]
    [SerializeField]
    private TMP_Text m_actorName = null;
    [SerializeField]
    private TMP_Text m_maxHP = null;
    [SerializeField]
    private TMP_Text m_currentHP = null;
    [SerializeField]
    private TMP_Text m_maxMana = null;
    [SerializeField]
    private TMP_Text m_currentMana = null;

    [Header("Buffs & Afflictions")]
    [SerializeField]
    private Transform m_statusEffectsList = null;

    [SerializeField]
    private GameObject m_buffStatusEffectPrefab = null;
    [SerializeField]
    private GameObject m_afflictionStatusPrefab = null;

    private Ares.Actor m_actor = null;
    public Ares.Actor Actor
    {
        get { return m_actor;}
        set
        {
            SetActor(value);
        }
    }

    int prevHP = 0;

    private List<StatusEffectBuffIndicator> buffEffects = new List<StatusEffectBuffIndicator>();
    private List<StatusEffectAfflictionIndicator> afflictionIndicators = new List<StatusEffectAfflictionIndicator>();
    private Dictionary<Ares.Affliction, StatusEffectAfflictionIndicator> afflictionToIndicator = new Dictionary<Ares.Affliction, StatusEffectAfflictionIndicator>();

    public void SetActor(Ares.Actor newActor)
    {
        if (m_actor != null)
        {
            m_actor.OnRecieveTempBuff.RemoveListener(AddTempBuff);
            m_actor.OnHPChange.RemoveListener(UpdateActorHP);
            m_actor.OnManaChange.RemoveListener(UpdateActorMana);
            m_actor.OnAfflictionObtain.RemoveListener(AddAffliction);
            m_actor.OnAfflictionCure.RemoveListener(RemoveAffliction);

            foreach(StatusEffectBuffIndicator statBuff in buffEffects)
            {
                statBuff.gameObject.SetActive(false);
            }
            foreach(StatusEffectAfflictionIndicator affliction in afflictionIndicators)
            {
                affliction.gameObject.SetActive(false);
            }
            afflictionToIndicator.Clear();
        }

        if (newActor != null)
        {
            m_maxHP.text = newActor.MaxHP.ToString();
            m_currentHP.text = newActor.HP.ToString();
            m_maxMana.text = newActor.MaxMana.ToString();
            m_currentMana.text = newActor.Mana.ToString();
            m_actorName.text = newActor.DisplayName;

            prevHP = newActor.HP;
            newActor.OnRecieveTempBuff.AddListener(AddTempBuff);
            newActor.OnHPChange.AddListener(UpdateActorHP);
            newActor.OnManaChange.AddListener(UpdateActorMana);
            newActor.OnAfflictionObtain.AddListener(AddAffliction);
            newActor.OnAfflictionCure.AddListener(RemoveAffliction);

            foreach(Ares.TemporaryBuff tempBuff in newActor.TemporaryBuffs)
            {
                if (tempBuff.TurnsRemaining > 0)
                {
                    AddTempBuff(tempBuff);
                }
            }

            foreach(Ares.Affliction affliction in newActor.Afflictions)
            {
                if (affliction.RoundsRemaining > 0)
                {
                    AddAffliction(affliction);
                }
            }
        }

        m_actor = newActor;
    }

    private void OnDestroy()
    {
        if (Actor)
        {
            SetActor(null);
        }
    }

    private void UpdateActorHP(int newHP)
    {
        m_currentHP.text = newHP.ToString();
        if (Actor)
        {
            BattlePlayerMenu.Instance.SetDamageIndicator(Actor.gameObject.transform.position, (prevHP - newHP));
        }
        prevHP = newHP;
    }

    private void UpdateActorMana(int newMana)
    {
        m_currentMana.text = newMana.ToString();
    }

    // Status Effect Functions

    private void AddTempBuff(Ares.TemporaryBuff tempBuff)
    {
        StatusEffectBuffIndicator statBuff = GetBuffStatusEffect();
        statBuff.SetBuff(tempBuff);
    }

    private void AddAffliction(Ares.Affliction affliction)
    {
        StatusEffectAfflictionIndicator newIndicator = GetAfflictionStatusEffect();
        afflictionToIndicator.Add(affliction, newIndicator);
        newIndicator.SetAffliction(affliction);
    }

    private void RemoveAffliction(Ares.Affliction affliction)
    {
        if (afflictionToIndicator.ContainsKey(affliction))
        {
            afflictionToIndicator[affliction].gameObject.SetActive(false);
            afflictionToIndicator.Remove(affliction);
        }
    }

    ////////////////////////
    /// Helper Functions ///
    ////////////////////////

    private StatusEffectBuffIndicator GetBuffStatusEffect()
    {
        foreach(StatusEffectBuffIndicator statBuff in buffEffects)
        {
            if (!statBuff.gameObject.activeSelf)
            {
                statBuff.transform.SetSiblingIndex(buffEffects.Count - 1);
                return statBuff;
            }
        }

        GameObject newStatBuffObject = Instantiate(m_buffStatusEffectPrefab, m_statusEffectsList);
        StatusEffectBuffIndicator newStatBuffComponent = newStatBuffObject.GetComponent<StatusEffectBuffIndicator>();
        buffEffects.Add(newStatBuffComponent);
        newStatBuffObject.transform.SetSiblingIndex(buffEffects.Count - 1);
        return newStatBuffComponent;
    }

    private StatusEffectAfflictionIndicator GetAfflictionStatusEffect()
    {
        foreach(StatusEffectAfflictionIndicator affliction in afflictionIndicators)
        {
            if (!affliction.gameObject.activeSelf)
            {
                affliction.transform.SetSiblingIndex(buffEffects.Count + afflictionIndicators.Count - 1);
                return affliction;
            }
        }

        GameObject newAfflictionObject = Instantiate(m_afflictionStatusPrefab, m_statusEffectsList);
        StatusEffectAfflictionIndicator newAfflictionComponent = newAfflictionObject.GetComponent<StatusEffectAfflictionIndicator>();
        afflictionIndicators.Add(newAfflictionComponent);
        newAfflictionObject.transform.SetSiblingIndex(buffEffects.Count - 1);
        return newAfflictionComponent;
    }
}
