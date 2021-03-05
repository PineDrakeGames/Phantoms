using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LootDropData : ScriptableObject
{
    public abstract List<string> GetItemDrops();
    public abstract int GetDropDrops();
}
