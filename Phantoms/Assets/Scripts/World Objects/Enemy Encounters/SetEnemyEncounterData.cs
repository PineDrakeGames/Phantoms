using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Phantoms/Enemy Encounter Data/Set Encounter")]
public class SetEnemyEncounterData : EnemyEncounterData
{
    [SerializeField]
    private List<PhantomInstanceData> m_phantoms = new List<PhantomInstanceData>();

    public override List<Ares.Actor> GetEnemies()
    {
        List<PhantomInstanceData> phantomList = new List<PhantomInstanceData>(m_phantoms);
        List<Ares.Actor> phantomActors = new List<Ares.Actor>();

        foreach (PhantomInstanceData phantom in phantomList)
        {
            phantomActors.Add(SpawnGenericPhantom(phantom));
        }

        return phantomActors;
    }
}