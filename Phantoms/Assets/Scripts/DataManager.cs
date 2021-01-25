using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private const string DATA_MANAGER_PREFAB = "Data Manager";

    private static DataManager s_instance = null;
    public static DataManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                GameObject instance = Instantiate(Resources.Load(DATA_MANAGER_PREFAB, typeof(GameObject))) as GameObject;
                s_instance = instance.GetComponent<DataManager>();
                if (s_instance)
                {
                    s_instance.Initialize();
                }
            }
            return s_instance;
        }
    }

    [SerializeField]
    private PlayerBattleData m_playerBattleData = null;

    [SerializeField]
    private PhantomDataTable m_phantomData = null;
    public static PhantomDataTable PhantomData
    {
        get { return Instance.m_phantomData; }
    }

    [SerializeField]
    private ItemDataTable m_itemData = null;
    public static ItemDataTable ItemData
    {
        get { return Instance.m_itemData; }
    }

    private Dictionary<string, PhantomData> m_phantomIdToData = null;
    private Dictionary<string, ItemData> m_ItemIdToData = null;

    public void Initialize()
    {
        // Any initialization things
        DontDestroyOnLoad(this.gameObject);

        if (m_phantomIdToData == null)
        {
            m_phantomIdToData = new Dictionary<string, PhantomData>();
            foreach (PhantomData data in PhantomData.Data)
            {
                m_phantomIdToData.Add(data.PhantomID, data);
            }
        }

        if (m_ItemIdToData == null)
        {
            m_ItemIdToData = new Dictionary<string, ItemData>();
            foreach (ItemData data in ItemData.Data)
            {
                m_ItemIdToData.Add(data.ItemID, data);
            }
        }
    }

    public PhantomData TryGetPhantomData(string phantomID)
    {
        if (m_phantomIdToData.ContainsKey(phantomID))
        {
            return m_phantomIdToData[phantomID];
        }
        else
        {
            return null;
        }
    }

    public ItemData TryGetItemData(string itemID)
    {
        if (m_ItemIdToData.ContainsKey(itemID))
        {
            return m_ItemIdToData[itemID];
        }
        else
        {
            return null;
        }
    }

    public PlayerBattleData GetPlayerBattleData()
    {
        return m_playerBattleData;
    }
}
