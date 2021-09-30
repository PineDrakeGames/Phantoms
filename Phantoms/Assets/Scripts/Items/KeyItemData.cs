using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ares;

[CreateAssetMenu(menuName = "Phantoms/Key Item Data")]
public class KeyItemData : ScriptableObject
{
    public string ItemID = null;

    [Header("Display Stuff")]
    public Sprite Sprite = null;
    public string DisplayName = null;
    [TextArea]
    public string Description = null;

    [Header("Special Settings")]
    public bool CanStack = false;
}
