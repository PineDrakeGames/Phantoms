using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{

    /// Static Instance stuff ///
    private static PlayerInventoryManager s_instance = null;
    public static PlayerInventoryManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                GameObject inventoryManager = Instantiate(new GameObject("Inventory Manager"));
                s_instance = inventoryManager.AddComponent<PlayerInventoryManager>();
                DontDestroyOnLoad(inventoryManager);
                s_instance.Initialize();
            }
            return s_instance;
        }
    }

    // Stuff stored in the inventory
    public List<ItemInstanceData> Items = new List<ItemInstanceData>();
    public List<PhantomInstanceData> Phantoms = new List<PhantomInstanceData>();
    public List<RelicInstance> Relics = new List<RelicInstance>();
    public List<KeyItemInstanceData> KeyItems = new List<KeyItemInstanceData>();

    // We are always gonna try and have the current phantom first in the list - if that ever changes,
    // make this number update!
    public const int CurrentActivePhantom = 0;

    public void Initialize()
    {
        // Any stuff that we gotta do first
    }

    /////////////////////////////////////////////////////////
    /// Public functions relating to items used in battle ///
    /////////////////////////////////////////////////////////
    public void AddItem(string itemID, int numItem = 1)
    {
        foreach (ItemInstanceData item in Items)
        {
            if (item.Data.ItemID == itemID)
            {
                item.Quantity += numItem;
                return;
            }
        }

        // If we reach this point, the given item isn't already in the inventory, so add it in
        ItemData data = DataManager.Instance.TryGetItemData(itemID);
        if (data != null)
        {
            ItemInstanceData instanceData = new ItemInstanceData(data);
            instanceData.Quantity = numItem;
            Items.Add(instanceData);
        }
        else
        {
            Debug.LogError("Trying to add item of ID " + itemID + " but couldn't.");
        }
    }

    public void UseItem(string itemID, int numUsed = 1)
    {
        foreach (ItemInstanceData item in Items)
        {
            if (item.Data.ItemID == itemID)
            {
                item.Quantity -= numUsed;
                return;
            }
        }
    }

    public Ares.Inventory CreateBattleInventory()
    {
        Ares.Inventory battleInventory = new Ares.StackedInventory();
        battleInventory.OnItemAdd.AddListener(SetupBattleItem);
        foreach (ItemInstanceData item in Items)
        {
            if (item.Quantity > 0)
            {
                battleInventory.AddItem(item.AresData, item.Quantity);
            }
        }
        return battleInventory;
    }

    public void SetupBattleItem(Ares.Item item, int quantity)
    {
        item.OnConsumed.AddListener(oldRemainingUses =>
        {
            string itemID = DataManager.Instance.TryGetItemData(item.Data).ItemID;
            UseItem(itemID);
        });
    }

    public void SaveBattleInventory(Ares.StackedInventory battleInventory)
    {
        foreach (Ares.StackedItem item in battleInventory.Items)
        {
            // TODO: Update inventory!
            Debug.Log(item.Item.Data.DisplayName);
        }
    }

    //////////////////////////////////////////////////////
    /// Public functions relating to phantoms captured ///
    //////////////////////////////////////////////////////
    public void AddPhantom(PhantomInstanceData newPhantom)
    {
        Phantoms.Add(newPhantom);
    }

    public PhantomInstanceData GetCurrentPhantom()
    {
        if (CurrentActivePhantom < Phantoms.Count)
        {
            return Phantoms[CurrentActivePhantom];
        }
        else
        {
            return null;
        }
    }

    public void SetCurrentPhantom(PhantomInstanceData phanData)
    {
        for (int i = 0; i < Phantoms.Count; i++)
        {
            PhantomInstanceData data = Phantoms[i];
            if (data == phanData)
            {
                Phantoms.RemoveAt(i);
                Phantoms.Insert(0, data);
                return;
            }
        }
    }

    //////////////////////////////////////////
    /// Public functions related to Relics ///
    //////////////////////////////////////////

    public void AddRelic(string relicID)
    {
        AddRelic(DataManager.Instance.TryGetRelicData(relicID));
    }

    public void AddRelic(RelicData data)
    {
        Relics.Add(new RelicInstance(data));
    }

    // Equip relic functions
    public void EquipRelic(RelicInstance relic, CombatantInstanceData combatant)
    {
        if (combatant is PhantomInstanceData)
        {
            EquipRelic(relic, combatant as PhantomInstanceData);
        }
        else if (combatant is PlayerBattleInstanceData)
        {
            EquipRelic(relic, combatant as PlayerBattleInstanceData);
        }
    }
    public void EquipRelic(RelicInstance relic, PhantomInstanceData phantom)
    {
        phantom.EquipRelic(relic);
    }
    public void EquipRelic(RelicInstance relic, PlayerBattleInstanceData player)
    {
        player.EquipRelic(relic);
    }

    // Unequip Relic Functions
    public void UnequipRelic(RelicInstance relic, CombatantInstanceData combatant)
    {
        if (combatant is PhantomInstanceData)
        {
            UnequipRelic(relic, combatant as PhantomInstanceData);
        }
        else if (combatant is PlayerBattleInstanceData)
        {
            UnequipRelic(relic, combatant as PlayerBattleInstanceData);
        }
    }
    public void UnequipRelic(RelicInstance relic, PhantomInstanceData phantom)
    {
        phantom.UnequipRelic(relic);
    }
    public void UnequipRelic(RelicInstance relic, PlayerBattleInstanceData player)
    {
        player.UnequipRelic(relic);
    }


    //////////////////////////////////////
    /// Public Functions for Key Items ///
    //////////////////////////////////////

    public void AddKeyItem(string itemID, int quantity = 1)
    {
        if (quantity <= 0)
        {
            return;
        }

        foreach (KeyItemInstanceData item in KeyItems)
        {
            if (item.Data.ItemID == itemID)
            {
                item.Quantity += quantity;
                return;
            }
        }

        // If we reach this point, the given item isn't already in the inventory, so add it in
        KeyItemData data = DataManager.Instance.TryGetKeyItemData(itemID);
        if (data != null)
        {
            KeyItemInstanceData instanceData = new KeyItemInstanceData(data);
            instanceData.Quantity = quantity;
            KeyItems.Add(instanceData);
        }
        else
        {
            Debug.LogError("Trying to add item of ID " + itemID + " but couldn't.");
        }
    }

    public void RemoveKeyItem(string itemID, int numToRemove = 1)
    {
        KeyItemInstanceData keyItem = null;
        foreach (KeyItemInstanceData item in KeyItems)
        {
            if (item.Data.ItemID == itemID)
            {
                keyItem = item;
                break;
            }
        }
        keyItem.Quantity -= numToRemove;
        if (keyItem.Quantity <= 0)
        {
            KeyItems.Remove(keyItem);
        }
    }
}
