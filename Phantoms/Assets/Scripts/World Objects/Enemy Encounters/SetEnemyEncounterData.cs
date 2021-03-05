using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Phantoms/Enemy Encounter Data/Set Encounter")]
public class SetEnemyEncounterData : EnemyEncounterData
{
    [SerializeField]
    private List<PhantomInstanceData> m_phantoms = new List<PhantomInstanceData>();

    public override List<PhantomInstanceData> GetEnemyPhantoms()
    {
        List<PhantomInstanceData> returnList = new List<PhantomInstanceData>(m_phantoms);

        return returnList;
    }
}