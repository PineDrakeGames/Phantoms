using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ares;

[CreateAssetMenu(menuName = "Phantoms/Item Data")]
public class ItemData : ScriptableObject
{
    public string ItemID = null;
    public Ares.ItemData AresData = null;

    public Sprite Sprite = null;

    public int BuyPrice = 10;
    public int SellPrice = 5;
}
