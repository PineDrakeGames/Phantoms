using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// Classes for storing the data that we actually need to save for stuff, as we don't always need everything - ///
/// Some stuff can be constructed upon loading the saved data.                                                 ///
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// A class for saving flags, as we can't easily serialize a dictionary.
[System.Serializable]
public class Flag
{
    public string ID = "";
    public bool Value = false;

    public Flag(string id, bool value)
    {
        ID = id;
        Value = value;
    }
}

/// A class for saving the important data for the player - only what we need. The rest of the stats can be built from this. ///
[System.Serializable]
public class SavedPlayerData
{
    // Can grab the base player data from the data manager
    public int Experience = 0;
    public LevelUpStats LevelUps = new LevelUpStats(); // Don't need the level, can calculate that with this.
    public int CurrentHP = 0;
    public int CurrentMana = 0;

    public SavedPlayerData()
    {
    }

    public SavedPlayerData(PlayerBattleInstanceData playerData)
    {
        Experience = playerData.Experience;
        LevelUps = new LevelUpStats(playerData.LevelUps);
        CurrentHP = playerData.CurrentHP;
        CurrentMana = playerData.CurrentMana;
    }
}

/// Similar to above, but for phantoms - needs a bit more extra info ///
[System.Serializable]
public class SavedPhantomData
{
    public string PhantomID = null; // Can grab the base phantom data from this
    public string InstanceID = null;
    public string NickName = "";
    public PhantomBackground Background = PhantomBackground.None;
    public int Experience = 0;
    public LevelUpStats LevelUps = new LevelUpStats(); // Don't need the level, can calculate that with this.
    public int CurrentHP = 0;
    public int CurrentMana = 0;

    public SavedPhantomData(PhantomInstanceData phantomData)
    {
        PhantomID = phantomData.PhantomID;
        InstanceID = phantomData.InstanceID;
        NickName = phantomData.NickName;
        Background = phantomData.Background;
        Experience = phantomData.Experience;
        LevelUps = new LevelUpStats(phantomData.LevelUps);
        CurrentHP = phantomData.CurrentHP;
        CurrentMana = phantomData.CurrentMana;
    }
}

/// Class for data saved for each relic ///
[System.Serializable]
public class SavedRelicData
{
    public string ID = ""; // Can get rest of data from ID
    public string UserID = ""; // Can assume equipped depending on if this is null or not

    public SavedRelicData()
    {
    }

    public SavedRelicData(RelicInstance data)
    {
        ID = data.Data.ID;
        UserID = data.UserID;
    }
}

/// Class for data saved for each item AND key item (Both require the same things - just ID and quantity)
[System.Serializable]
public class SavedItemData
{
    public string ID = "";
    public int Quantity = 0;

    public SavedItemData()
    {
    }

    public SavedItemData(ItemInstanceData data)
    {
        ID = data.Data.ItemID;
        Quantity = data.Quantity;
    }

    public SavedItemData(KeyItemInstanceData data)
    {
        ID = data.Data.ItemID;
        Quantity = data.Quantity;
    }
}

////////////////////////////////////////////////////////////////////////
/// The class containing all the data that we actually need to save. ///
////////////////////////////////////////////////////////////////////////
[System.Serializable]
public class SavedGameData
{
    // Story Stuff
    public List<Flag> Flags = new List<Flag>();

    // The Player!
    public SavedPlayerData PlayerBattleData = new SavedPlayerData();

    // Inventory Stuff
    public List<SavedItemData> Items = new List<SavedItemData>();
    public List<SavedPhantomData> Phantoms = new List<SavedPhantomData>();
    public List<SavedRelicData> Relics = new List<SavedRelicData>();
    public List<SavedItemData> KeyItems = new List<SavedItemData>();

    // Last scene and position that the player was in
    public string Scene = "";
    public Vector3 ScenePosition = Vector3.zero;

    // Other
    public int Drops = 0;
    public List<string> StartersOrder = new List<string>();
    public List<string> StarterChoices = new List<string>();
    public string ChosenStarterID = "";
    public string KindredInstanceID = "";
}


public static class SaveDataManager
{
    public static readonly string VERSION_NUMBER = "0.1.0";

    public static int CurrentSaveSlot = 0;

    public static SavedGameData SavedData = new SavedGameData();

    private static Dictionary<string, bool> m_flags = new Dictionary<string, bool>();

    public static UnityEvent<string, bool> OnFlagUpdate = new UnityEvent<string, bool>();

