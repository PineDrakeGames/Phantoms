using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthIndicator : MonoBehaviour
{
    [Header("Reference to actor to show HP for.")]
    [SerializeField]
    private Ares.Actor m_actor = null;
    
    [Header("References to UI Elements")]
    [SerializeField]
    private TMP_Text m_maxHP = null;
    [SerializeField]
    private TMP_Text m_currentHP = null;

    int prevHP = 0;

    // Start is called before the first frame update
    public void BattleStart()
    {
        m_maxHP.text = m_actor.MaxHP.ToString();
        m_currentHP.text = m_actor.HP.ToString();
        prevHP = m_actor.HP;

        m_actor.OnHPChange.AddListener(UpdateActorHP);
    }

    private void OnDestroy() {
        m_actor.OnHPChange.RemoveListener(UpdateActorHP);
    }

    private void UpdateActorHP(int newHP)
    {
        m_currentHP.text = newHP.ToString();
        BattlePlayerMenu.Instance.SetDamageIndicator(m_actor.gameObject.transform.position, (prevHP - newHP));
        prevHP = newHP;
    }
}
