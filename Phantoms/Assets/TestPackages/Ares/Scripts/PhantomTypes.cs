using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PhantomType
{
    NONE = 0,
    LOVE = 1,
    FEAR = 2,
    ANGER = 3,
    SADNESS = 4,
    CALM = 5,
    HOPE = 6,
    JOY = 7,
    SHAME = 8,
}

// Such that, when divided by 2, is what is multiplied by the damage.
public enum TypeMultiplier
{
    DEFAULT = 2,
    WEAK = 4,
    RESISTANT = 1,
    IMMUNE = 0
}

public static class PhantomTypes
{
    private static PhantomTypeChart s_typeChart = null;

    public static float GetTypeMultiplier(PhantomType attack, PhantomType defender)
    {
        if (s_typeChart == null)
        {
            s_typeChart = Resources.Load<PhantomTypeChart>("PhantomTypeChart");
            if (s_typeChart == null)
            {
                Debug.LogError("Type chart cannot be found, using default multiplier of 1.");
                return 1f;
            }
        }

        TypeMultiplier multiplier = s_typeChart.TypeChart[attack][defender];

        return ((sbyte)multiplier / 2f);
    }

}
