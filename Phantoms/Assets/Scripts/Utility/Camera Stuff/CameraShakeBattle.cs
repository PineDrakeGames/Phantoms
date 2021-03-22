using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeBattle : CameraShake
{
    private static CameraShakeBattle s_instance = null;
    public static CameraShakeBattle Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<CameraShakeBattle>();
            }
            return s_instance;
        }
    }
}
