using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInitializer : MonoBehaviour
{
    [Header("Data for where and how to spawn the combatants")]
    [SerializeField]
    private Transform m_playerSpawnPoint1;
    [SerializeField]
    private Transform m_playerSpawnPoint2;

    [SerializeField]
    private float m_maxDistanceBetweenPlayers = 3f;

    [SerializeField]
    private Transform m_enemySpawnPoint1;
    [SerializeField]
    private Transform m_enemySpawnPoint2;

    private float m_maxDistanceBetweenEnemies = 3f;



    // Start is called before the first frame update
    public void InitializeBattle()
    {
        
    }
}
