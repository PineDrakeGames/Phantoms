using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItemInstanceData
{
    public KeyItemData Data = null;

    public int Quantity = 0;

    public KeyItemInstanceData(KeyItemData data)
    {
        Data = data;
        Quantity = 0;
    }
}
