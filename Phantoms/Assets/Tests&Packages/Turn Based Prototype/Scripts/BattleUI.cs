using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_playerCurrentHealth;

    [SerializeField]
    private TextMeshProUGUI m_playerMaxHealth;

    [SerializeField]
    private TextMeshProUGUI m_enemyCurrentHealth;

    [SerializeField]
    private TextMeshProUGUI m_enemyMaxHealth;

    public void SetPlayerHealth(int currentHealth, int maxHealth)
    {
        m_playerCurrentHealth.text = currentHealth.ToString();
        m_playerMaxHealth.text = maxHealth.ToString();
    }

    public void SetPlayerHealth(int currentHealth)
    {
        m_playerCurrentHealth.text = currentHealth.ToString();
    }

    public void SetEnemyHealth(int currentHealth, int maxHealth)
    {
        m_enemyCurrentHealth.text = currentHealth.ToString();
        m_enemyMaxHealth.text = maxHealth.ToString();
    }

    public void SetEnemyHealth(int currentHealth)
    {
        m_enemyCurrentHealth.text = currentHealth.ToString();
    }
}
