using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLoadingTrigger : MonoBehaviour
{
    [Scene]
    [SerializeField]
    private string scene = null;

    [SerializeField]
    private EnemyEncounterData m_encounterData = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && m_encounterData != null)
        {
            BattleStartManager.Enemies = m_encounterData;
            LoadingManager.LoadBattle(scene);
            transform.parent.gameObject.SetActive(false);
        }
    }
}
