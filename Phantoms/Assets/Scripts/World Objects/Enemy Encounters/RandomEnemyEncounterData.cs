using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Phantoms/Enemy Encounter Data/Random Encounter")]
public class RandomEnemyEncounterData : EnemyEncounterData
{
    [System.Serializable]
    public class RandomEnemy
    {
        [Range(0, 15)]
        public int PhantomLevel = 1;

        public PhantomData Phantom;
    }

    [System.Serializable]
    public class RandomEnemyEncounter
    {
        public string EncounterTypeName = "Encounter";
        public float EncounterWeight = 1f;

        [ShowOnly]
        public float PercentageChance = 0f;
        [HideInInspector]
        public float PercentageMin = 0f;
        [HideInInspector]
        public float PercentageMax = 0f;

        public RandomEnemy[] Enemies;

    }

    public List<RandomEnemyEncounter> RandomEnemyEncounters = new List<RandomEnemyEncounter>();

    private void OnValidate()
    {
        if (RandomEnemyEncounters != null && RandomEnemyEncounters.Count > 0)
        {
            float weightTotal = 0f;
            foreach(RandomEnemyEncounter enemyEncounter in RandomEnemyEncounters)
            {
                if (enemyEncounter.EncounterWeight > 0f)
                {
                    if (enemyEncounter.Enemies.Length > 0)
                    {
                        weightTotal += enemyEncounter.EncounterWeight;
                    }
                }
                else
                {
                    enemyEncounter.EncounterWeight = 0f;
                }
            }

            float currentPercentage = 0f;
            foreach(RandomEnemyEncounter enemyEncounter in RandomEnemyEncounters)
            {
                if (enemyEncounter.EncounterWeight > 0f && enemyEncounter.Enemies.Length > 0)
                {
                    enemyEncounter.PercentageMin = currentPercentage;

                    enemyEncounter.PercentageChance = (enemyEncounter.EncounterWeight / weightTotal) * 100f;

                    currentPercentage += enemyEncounter.PercentageChance;

                    enemyEncounter.PercentageMax = currentPercentage;
                }
            }
        }
    }

    public override List<Ares.Actor> GetEnemies()
    {
        List<PhantomInstanceData> phantomList = new List<PhantomInstanceData>();
        List<Ares.Actor> phantomActors = new List<Ares.Actor>();

        RandomEnemyEncounter encounter = null;

        float randomRoll = Random.Range(0f, 100f);

        foreach (RandomEnemyEncounter randEncounter in RandomEnemyEncounters)
        {
            if (randomRoll >= randEncounter.PercentageMin && randomRoll < randEncounter.PercentageMax)
            {
                encounter = randEncounter;
                break;
            }
        }

        foreach(RandomEnemy randEnemy in encounter.Enemies)
        {
            PhantomInstanceData phanData = PhantomDataUtility.GenerateRandomPhantom(randEnemy.Phantom, randEnemy.PhantomLevel);
            if (phanData != null)
            {
                phantomList.Add(phanData);
            }
        }

        foreach (PhantomInstanceData phantom in phantomList)
        {
            phantomActors.Add(SpawnGenericPhantom(phantom));
        }

        return phantomActors;
    }
}