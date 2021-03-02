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

            List<PhantomInstanceData> encounterPhantoms = m_encounterData.GetEnemyPhantoms();
            if (encounterPhantoms.Count > 0)
            {
                BattleStartManager.EnemyPhantoms = encounterPhantoms;
                LoadingManager.LoadScene(scene, LoadingManager.SceneType.BATTLE);
            }
        }
    }
}
