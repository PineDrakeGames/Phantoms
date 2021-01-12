using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthIndicator : MonoBehaviour
{
    [Header("Reference to actor to show HP for.")]
    public Ares.Actor Actor = null;

    [Header("References to UI Elements")]
    [SerializeField]
    private TMP_Text m_maxHP = null;
    [SerializeField]
    private TMP_Text m_currentHP = null;

    int prevHP = 0;

    // Start is called before the first frame update
    public void BattleStart()
    {
        if (Actor)
        {
            m_maxHP.text = Actor.MaxHP.ToString();
            m_currentHP.text = Actor.HP.ToString();
            prevHP = Actor.HP;
            Actor.OnHPChange.AddListener(UpdateActorHP);
        }
    }

    private void OnDestroy()
    {
        if (Actor)
        {
            Actor.OnHPChange.RemoveListener(UpdateActorHP);
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
}
