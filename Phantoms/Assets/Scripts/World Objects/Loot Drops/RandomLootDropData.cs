using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Phantoms/Loot Drop/Random")]
public class RandomLootDropData : LootDropData
{
    [System.Serializable]
    private class RandomItemDrop
    {
        [Item]
        public string ItemID = null;
        public float ItemWeight = 1f;

        [ShowOnly]
        public float PercentageChance = 0f;
        [HideInInspector]
        public float PercentageMin = 0f;
        [HideInInspector]
        public float PercentageMax = 0f;
    }

    [Header("Drops variables")]
    [SerializeField]
    private int m_minDrops = 0;
    [SerializeField]
    private int m_maxDrops = 0;

    [Header("Items Variables")]
    [SerializeField]
    [Range(0f, 100f)]
    private float m_itemDropChance = 20f;
    [SerializeField]
    private List<RandomItemDrop> m_randomItemDrops = new List<RandomItemDrop>();

    private void OnValidate()
    {
        if (m_minDrops < 0) { m_minDrops = 0; }
        if (m_maxDrops < m_minDrops) { m_maxDrops = m_minDrops; }

        if (m_randomItemDrops != null && m_randomItemDrops.Count > 0)
        {
            float weightTotal = 0f;
            foreach(RandomItemDrop itemDrop in m_randomItemDrops)
            {
                if (itemDrop.ItemWeight > 0f)
                {
                    weightTotal += itemDrop.ItemWeight;
                }
                else
                {
                    itemDrop.ItemWeight = 0f;
                }
            }

            float currentPercentage = 0f;
            foreach(RandomItemDrop itemDrop in m_randomItemDrops)
            {
                if (itemDrop.ItemWeight > 0f)
                {
                    itemDrop.PercentageMin = currentPercentage;

                    itemDrop.PercentageChance = (itemDrop.ItemWeight / weightTotal) * 100f;

                    currentPercentage += itemDrop.PercentageChance;

                    itemDrop.PercentageMax = currentPercentage;
                }
            }
        }
    }

    public override List<string> GetItemDrops()
    {
        List<string> returnList = new List<string>();
        float randomRoll = Random.Range(0f, 100f);

        if (randomRoll <= m_itemDropChance)
        {
            randomRoll = Random.Range(0f, 100f);
            foreach (RandomItemDrop randItem in m_randomItemDrops)
            {
                if (randomRoll >= randItem.PercentageMin && randomRoll < randItem.PercentageMax)
                {
                    returnList.Add(randItem.ItemID);
                    break;
                }
            }
        }

        return returnList;
    }

    public override int GetDropDrops()
    {
        return Random.Range(m_minDrops, m_maxDrops + 1);
    }
}
