using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Phantoms/Enemy Encounter Data/Set Encounter")]
public class SetEnemyEncounterData : EnemyEncounterData
{
    [SerializeField]
    private List<PhantomInstanceData> m_phantoms = new List<PhantomInstanceData>();

    public override Dictionary<CombatantInstanceData, Ares.Actor> GetEnemies()
    {
        List<PhantomInstanceData> phantomList = new List<PhantomInstanceData>(m_phantoms);
        Dictionary<CombatantInstanceData, Ares.Actor> phantomActors = new Dictionary<CombatantInstanceData, Ares.Actor>();

        foreach (PhantomInstanceData phantom in phantomList)
        {
            phantomActors[phantom] = (SpawnGenericPhantom(phantom));
        }

        return phantomActors;
    }
}