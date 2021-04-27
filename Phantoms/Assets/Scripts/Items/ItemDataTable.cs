using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Phantoms/Data Tables/Item Data Table")]
public class ItemDataTable : ScriptableObject
{
    public ItemData[] Data = null;
}