    // Updating the SavedData object with all the other currently set static data.
    // NOTE(CJ): Trying something different from apotheker - trying to just set up one big ol' class that will contain all
    // the things being saved. Might be a terrible idea, but the hope is that this will make testing and controlling save data
    // much easier to do.
    public static void UpdateSavedData()
    {
        if (SavedData == null) { SavedData = new SavedGameData(); }
        // Flags
        SavedData.Flags.Clear();
        foreach(KeyValuePair<string, bool> flag in m_flags)
        {
            SavedData.Flags.Add(new Flag(flag.Key, flag.Value));
        }

        // --------------------------------------------
        /// Note for saving the player, phantom, and relics:
        ///    The player's relic inventory and their player and phantoms are pretty interconnected,
        ///    but for saving I decided that only the relics would need to remember who they are attached to,
        ///    and so the player and phantoms are sorta constructed/deconstructed when this happens.
        // --------------------------------------------

        // Player
        SavedData.PlayerBattleData = new SavedPlayerData(DataManager.Instance.PlayerInstanceData);
        
        // Player's Phantoms
        SavedData.Phantoms.Clear();
        foreach(PhantomInstanceData phantom in PlayerInventoryManager.Instance.Phantoms)
        {
            SavedPhantomData savedPhantom = new SavedPhantomData(phantom);
            SavedData.Phantoms.Add(savedPhantom);
        }

        // Relics
        SavedData.Relics.Clear();
        foreach(RelicInstance relic in PlayerInventoryManager.Instance.Relics)
        {
            SavedRelicData savedRelic = new SavedRelicData(relic);
            SavedData.Relics.Add(savedRelic);
        }

        // Items
        SavedData.Items.Clear();
        foreach(ItemInstanceData item in PlayerInventoryManager.Instance.Items)
        {
            SavedItemData savedItem = new SavedItemData(item);
            SavedData.Items.Add(savedItem);
        }

        // Key Items
        SavedData.KeyItems.Clear();
        foreach(KeyItemInstanceData keyItem in PlayerInventoryManager.Instance.KeyItems)
        {
            SavedItemData savedItem = new SavedItemData(keyItem);
            SavedData.KeyItems.Add(savedItem);
        }

        // Scene and position
        SavedData.Scene = LoadingManager.Instance.LastOverworldScene.path;
        Debug.Log(PlayerRespawnManager.Instance.SafeRespawnPoint);
        Vector3 spawn = PlayerRespawnManager.Instance.SafeRespawnPoint;
        SavedData.ScenePosition = new Vector3(spawn.x, spawn.y, spawn.z);
        Debug.Log(SavedData.ScenePosition);

        // Other
        SavedData.Drops = DataManager.CurrentDrops;
        SavedData.ChosenStarterID = System.String.Copy(DataManager.Instance.ChosenStarterID);
        SavedData.KindredInstanceID = System.String.Copy(DataManager.Instance.KindredInstanceID);
        SavedData.StartersOrder = DataManager.Instance.StartersOrder.ConvertAll(starter => System.String.Copy(starter)); // Deep copy of the list of strings
        SavedData.StarterChoices = DataManager.Instance.StarterChoices.ConvertAll(starter => System.String.Copy(starter));

        SaveSlotManager.Save();
    }

