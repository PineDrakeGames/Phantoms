using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerCameraState
{
    public CameraController Controller;

    public abstract void GetCameraPosition(ref Vector3 position, ref Quaternion rotation);
}
