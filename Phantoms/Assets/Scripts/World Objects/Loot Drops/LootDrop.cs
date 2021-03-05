using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDrop : MonoBehaviour
{
    [Header("Data for drop, leave empty for none.")]
    [SerializeField]
    private LootDropData m_lootDropData = null;

    [Header("Where to place the Drop - leave blank for transform")]
    [SerializeField]
    private Transform m_dropLocation = null;


    public void DropLoot()
    {
        if (m_lootDropData)
        {
            if (m_dropLocation == null)
            {
                m_dropLocation = transform;
            }

            // Instantiate things for all the loot drop data!
            for (int i = 0; i < m_lootDropData.GetDropDrops(); i++)
            {
                GameObject.Instantiate(DataManager.DropLootPrefab, m_dropLocation.position, Quaternion.identity);
            }

            List<string> itemDrops = m_lootDropData.GetItemDrops();

            foreach (string itemID in itemDrops)
            {
                GameObject itemObject = GameObject.Instantiate(DataManager.ItemLootPrefab, m_dropLocation.position, Quaternion.identity);
                ItemPickup pickupComponent = itemObject.GetComponent<ItemPickup>();
                pickupComponent.SetItem(itemID);
            }
        }
    }
}