    // The reverse of the previous function, updating all of our static data with the SavedData.
    public static void UpdateStaticData()
    {
        // Flags
        m_flags.Clear();
        foreach(Flag flag in SavedData.Flags)
        {
            m_flags.Add(flag.ID, flag.Value);
        }

        // Player
        PlayerBattleInstanceData playerData = new PlayerBattleInstanceData(DataManager.Instance.PlayerBattleData);
        SavedPlayerData savedPlayerData = SavedData.PlayerBattleData;
        playerData.CurrentHP = savedPlayerData.CurrentHP;
        playerData.CurrentMana = savedPlayerData.CurrentMana;
        playerData.Experience = savedPlayerData.Experience;
        playerData.LevelUps = new LevelUpStats(savedPlayerData.LevelUps);
        playerData.Data = DataManager.Instance.PlayerBattleData;
        DataManager.Instance.PlayerInstanceData = playerData;
        
        // Player's Phantoms
        PlayerInventoryManager.Instance.Phantoms.Clear();
        foreach(SavedPhantomData savedPhantom in SavedData.Phantoms)
        {
            PhantomInstanceData phantomInstance = new PhantomInstanceData(DataManager.Instance.TryGetPhantomData(savedPhantom.PhantomID));
            Debug.Log(phantomInstance.Data.DisplayName);
            phantomInstance.InstanceID = savedPhantom.InstanceID; // VERY IMPORTANT - make sure that it gets the same ID so that relics can be re-equipped properly
            phantomInstance.CurrentHP = savedPhantom.CurrentHP;
            phantomInstance.CurrentMana = savedPhantom.CurrentMana;
            phantomInstance.Experience = savedPhantom.Experience;
            phantomInstance.LevelUps = new LevelUpStats(savedPhantom.LevelUps);
            phantomInstance.NickName = savedPhantom.NickName;
            phantomInstance.Background = savedPhantom.Background;
            PlayerInventoryManager.Instance.AddPhantom(phantomInstance);
        }

        // Relics
        PlayerInventoryManager.Instance.Relics.Clear();
        foreach(SavedRelicData savedRelic in SavedData.Relics)
        {
            // Create the relic instance, then try to equip it to a player or phantom
            RelicInstance relicInstance = new RelicInstance(DataManager.Instance.TryGetRelicData(savedRelic.ID));
            PlayerInventoryManager.Instance.Relics.Add(relicInstance);

            if (savedRelic.UserID == PlayerBattleInstanceData.PLAYER_INSTANCE_ID)
            {
                DataManager.Instance.PlayerInstanceData.EquipRelic(relicInstance); // Player  will always have the same instance ID, worth checking first.
            }
            else if (!string.IsNullOrEmpty(savedRelic.UserID)) // If there is no user ID, assume this relic is unequipped.
            {
                foreach(PhantomInstanceData phantom in PlayerInventoryManager.Instance.Phantoms)
                {
                    if (phantom.InstanceID == savedRelic.UserID)
                    {
                        phantom.EquipRelic(relicInstance);
                    }
                }
            }
        }

        // Items
        PlayerInventoryManager.Instance.Items.Clear();
        foreach(SavedItemData savedItem in SavedData.Items)
        {
            ItemInstanceData itemInstance = new ItemInstanceData(DataManager.Instance.TryGetItemData(savedItem.ID));
            itemInstance.Quantity = savedItem.Quantity;
            PlayerInventoryManager.Instance.Items.Add(itemInstance);
        }

        // Key Items
        PlayerInventoryManager.Instance.KeyItems.Clear();
        foreach(SavedItemData savedKeyItem in SavedData.KeyItems)
        {
            KeyItemInstanceData keyItemInstance = new KeyItemInstanceData(DataManager.Instance.TryGetKeyItemData(savedKeyItem.ID));
            keyItemInstance.Quantity = savedKeyItem.Quantity;
            PlayerInventoryManager.Instance.KeyItems.Add(keyItemInstance);
        }

        // Other
        DataManager.CurrentDrops = SavedData.Drops;
        DataManager.Instance.ChosenStarterID = System.String.Copy(SavedData.ChosenStarterID);
        DataManager.Instance.KindredInstanceID = System.String.Copy(SavedData.KindredInstanceID);
        DataManager.Instance.StartersOrder = SavedData.StartersOrder.ConvertAll(starter => System.String.Copy(starter)); // Deep copy of the list of strings
        DataManager.Instance.StarterChoices = SavedData.StarterChoices.ConvertAll(starter => System.String.Copy(starter));

        // LAST PART is loading into the correct scene and position!
        // Gonna be pretty naive about this for now, just always assume that we CAN load and
        // not really check if its safe.
        OverworldManager.Instance.QueuePlayerSpawnPoint(SavedData.ScenePosition);
        LoadingManager.LoadScene(SavedData.Scene);
    }

    public static void ResetStaticData()
    {
        m_flags.Clear();

        // Player
        PlayerBattleInstanceData playerData = new PlayerBattleInstanceData(DataManager.Instance.PlayerBattleData);

        // Player's Phantoms
        PlayerInventoryManager.Instance.Phantoms.Clear();

        // Relics
        PlayerInventoryManager.Instance.Relics.Clear();

        // Items
        PlayerInventoryManager.Instance.Items.Clear();

        // Key Items
        PlayerInventoryManager.Instance.KeyItems.Clear();

        // Other
        DataManager.CurrentDrops = 0;
        DataManager.Instance.ChosenStarterID = string.Empty;
        DataManager.Instance.StartersOrder = new List<string>();
        DataManager.Instance.StarterChoices = new List<string>();
    }
    
    ////////////////////
    /// Flags Stuff! ///
    ////////////////////
    public static bool CheckFlag(string flagName)
    {
        if (string.IsNullOrEmpty(flagName))
        {
            return false;
        }

        if (m_flags.ContainsKey(flagName) && m_flags[flagName])
        {
            return true;
        }
        return false;
    }

    public static void SetFlag(string flagName, bool flagValue = true)
    {
        if (string.IsNullOrEmpty(flagName))
        {
            return;
        }

        if (m_flags.ContainsKey(flagName))
        {
            m_flags[flagName] = flagValue;
        }
        else
        {
            m_flags.Add(flagName, flagValue);
        }

        OnFlagUpdate.Invoke(flagName, flagValue);
    }
}
