using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyEncounterData : ScriptableObject
{
    public abstract List<PhantomInstanceData> GetEnemyPhantoms();
}