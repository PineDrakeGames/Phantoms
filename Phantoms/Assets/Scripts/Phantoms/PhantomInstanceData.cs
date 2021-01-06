using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomInstanceData
{
    public string PhantomID = null;

    public string NickName = null;

    public int Level = 0;

    public PhantomBackground Background = PhantomBackground.None;

    public BattleStats LevelUps = new BattleStats();

    public BattleStats CurrentStats = new BattleStats();
}
