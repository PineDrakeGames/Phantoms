using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PixelCrushers.DialogueSystem;

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
                CreateInstance();
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
    public PlayerBattleData PlayerBattleData { get { return m_playerBattleData; } }

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
    private KeyItemDataTable m_keyItemData = null;
    public static KeyItemDataTable KeyItemData
    {
        get { return Instance.m_keyItemData; }
    }

    [SerializeField]
    private AudioClipTable m_soundEffectData = null;
    public static AudioClipTable SoundEffectData
    {
        get { return Instance.m_soundEffectData; }
    }

    [SerializeField]
    private AudioClipTable m_musicData = null;
    public static AudioClipTable MusicData
    {
        get { return Instance.m_musicData; }
    }

    [SerializeField]
    private AudioClipTable m_ambienceData = null;
    public static AudioClipTable AmbienceData
    {
        get { return Instance.m_ambienceData; }
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

    private SettingsData m_settings = null;
    public SettingsData Settings
    {
        get { return m_settings; }
        set { m_settings = value; }
    }

    // Converting the data into dictionaries to more easily access it
    private Dictionary<string, PhantomData> m_phantomIdToData = null;
    private Dictionary<string, ItemData> m_ItemIdToData = null;
    private Dictionary<string, RelicData> m_RelicIdToData = null;
    private Dictionary<string, KeyItemData> m_KeyItemIdToData = null;


    // Current player data
    private PlayerBattleInstanceData m_playerBattleInstanceData = null;
    public PlayerBattleInstanceData PlayerInstanceData
    {
        get { return m_playerBattleInstanceData; }
        set { m_playerBattleInstanceData = value; }
    }

    // Variables for drops, the currency.
    private const string FIRST_DROP_FLAG = "GrabbedFirstDrop";

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


    // This should be saved at some point, but I guess for now just put it here?
    public List<string> StartersOrder = new List<string>();
    public List<string> StarterChoices = new List<string>();
    public string ChosenStarterID = "";
    public string KindredInstanceID = "";

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

        CurrentDropChange.AddListener(FirstDropTutorial);
    }


    ////////////////////////
    /// Public functions ///
    ////////////////////////

    public static void CreateInstance()
    {
        GameObject instance = Instantiate(Resources.Load(DATA_MANAGER_PREFAB, typeof(GameObject))) as GameObject;
        instance.name = "Data Manager";
        s_instance = instance.GetComponent<DataManager>();
        if (s_instance)
        {
            s_instance.Initialize();
        }
    }

    // Initialize function, called when a data manager is created
    public void Initialize()
    {
        // Any initialization things
        DontDestroyOnLoad(this.gameObject);

        m_settings = new SettingsData();

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

        if (m_KeyItemIdToData == null)
        {
            m_KeyItemIdToData = new Dictionary<string, KeyItemData>();
            foreach (KeyItemData data in KeyItemData.Data)
            {
                m_KeyItemIdToData.Add(data.ItemID, data);
            }
        }

        m_playerBattleInstanceData = new PlayerBattleInstanceData(m_playerBattleData);
        // TODO: Save this stuff and whatnot, just defaulting the player to level 3 with 1 level up in each.
        m_playerBattleInstanceData.Level = 1;
        m_playerBattleInstanceData.LevelUps.Mana = 0;
        m_playerBattleInstanceData.LevelUps.MaxHP = 0;
        m_playerBattleInstanceData.LevelUps.Relic = 0;
        m_playerBattleInstanceData.SetCurrentStats();
        m_playerBattleInstanceData.FullRestore();

        RegisterLuaFunctions();
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

    public KeyItemData TryGetKeyItemData(string itemID)
    {
        if (m_KeyItemIdToData.ContainsKey(itemID))
        {
            return m_KeyItemIdToData[itemID];
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

    public float GetMusicVolume()
    {
        return Settings.MasterVolume * Settings.MusicVolume;
    }

    public float GetSoundVolume()
    {
        return Settings.MasterVolume * Settings.SoundsVolume;
    }

    public void FirstDropTutorial(int numDrops)
    {
        if (!SaveDataManager.CheckFlag(FIRST_DROP_FLAG) && numDrops > 0)
        {
            NotificationManager.SetBottomNotification("You got a <b>drop!</b>\nThere seems to be many of them scattered about. Collecting them will probably be useful!");
            SaveDataManager.SetFlag(FIRST_DROP_FLAG);
        }
    }


    /// Private Helper Functions! ///

    // NOTE(CJ) - It might be better to put all these lua functions in one seperate script at one point,
    // But for now I'm keeping them with the scripts that they are related to.
    private void RegisterLuaFunctions()
    {
        Lua.RegisterFunction("CheckFlag", this, SymbolExtensions.GetMethodInfo(() => SaveDataManager.CheckFlag(string.Empty)));
        Lua.RegisterFunction("SetFlag", this, SymbolExtensions.GetMethodInfo(() => SaveDataManager.SetFlag(string.Empty, true)));
        Lua.RegisterFunction("Save", this, SymbolExtensions.GetMethodInfo(() => PixelCrushers.SaveSystem.SaveToSlot(SaveDataManager.CurrentSaveSlot)));
    }
}
