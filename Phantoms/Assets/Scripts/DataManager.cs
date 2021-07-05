using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataManager : MonoBehaviour
{
    // Reference to the data manager prefab in resources, so that we can load it into any scene.
    private const string DATA_MANAGER_PREFAB = "Data Manager";

    // Stuff for the static instance of data manager - will create a data manager if one does not exist.
    private static DataManager s_instance = null;
    public static DataManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                GameObject instance = Instantiate(Resources.Load(DATA_MANAGER_PREFAB, typeof(GameObject))) as GameObject;
                instance.name = "Data Manager";
                s_instance = instance.GetComponent<DataManager>();
                if (s_instance)
                {
                    s_instance.Initialize();
                }
            }
            return s_instance;
        }
    }


    //////////////////////////////////
    /// Serialized fields for data ///
    //////////////////////////////////
    [Header("Data Serialize Fields")]
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

    [SerializeField]
    private RelicDataTable m_relicData = null;
    public static RelicDataTable RelicData
    {
        get { return Instance.m_relicData; }
    }

    [SerializeField]
    private AudioClipTable m_soundEffectData = null;
    public static AudioClipTable SoundEffectData
    {
        get { return Instance.m_soundEffectData; }
    }

    /////////////////////////////////////////////////////////////
    /// Prefabs that can be instantiated pretty much anywhere ///
    /////////////////////////////////////////////////////////////
    [Header("Prefab references needed commonly")]
    [SerializeField]
    private GameObject m_dropLootPrefab;
    public static GameObject DropLootPrefab { get { return Instance.m_dropLootPrefab; } }

    [SerializeField]
    private GameObject m_itemLootPrefab;
    public static GameObject ItemLootPrefab { get { return Instance.m_itemLootPrefab; } }

    /////////////////////
    /// Public events ///
    /////////////////////
    public static UnityEvent<int> CurrentDropChange = new UnityEvent<int>();
    public static UnityEvent<int> PlayerHPChange = new UnityEvent<int>();

    ////////////////////
    /// Runtime Data ///
    ////////////////////

    // Converting the data into dictionaries to more easily access it
    private Dictionary<string, PhantomData> m_phantomIdToData = null;
    private Dictionary<string, ItemData> m_ItemIdToData = null;
    private Dictionary<string, RelicData> m_RelicIdToData = null;

    // Current player data
    private PlayerBattleInstanceData m_playerBattleInstanceData = null;

    // Variables for drops, the currency.
    private int m_currentDrops = 0;
    public static int CurrentDrops
    {
        get
        {
            return Instance.m_currentDrops;
        }
        set
        {
            // Update current drops, make sure we have at minimum 0, then invoke the event with the new total amount of drops.
            Instance.m_currentDrops = value;
            if (Instance.m_currentDrops < 0) { Instance.m_currentDrops = 0; }
            CurrentDropChange.Invoke(Instance.m_currentDrops);
        }
    }

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
            Initialize();
        }
        else if (s_instance != this)
        {
            Destroy(this);
        }
    }


    ////////////////////////
    /// Public functions ///
    ////////////////////////

    // Initialize function, called when a data manager is created
    public void Initialize()
    {
        // Any initialization things
        DontDestroyOnLoad(this.gameObject);

        if (m_phantomIdToData == null)
        {
            m_phantomIdToData = new Dictionary<string, PhantomData>();
            foreach (PhantomData data in PhantomData.Data)
            {
                m_phantomIdToData.Add(data.ID, data);
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

        if (m_RelicIdToData == null)
        {
            m_RelicIdToData = new Dictionary<string, RelicData>();
            foreach (RelicData data in RelicData.Data)
            {
                m_RelicIdToData.Add(data.ID, data);
            }
        }

        m_playerBattleInstanceData = new PlayerBattleInstanceData(m_playerBattleData);
        // TODO: Save this stuff and whatnot, just defaulting the player to level 3 with 1 level up in each.
        m_playerBattleInstanceData.Level = 3;
        m_playerBattleInstanceData.LevelUps.Mana = 1;
        m_playerBattleInstanceData.LevelUps.MaxHP = 1;
        m_playerBattleInstanceData.LevelUps.Relic = 1;
        m_playerBattleInstanceData.SetCurrentStats();
        m_playerBattleInstanceData.FullRestore();
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

    public ItemData TryGetItemData(Ares.ItemData aresItem)
    {
        foreach (ItemData itemData in ItemData.Data)
        {
            if (itemData.AresData == aresItem)
            {
                return itemData;
            }
        }
        return null;
    }

    public RelicData TryGetRelicData(string relicID)
    {
        if (m_RelicIdToData.ContainsKey(relicID))
        {
            return m_RelicIdToData[relicID];
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

    public PlayerBattleInstanceData GetPlayerBattleInstanceData()
    {
        return m_playerBattleInstanceData;
    }

    public void FullHeal()
    {
        m_playerBattleInstanceData.FullRestore();

        foreach (PhantomInstanceData phantom in PlayerInventoryManager.Instance.Phantoms)
        {
            phantom.FullRestore();
        }
    }
}
