using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEffect
{
    public int index;
    public FancyTextEffect Effect;

    public void Apply(int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        Effect.ApplyEffect(index, vertexIndex, sourceVertices, ref destinationVertices, ref newVertexColors);
    }
}