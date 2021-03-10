using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ares;

public class BattleLog : MonoBehaviour
{
    [SerializeField]
    private ScrollRect m_scrollRectComponent = null;

    [SerializeField]
    private TMP_Text m_textLog = null;

    private string m_currentLog = string.Empty;

    private static BattleLog s_instance = null;
    public static BattleLog Instance
    {
        get 
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<BattleLog>();
                s_instance.Initialize();
            }
            return s_instance;
        }
    }

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
            Initialize();
        }
        else if (s_instance != this)
        {
            Destroy(this);
        }
    }

    private void Initialize()
    {
        m_currentLog = "Beginning of Combat Log!";
        m_textLog.text = m_currentLog;
    }

    public void AddBattleListeners(Battle battle)
    {
        battle.OnAbilityResults.AddListener(AbilityResult);
        battle.OnItemResults.AddListener(ItemResult);

        foreach(Actor actor in battle.Actors)
        {
            actor.OnAbilityActionEnd.AddListener(OnAbilityUse);
        }
    }

    public void AddLog(string newLine)
    {
        m_currentLog += "\n";
        m_currentLog += newLine;
        m_textLog.text = m_currentLog;
        m_textLog.GetComponent<TextSizer>().Refresh();
        m_textLog.GetComponent<TextSizer>().SetSize();
        m_textLog.ForceMeshUpdate();
        m_scrollRectComponent.normalizedPosition = Vector2.zero;
    }

    ////////////////////////
    /// Battle Listeners ///
    ////////////////////////

    private void OnAbilityUse(Actor[] targetActors, Ability ability, AbilityAction abilityAction)
    { 
        foreach(Actor target in targetActors)
        {
            string log = string.Format("   {0} recieved {1} with power {2}", target.DisplayName, abilityAction.Action.ToString(), ability.GetCachedEvaluatedPower(abilityAction, target));
            AddLog(log);
        }
    }

    private void AbilityResult(Actor actor, AbilityResults results)
    {
        string log = string.Format("{0} used ability {1}.", actor.DisplayName, results.ability.Data.DisplayName);

        /*
        foreach (AbilityAction action in results.actionResults.Keys)
        {
            BattleActionResults actionResults = results.actionResults[action];
            action.

            log += string.Format("\n    {0} action", action.Action.ToString());

            if (actionResults.hitTargets.Count > 0)
            {
                string hitTargets = "";
                for (int i = 0; i < actionResults.hitTargets.Count; i++)
                {
                    Actor hitActor = actionResults.hitTargets[i];
                    if (i > 0)
                    {
                        if (i == actionResults.hitTargets.Count - 1)
                        {
                            hitTargets += " and ";
                        }
                        else
                        {
                            hitTargets += ", ";
                        }
                    }
                    hitTargets += hitActor.DisplayName;
                }

                log += " hit targets " + hitTargets;
            }

            if (actionResults.missedActions.Count > 0)
            {
                string missedTargets = "";
                for (int i = 0; i < actionResults.missedActions.Count; i++)
                {
                    Actor missedActor = actionResults.missedActions[i].target;
                    if (i > 0)
                    {
                        if (i == actionResults.hitTargets.Count - 1)
                        {
                            missedTargets += " and ";
                        }
                        else
                        {
                            missedTargets += ", ";
                        }
                    }
                    missedTargets += missedActor.DisplayName;
                }
                if (actionResults.hitTargets.Count > 0)
                {
                    log += ", and";
                }
                log += " missed targets " + missedTargets;
            }
            log += ".";
        }
        */

        AddLog(log);
    }

    private void ItemResult(Actor actor, ItemResults results)
    {
        string log = string.Format("{0} used item {1}.", actor.DisplayName, results.item.Data.DisplayName);
        AddLog(log);
    }
}
