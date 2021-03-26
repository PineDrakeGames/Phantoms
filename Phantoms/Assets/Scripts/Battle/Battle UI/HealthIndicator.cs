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

    public void SetActor(Ares.Actor newActor)
    {
        if (m_actor != null)
        {
            m_actor.OnRecieveTempBuff.RemoveListener(AddTempBuff);
            m_actor.OnHPChange.RemoveListener(UpdateActorHP);
            m_actor.OnManaChange.RemoveListener(UpdateActorMana);

            foreach(StatusEffectBuffIndicator statBuff in buffEffects)
            {
                statBuff.gameObject.SetActive(false);
            }
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

            foreach(Ares.TemporaryBuff tempBuff in newActor.TemporaryBuffs)
            {
                if (tempBuff.TurnsRemaining > 0)
                {
                    AddTempBuff(tempBuff);
                }
            }
        }

        m_actor = newActor;
    }

    private void OnDestroy()
    {
        if (Actor)
        {
            Actor.OnRecieveTempBuff.RemoveListener(AddTempBuff);
            Actor.OnHPChange.RemoveListener(UpdateActorHP);
            Actor.OnManaChange.RemoveListener(UpdateActorMana);
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

    private void AddTempBuff(Ares.TemporaryBuff tempBuff)
    {
        StatusEffectBuffIndicator statBuff = GetBuffStatusEffect();
        statBuff.SetBuff(tempBuff);
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
}
