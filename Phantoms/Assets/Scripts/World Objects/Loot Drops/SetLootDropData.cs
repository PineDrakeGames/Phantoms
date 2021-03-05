using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Phantoms/Loot Drop/Set")]
public class SetLootDropData : LootDropData
{
    // Classes for data stuff
    [System.Serializable]
    private class SetItemDrop
    {
        [Item]
        public string ItemID = null;

        public int ItemAmount = 1;
    }

    [SerializeField]
    private int m_drops = 0;

    [SerializeField]
    private SetItemDrop[] m_items = null;

    private void OnValidate()
    {
        if (m_drops < 0) { m_drops = 0; }
    }

    public override List<string> GetItemDrops()
    {
        List<string> returnList = new List<string>();

        foreach (SetItemDrop itemDrop in m_items)
        {
            for (int i = 0; i < itemDrop.ItemAmount; i++)
            {
                returnList.Add(itemDrop.ItemID);
            }
        }

        return returnList;
    }

    public override int GetDropDrops()
    {
        return m_drops;
    }
}
