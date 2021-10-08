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

        public RandomEnemyEncounter(RandomEnemyEncounter other)
        {
            EncounterTypeName = other.EncounterTypeName;
            EncounterWeight = other.EncounterWeight;
            PercentageChance = other.PercentageChance;
            PercentageMin = other.PercentageMin;
            PercentageMax = other.PercentageMax;
            Enemies = other.Enemies;
        }
    }

    [Header("Random Encounter Settings")]
    [SerializeField]
    private bool m_restrictStarter = true;

    [SerializeField]
    private bool m_restrictStarterOptions = true;


    [Header("Random Encounters")]
    public List<RandomEnemyEncounter> RandomEnemyEncounters = new List<RandomEnemyEncounter>();

    private void OnValidate()
    {
        SetPercentages(RandomEnemyEncounters);

        if (m_restrictStarterOptions && !m_restrictStarter) { m_restrictStarter = true;}
    }

    public override Dictionary<CombatantInstanceData, Ares.Actor> GetEnemies()
    {
        // Create some containers for stuff
        List<PhantomInstanceData> phantomList = new List<PhantomInstanceData>();
        Dictionary<CombatantInstanceData, Ares.Actor> phantomActors = new Dictionary<CombatantInstanceData, Ares.Actor>();
        RandomEnemyEncounter encounter = null;

        // Make a new list of encounters, implementing any restrictions.
        List<RandomEnemyEncounter> restrictedList  = GetRestrictedList();
        SetPercentages(restrictedList);

        // Then get to random rolling
        float randomRoll = Random.Range(0f, 100f);

        // Figure out which encounter our random roll is
        foreach (RandomEnemyEncounter randEncounter in restrictedList)
        {
            if (randomRoll >= randEncounter.PercentageMin && randomRoll < randEncounter.PercentageMax)
            {
                encounter = randEncounter;
                break;
            }
        }

        // Generate the phantom instance data for each phantom in the encounter
        foreach(RandomEnemy randEnemy in encounter.Enemies)
        {
            PhantomInstanceData phanData = PhantomDataUtility.GenerateRandomPhantom(randEnemy.Phantom, randEnemy.PhantomLevel);
            if (phanData != null)
            {
                phantomList.Add(phanData);
            }
        }

        // Spawn in an actor for each generated phantom
        foreach (PhantomInstanceData phantom in phantomList)
        {
            phantomActors[phantom] = SpawnGenericPhantom(phantom);
        }

        return phantomActors;
    }

    // Function to generate the restricted list of phantom encounters, based on any restrictions that might be place
    // (Such as no starter options)
    private List<RandomEnemyEncounter> GetRestrictedList()
    {
        
        List<string> restrictedPhantoms = new List<string>();

        if (DataManager.Instance != null)
        {
            if (m_restrictStarterOptions && DataManager.Instance.StartersOrder != null && DataManager.Instance.StartersOrder.Count >= 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    restrictedPhantoms.Add(DataManager.Instance.StartersOrder[i]);
                }
            }
            else if (m_restrictStarter && !string.IsNullOrEmpty(DataManager.Instance.ChosenStarterID))
            {
                restrictedPhantoms.Add(DataManager.Instance.ChosenStarterID);
            }
        }
        else
        {
            Debug.Log("Data Manager is Null");
        }

        List<RandomEnemyEncounter> restrictedList = new List<RandomEnemyEncounter>();
        foreach(RandomEnemyEncounter encounterOption in RandomEnemyEncounters)
        {
            bool validEncounter = true;
            foreach(RandomEnemy phantom in encounterOption.Enemies)
            {
                if (restrictedPhantoms.Contains(phantom.Phantom.ID))
                {
                    validEncounter = false;
                    break;
                }
            }

            if (validEncounter)
            {
                restrictedList.Add(new RandomEnemyEncounter(encounterOption));
                Debug.Log("Valid Encounter: " + encounterOption.EncounterTypeName);
            }
            else
            {
                Debug.Log("Invalid Encounter: " + encounterOption.EncounterTypeName);
            }
        }

        if (restrictedList.Count > 0)
        {
            return restrictedList;
        }
        else
        {
            // If all the options are restricted, just return the normal list.
            Debug.LogWarning("All encounter options are restricted, so just returning the full list.");
            return RandomEnemyEncounters;
        }
    }

    private void SetPercentages(List<RandomEnemyEncounter> enemyEncounters)
    {
        if (enemyEncounters != null && enemyEncounters.Count > 0)
        {
            float weightTotal = 0f;
            foreach(RandomEnemyEncounter enemyEncounter in enemyEncounters)
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
            foreach(RandomEnemyEncounter enemyEncounter in enemyEncounters)
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
}