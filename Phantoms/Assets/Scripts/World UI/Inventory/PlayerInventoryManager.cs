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

    private int m_currentActivePhantom = 0;
    public int CurrentActivePhantom
    {
        get { return m_currentActivePhantom; }
        set
        {
            if (value >= Phantoms.Count) 
            {
                m_currentActivePhantom = Phantoms.Count - 1; 
            }
            else
            {
                m_currentActivePhantom = value;
            }

            if (m_currentActivePhantom < 0) 
            { 
                m_currentActivePhantom = 0; 
            }
        }
    }

    public void Initialize()
    {
        // Any stuff that we gotta do first
    }

    /////////////////////////////////////////////////////////
    /// Public functions relating to items used in battle ///
    /////////////////////////////////////////////////////////
    public void AddItem(string itemID, int numItem = 1)
    {
        foreach(ItemInstanceData item in Items)
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
        foreach(ItemInstanceData item in Items)
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
            battleInventory.AddItem(item.AresData, item.Quantity);
        }
        return battleInventory;
    }

    public void SetupBattleItem(Ares.Item item, int quantity)
    {
        item.OnConsumed.AddListener(oldRemainingUses => {
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
                CurrentActivePhantom = i;
                return;
            }
        }
    }
}
