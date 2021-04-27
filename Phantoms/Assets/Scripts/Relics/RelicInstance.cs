using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RelicInstance : MonoBehaviour
{
    public RelicData Data = null;
    public bool Equipped = false;
    public UserBattleInstanceData User = null;

    public RelicInstance()
    {
        Data = null;
        Equipped = false;
        User = null;
    }

    public RelicInstance(RelicData data)
    {
        Data = data;
        Equipped = false;
        User = null;
    }
}
