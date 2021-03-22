using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeOverworld : CameraShake
{
    private static CameraShakeOverworld s_instance = null;
    public static CameraShakeOverworld Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<CameraShakeOverworld>();
            }
            return s_instance;
        }
    }
}
