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

    [Header("Overworld Item Use")]
    public bool CanUseOutOfBattle = false;
    public bool HealsAll = false;
    public int HP = 0;
    public int Mana = 0;

    [Header("For Shop Stuff")]
    public int BuyPrice = 10;
    public int SellPrice = 5;
}
