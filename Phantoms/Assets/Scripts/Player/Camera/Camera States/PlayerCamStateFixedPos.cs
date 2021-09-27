using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamStateFixedPos : PlayerCameraState
{
    public Vector3 FixedPosition = Vector3.zero;
    public Quaternion FixedRotation = Quaternion.identity;

    public override void GetCameraPosition(ref Vector3 position, ref Quaternion rotation)
    {
        position = FixedPosition;
        rotation = FixedRotation;
    }
}
