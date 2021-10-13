using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInstanceData
{
    public ItemData Data = null;
    public Ares.ItemData AresData
    {
        get
        {
            if (Data) { return Data.AresData; }
            return null;
        }
    }

    public int Quantity = 0;

    public ItemInstanceData(ItemData data)
    {
        Data = data;
        Quantity = 0;
    }

    public ItemInstanceData(ItemInstanceData instanceData)
    {
        Data = instanceData.Data;
        Quantity = instanceData.Quantity;
    }
}
