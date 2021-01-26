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

    public Ares.Inventory CreateBattleInventory()
    {
        Ares.Inventory battleInventory = new Ares.StackedInventory();
        foreach (ItemInstanceData item in Items)
        {
            battleInventory.AddItem(item.AresData, item.Quantity);
        }
        return battleInventory;
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

    }
}
